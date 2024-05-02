// 2024-04-30 RZN

using UnityEngine;
using UnityEngine.InputSystem;

namespace AlienProject
{
	/// <summary>
	/// 캐릭터 움직임 컴포넌트들의 기본 추상 클래스입니다.
	/// </summary>
	[RequireComponent(typeof(PlayerInput))]
	[RequireComponent(typeof(CharacterController))]
	public abstract partial class MovementActionBase : MonoBehaviour
	{
		// MARK: Component Caching
		protected CharacterController _characterController;

		// MARK: Members - Inspector Exposed

		[Header("기본 이동 설정")]

		[SerializeField]
		[Range(0f, 20f)]
		[Tooltip("캐릭터의 최대 이동 속도")]
		protected float _maxSpeed = 5f;

		[SerializeField]
		[Range(0f, 360f)]
		[Tooltip("캐릭터의 회전 속도")]
		protected float _rotationSpeed = 5f;

		[SerializeField]
		[Range(0.001f, 0.5f)]
		[Tooltip("캐릭터의 이동 정지 임계 속력값.")]
		protected float _movementStopThresholdSpeed = 0.1f;

		[SerializeField]
		[Tooltip("캐릭터에 중력을 적용할지 여부를 결정합니다.")]
		private bool _applyGravity = true;

		// MARK: Properties

		/// <summary>
		/// <para>플레이어가 바라보는 방향을 기준으로 해석된 입력 벡터로, 플레이어가 캐릭터를 움직이고자 하는 방향을 나타냅니다.</para>
		/// <para>캐릭터의 가속도의 방향으로 직역할 수 있으며, 속도의 방향과는 다를 수도 있음을 주의합니다.</para>
		/// </summary>
		public Vector3 TranslatedMovementInput
		{
			get
			{
				var cameraYawRotation = Quaternion.Euler(0f, Camera.main.transform.eulerAngles.y, 0f);
				return cameraYawRotation * _rawMovementInput;
			}
		}

		/// <summary>
		/// 최대 속력에 대한 현재 속력의 비율입니다. (0.0 ~ 1.0)
		/// </summary>
		public float SpeedRatio => MovementSpeed / _maxSpeed;

		protected float MovementSpeed => _movementVelocity.magnitude;
		protected Vector3 MovementDirection => _movementVelocity.normalized;

		// MARK: Properties - Checker

		/// <summary>
		/// <para>캐릭터가 현재 움직이고 있는지의 여부를 반환합니다.</para>
		/// <para>속력이 0보다 크면 움직이고 있는 것으로 간주하며, 플레이어 입력이 있는지 없는지는 고려하지 않습니다.</para>
		/// </summary>
		public bool IsMoving => MovementSpeed > 0f;
		protected bool IsGrounded => _characterController.isGrounded;
		protected bool IsPlayerDesiredToMove => _rawMovementInput != Vector3.zero;

		// MARK: Members

		protected Vector3 _movementVelocity;
		protected bool _isMovementDisabled = false;
		private Vector3 _rawMovementInput;

		#region Unity Callbacks

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
				ApplyGravity();
			}
		}

		/// <summary>
		/// PlayerInput 컴포넌트에 등록 해야 합니다.
		/// </summary>
		public void OnInput_MovementAction(InputAction.CallbackContext context)
		{
			var rawInputVector2 = context.ReadValue<Vector2>();
			_rawMovementInput = new Vector3(rawInputVector2.x, 0, rawInputVector2.y);
		}

		public virtual partial void OnDrawGizmos();

		#endregion // Unity Callbacks

		// MARK: Methods

		protected abstract void Move();

		protected void TryClampMaxMovementVelocity()
		{
			if (MovementSpeed > _maxSpeed)
			{
				_movementVelocity = MovementDirection * _maxSpeed;
			}
		}
		protected void TryClampMinMovementVelocity()
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
		private void Rotate()
		{
			if (!IsPlayerDesiredToMove)
			{
				return;
			}

			if (!IsMoving)
			{
				return;
			}

			if (MovementDirection == Vector3.zero)
			{
				return;
			}

			// Lerp 사용하여 부드러운 회전 구현
			transform.rotation = Quaternion.Slerp
			(
				transform.rotation,
				Quaternion.LookRotation(MovementDirection),
				_rotationSpeed * Time.deltaTime
			);
		}
		private void ApplyGravity()
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

			// 행동 중 이동을 허용하는 종류의 회피라면 회피의 시작과 끝에 이벤트를 등록할 필요가 없다.
			if (dodgeAction.AllowMovementActionWhileDodging)
			{
				return;
			}

			dodgeAction.OnDodgeActionBegin.AddListener(DisableMovement);
			dodgeAction.OnDodgeActionEnd.AddListener(EnableMovement);
		}

		private void DisableMovement()
		{
			_isMovementDisabled = true;
		}

		private void EnableMovement()
		{
			_isMovementDisabled = false;
		}

	}
} // namespace AlienProject
