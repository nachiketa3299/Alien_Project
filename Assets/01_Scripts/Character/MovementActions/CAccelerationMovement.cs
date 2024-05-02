// 2024-04-25 RZN

using UnityEngine;

namespace AlienProject
{
	/// <summary>
	/// 가속도와 감속도의 영향을 받는 움직임을 구현하는 컴포넌트입니다.
	/// </summary>
	[AddComponentMenu("Alien Project/Character/Movement Actions/Acceleration Movement Component")]
	public partial class CAccelerationMovement : MovementActionBase
	{
		[Header("가속도와 감속도 설정")]

		[SerializeField]
		[Range(0f, 20f)]
		private float _accelerationMagnitude = 15f;

		[SerializeField]
		[Range(0f, 100f)]
		private float _decelerationMagnitude = 20f;

		private Vector3 _accelerationDirection;
		private Vector3 _decelerationDirection;

		private Vector3 Acceleration => _accelerationDirection * _accelerationMagnitude;
		private Vector3 Deceleration => _decelerationDirection * _decelerationMagnitude;

		protected override void Move()
		{
			// 플레이어 입력이 있다 -> 가속도 적용
			if (IsPlayerDesiredToMove)
			{
				_accelerationDirection = TranslatedMovementInput;
				_decelerationDirection = Vector3.zero;

				_movementVelocity += Acceleration * Time.deltaTime;

				TryClampMaxMovementVelocity();
			}
			// 플레이어 입력이 없다 -> 감속도 적용
			else if (!IsPlayerDesiredToMove)
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

		public override partial void OnDrawGizmos();

	} // class CAccelerationMovement
} // namespace AlienProject