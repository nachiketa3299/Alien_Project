// 2024-04-25 RZN

#if false
// TODO 제대로 구현할 것

using UnityEngine;

namespace AlienProject
{

	/// <summary>
	/// 회피 동작을 실행하면 플레이어의 속도와 가속도가 증가하는 등의 "버프"형 회피 액션 구현에 사용되는 컴포넌트
	/// </summary>
	[AddComponentMenu("Alien Project/Character/Dodge Actions/Buff Dodge Component")]
	[RequireComponent(typeof(MovementActionBase))]
	public class CBuffDodgeAction : DodgeActionBase
	{
		protected MovementActionBase _movementModel;

		[SerializeField]
		[Range(1f, 10f)]
		private float _buffTime = 3f;
		[SerializeField]
		[Range(1f, 3f)]
		private float _maxSpeedMultiplier = 1.5f;

		[SerializeField]
		[Range(1f, 3f)]
		private float _accelerationMagnitudeMultiplier = 1.5f;

		private float _cachedMaxSpeed;
		private float _cachedAccelerationMagnitude;

		#region Unity Events

		protected override void Awake()
		{
			base.Awake();

			_movementModel = GetComponent<MovementActionBase>();

			_cachedMaxSpeed = _movementModel.MaxSpeed;
			_cachedAccelerationMagnitude = _movementModel.AccelerationMagintude;
		}

		#endregion

		public override void DodgeStart()
		{
			if (_isDodging)
			{
				return;
			}

			base.DodgeStart();

			_cachedMaxSpeed = _movementModel.MaxSpeed;
			_cachedAccelerationMagnitude = _movementModel.AccelerationMagintude;

			var newMaxSpeed = _movementModel.MaxSpeed * _maxSpeedMultiplier;
			var newAccelerationMagnitude = _movementModel.AccelerationMagintude * _accelerationMagnitudeMultiplier;

			_movementModel.MaxSpeed = newMaxSpeed;
			_movementModel.AccelerationMagintude = newAccelerationMagnitude;

			Invoke(nameof(DodgeEnd), _buffTime);
		}

		public override void DodgeEnd()
		{
			base.DodgeEnd();

			_movementModel.MaxSpeed = _cachedMaxSpeed;
			_movementModel.AccelerationMagintude = _cachedAccelerationMagnitude;
		}
	}

} // AlienProject

#endif // false