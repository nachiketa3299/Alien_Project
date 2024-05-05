// 2024-04-25 RZN

using UnityEngine;
using UnityEngine.InputSystem;

namespace AlienProject
{
	/// <summary>
	/// 가속도와 감속도의 영향을 받는 움직임을 구현하는 컴포넌트입니다.
	/// </summary>
	[DisallowMultipleComponent]
	[RequireComponent(typeof(PlayerInput))]
	[RequireComponent(typeof(CharacterController))]
	[AddComponentMenu("Alien Project/Character/Movement Action Component")]
	public partial class CMovementAction : MonoBehaviour
	{
		public enum EMovingState
		{
			Idle,
			Moving
		}

		public enum ETurningState
		{
			None,
			Turning
		}

		private CharacterController _characterController;

		// MARK: Inspector

		[Header("기본 이동 설정")]

		[SerializeField] private float _maxSpeed = 5f;
		[SerializeField] private float _rotationSpeed = 8f;
		[SerializeField] private float _movementStopThresholdSpeed = 0.1f;
		[SerializeField] private bool _applyGravity = true;

		[Header("가속도와 감속도 설정")]

		[SerializeField] private float _accelerationMagnitude = 15f;
		[SerializeField] private float _decelerationMagnitude = 20f;

		private Vector3 _accelerationDirection;
		private Vector3 _decelerationDirection;

		private Vector3 Acceleration => _accelerationDirection * _accelerationMagnitude;
		private Vector3 Deceleration => _decelerationDirection * _decelerationMagnitude;

		// MARK: Properties

		public Vector3 TranslatedMovementInput
		{
			get
			{
				var cameraYawRotation = Quaternion.Euler(0f, Camera.main.transform.eulerAngles.y, 0f);
				return cameraYawRotation * _rawMovementInput;
			}
		}

		public float MovementSpeed => _movementVelocity.magnitude;
		public Vector3 MovementDirection => _movementVelocity.normalized;
		public float MovementSpeedRatio => MovementSpeed / _maxSpeed;

		public bool IsMoving => MovementSpeed > 0f;
		public bool IsGrounded => _characterController.isGrounded;
		public bool IsPlayerDesiredToMove => _rawMovementInput != Vector3.zero;


		// [0f, 180f] 사이 값 반환
		private float AngleDiffForwardAndMovement
		{
			get
			{
				if (IsPlayerDesiredToMove)
				{
					var angle = Vector3.Angle(transform.forward, _accelerationDirection);
					return angle;
				}
				else
				{
					return 0f;
				}
			}
		}
		private bool NeedToRotate => AngleDiffForwardAndMovement > _turningStateThreshold;

		// MARK: Members

		// Movement States
		private ETurningState _turningState = ETurningState.None;
		private EMovingState _movingState = EMovingState.Idle;

		private Vector3 _movementVelocity;
		private bool _isMovementDisabled = false;
		private Vector3 _rawMovementInput;
		private float _turningStateThreshold = 10f;

		#region Unity Events

		private void Reset()
		{
			TryInitializeDodgeAction();
		}

		private void Awake()
		{
			_characterController = GetComponent<CharacterController>();

			TryInitializeDodgeAction();
		}

		private partial void Start();

		private void FixedUpdate()
		{

			if (!_isMovementDisabled)
			{
				Move();
				Rotate();
			}


			if (_applyGravity)
			{
				TryApplyGravity();
			}
		}

		private void LateUpdate()
		{
			if (IsMoving)
			{
				_movingState = EMovingState.Moving;
				_turningState = NeedToRotate ? ETurningState.Turning : ETurningState.None;
			}
			else
			{
				_movingState = EMovingState.Idle;

				if (!IsPlayerDesiredToMove)
				{
					_turningState = ETurningState.None;
				}
				else
				{
					_turningState = NeedToRotate ? ETurningState.Turning : ETurningState.None;
				}
			}
		}

		public void OnInput_MovementAction(InputAction.CallbackContext context)
		{
			var rawInputVector2 = context.ReadValue<Vector2>();
			_rawMovementInput = new Vector3(rawInputVector2.x, 0f, rawInputVector2.y);
		}

		private partial void OnDrawGizmos();

		#endregion // Unity Events

		// MARK: Methods

		public void DisableMovement() => _isMovementDisabled = true;

		public void EnableMovement() => _isMovementDisabled = false;

		private void Move()
		{
			// 플레이어 입력이 있다 -> 가속도 적용
			if (IsPlayerDesiredToMove)
			{
				_accelerationDirection = TranslatedMovementInput;
				_decelerationDirection = Vector3.zero;

				if (NeedToRotate && !IsMoving)
				{
					return;
				}

				_movementVelocity += Acceleration * Time.deltaTime;

				TryClampMaxMovementVelocity();

			}
			// 플레이어 입력이 없다 -> 감속도 적용
			else
			{
				_accelerationDirection = Vector3.zero;

				if (IsMoving)
				{
					_accelerationDirection = Vector3.zero;

					_decelerationDirection = -1 * MovementDirection;
					_movementVelocity += Deceleration * Time.deltaTime;

					TryClampMinMovementVelocity();
				}
				else
				{
					_decelerationDirection = Vector3.zero;
				}


			}

			if (IsMoving)
			{
				_characterController.Move(_movementVelocity * Time.deltaTime);
			}
		}
		private void Rotate()
		{
			// TODO 감속도가 매우 적은 경우에 이게 말이 되는지 테스트
			if (!IsPlayerDesiredToMove)
			{
				return;
			}

			if (!IsMoving)
			{
				if (NeedToRotate)
				{
					transform.rotation = Quaternion.Slerp
					(
						transform.rotation,
						Quaternion.LookRotation(_accelerationDirection),
						_rotationSpeed * Time.deltaTime
					);
				}

				return;
			}

			if (MovementDirection == Vector3.zero)
			{
				return;
			}

			transform.rotation = Quaternion.Slerp
			(
				transform.rotation,
				Quaternion.LookRotation(MovementDirection),
				_rotationSpeed * Time.deltaTime
			);
		}

		private void TryClampMaxMovementVelocity()
		{
			if (MovementSpeed > _maxSpeed)
			{
				_movementVelocity = MovementDirection * _maxSpeed;
			}
		}

		private void TryClampMinMovementVelocity()
		{
			if (MovementSpeed <= _movementStopThresholdSpeed)
			{
				_movementVelocity = Vector3.zero;
			}
			else // 이게 없으면 Deceleration이 앞뒤로 Flickering 하는 문제가 발생
			{
				// 간단하게 이동 방향과 캐릭터 방향이 반대일 경우 이동을 아예 멈추도록 한다.
				var cos = Vector3.Dot(transform.forward, MovementDirection);

				if (cos < 0f)
				{
					_movementVelocity = Vector3.zero;
				}
			}
		}


		private void TryApplyGravity()
		{
			if (IsGrounded)
			{
				return;
			}

			_characterController.Move(new Vector3(0, Physics.gravity.y, 0) * Time.deltaTime);
		}

		private void TryInitializeDodgeAction()
		{
			var dodgeAction = GetComponent<DodgeActionBase>();

			if (!dodgeAction)
			{
				return;
			}

			// 회피 동작 중에도 이동이 가능하도록 설정되어 있지 않다면 이동을 막는다.
			if (dodgeAction.AllowMovementActionWhileDodging)
			{
				return;
			}

			dodgeAction.OnDodgeActionBegin.AddListener(DisableMovement);
			dodgeAction.OnDodgeActionBegin.AddListener(EnableMovement);
		}



	} // class CMovementAction
} // namespace AlienProject