#pragma warning disable 0414

using UnityEngine;


namespace AlienProject
{
	/// <summary>
	/// 가속도와 감속도의 영향을 받는 움직임을 구현하는 컴포넌트입니다.
	/// </summary>
	[AddComponentMenu("Alien Project/Character/Acceleration Movement Action Component")]
	public partial class CAccelerationMovement : MovementActionBase, IInitializable
	{
		public enum ETurningState
		{
			None,
			Turning
		}

		// MARK: Inspector

		[SerializeField] private float _rotationSpeed = 8f;

		[SerializeField] private float _movementStopThresholdSpeed = 0.1f;

		[SerializeField] private bool _idleTurning = false;

		[SerializeField] private float _accelerationMagnitude = 15f;

		[SerializeField] private float _decelerationMagnitude = 20f;

		#region IInitializable Impelementation

		public void Initialize(PawnDataBase data)
		{
			_maxSpeed = data.movementData.maxSpeed;
			_rotationSpeed = data.movementData.rotationSpeed;
			_accelerationMagnitude = data.movementData.accelerationMagnitude;
			_decelerationMagnitude = data.movementData.decelerationMagnitude;
		}

		#endregion // IInitializable Impelementation

		// MARK: Properties

		// TODO Input 과 관계되도록 명확하게 재작성 (Base Class 에서 작성 필요할 듯)
		public bool IsDesiredToMove => _accelerationDirection != Vector3.zero;

		public bool IsTurning => _turningState == ETurningState.Turning;

		private bool NeedToRotate => AngleDiffForwardAndMovement > _turningStateThreshold;

		private Vector3 Acceleration => _accelerationDirection * _accelerationMagnitude;

		private Vector3 Deceleration => _decelerationDirection * _decelerationMagnitude;

		// [0f, 180f] 사이 값 반환
		private float AngleDiffForwardAndMovement
		{
			get
			{
				if (IsDesiredToMove)
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

		// MARK: Members

		private Vector3 _accelerationDirection;
		private Vector3 _decelerationDirection;

		//  States (Work in Progress)

		private ETurningState _turningState = ETurningState.None;
		private EMovingState _movingState = EMovingState.Idle;
		private float _turningStateThreshold = 10f;

		#region Unity Callbacks

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
				// TODO Moving 의 경우 이미 IsMoving이 테스트하고 있으므로 열거형이 불필요
				_movingState = EMovingState.Moving;
				_turningState = NeedToRotate ? ETurningState.Turning : ETurningState.None;
			}
			else
			{
				_movingState = EMovingState.Idle;

				if (!IsDesiredToMove)
				{
					_turningState = ETurningState.None;
				}
				else
				{
					_turningState = NeedToRotate ? ETurningState.Turning : ETurningState.None;
				}
			}
		}

		#endregion // Unity Events

		// MARK: Override Methods

		public override void MoveTo(Vector3 wordPosition)
		{
			_accelerationDirection = (wordPosition - transform.position).normalized;
		}

		public override void MoveToward(Vector3 wordDirection)
		{
			_accelerationDirection = wordDirection;
		}

		protected override void Move()
		{
			// 움직일 의도가 있다 -> 가속도가 존재한다 -> 가속
			if (IsDesiredToMove)
			{
				_decelerationDirection = Vector3.zero;

				if (_idleTurning) // NOTE Idle Turn을 나중에 구현하기 위해 남겨둚
				{
					if (NeedToRotate && !IsMoving)
					{
						return;
					}
				}

				_movementVelocity += Acceleration * Time.deltaTime;

				TryClampMaxMovementVelocity();

			}
			// 움직일 의도가 없다 -> 가속도가 없다 -> 감속
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

		// MARK: Methods

		private void Rotate()
		{
			// TODO 감속도가 매우 적은 경우에 이게 말이 되는지 테스트
			if (!IsDesiredToMove)
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
						_rotationSpeed * 2 * Time.deltaTime
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

	} // class CMovementAction
} // namespace AlienProject