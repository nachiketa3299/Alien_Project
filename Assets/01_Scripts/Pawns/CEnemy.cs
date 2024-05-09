
using UnityEngine;
using UnityEngine.Events;

namespace AlienProject
{
	[AddComponentMenu("Alien Project/Enemy Component")]
	[RequireComponent(typeof(SensorBase))]
	public class CEnemy : PawnBase
	{
		// MARK: Events

		// TODO 이름을 변경해야 할 확률이 매우 높음

		public UnityEvent<Transform> OnTargetDetected;
		public UnityEvent<Transform> OnTargetLost;
		public UnityEvent<Transform> OnTargetEmpty;

		public enum EEnemyState
		{
			Idle,
			Patrol,
			Chase
		}


		// MARK: Component Caching
		private SensorBase _sensor;

		// MARK: Members

		// MARK: Properties

		public EEnemyState State { get; private set; }

		#region Unity Callbacks

		protected override void Awake()
		{
			base.Awake();

			_sensor = GetComponent<SensorBase>();
		}

		protected override void Start()
		{
			base.Start();

			_sensor.OnTrackingTargetDetected.AddListener
			(
				(tr) =>
				{
					State = EEnemyState.Chase;

					Debug.Log($"[CEnemy] {_sensor}이 플레이어를 발견했습니다.");
					OnTargetDetected?.Invoke(tr);
				}
			);

			_sensor.OnTargetLost.AddListener
			(
				(tr) =>
				{
					State = EEnemyState.Patrol;

					Debug.Log($"[CEnemy] {_sensor}이 플레이어를 잃었습니다.");
					OnTargetLost?.Invoke(tr);
				}
			);

			_sensor.OnTargetEmpty.AddListener
			(
				() =>
				{
					State = EEnemyState.Idle;
					OnTargetEmpty?.Invoke(null);
				}
			);
		}

		#endregion // Unity Callbacks

	} // class CEnemy
} // namespace AlienProject