
using UnityEngine;
using UnityEngine.Events;

namespace AlienProject
{
	[AddComponentMenu("Alien Project/Enemy Component")]
	[RequireComponent(typeof(SensorBase))]
	public class CEnemy : PawnBase
	{
		#region IDamageable Impelementation

		public override void TakeDamage(float damageAmount)
		{ }

		#endregion // IDamageable Impelementation

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

		// MARK: Inspector

		[SerializeField]
		private OEnemyData _enemyData;

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

					Debug.Log($"[CEnemy] {_sensor}이 {tr.name}을 감지했습니다.");
					OnTargetDetected?.Invoke(tr);
				}
			);

			_sensor.OnTrackingTargetLost.AddListener
			(
				(tr) =>
				{
					State = EEnemyState.Patrol;

					Debug.Log($"[CEnemy] {_sensor}이 {tr.name}을 잃었습니다.");
					OnTargetLost?.Invoke(tr);
				}
			);

			_sensor.OnTrackingTargetEmpty.AddListener
			(
				() =>
				{
					State = EEnemyState.Idle;

					Debug.Log($"[CEnemy] {_sensor}가 아무것도 감지하지 못했습니다.");
					OnTargetEmpty?.Invoke(null);
				}
			);

			if (_shouldInitalizeWithPawnData)
			{
				foreach (var initializable in GetComponents<IInitializable>())
				{
					initializable.Initialize(_enemyData);
				}
			}
		}

		#endregion // Unity Callbacks

	} // class CEnemy
} // namespace AlienProject