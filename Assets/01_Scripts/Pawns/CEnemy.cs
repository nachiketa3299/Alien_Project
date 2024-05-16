
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.PlayerLoop;

namespace AlienProject
{
	[AddComponentMenu("Alien Project/Enemy Component")]
	[RequireComponent(typeof(SensorBase))]
	public class CEnemy : PawnBase
	{
		public enum EEnemyState
		{
			Idle,
			Patroling,
			Chasing,
			Attacking,
		}

		// MARK: Events

		[HideInInspector] public UnityEvent ToIdle;
		[HideInInspector] public UnityEvent ToPatroling;
		[HideInInspector] public UnityEvent<GameObject> ToChasing;
		[HideInInspector] public UnityEvent ToAttacking;

		#region IDamageable Impelementation

		public override void TakeDamage(float damageAmount)
		{ }

		#endregion // IDamageable Impelementation

		// MARK: Component Caching

		private SensorBase _sensor;

		// MARK: Members

		// MARK: Inspector

		[SerializeField]
		private OEnemyData _enemyData;

		[SerializeField]
		private float _attackStateStartRange = 10f;

		// MARK: Properties

		public EEnemyState State { get; private set; }

		#region Unity Callbacks

		protected override void Awake()
		{
			base.Awake();

			{  // SensorBase 초기화
				_sensor = GetComponent<SensorBase>();

				_sensor.TrackingTargetEmpty.AddListener(OnTrackingTargetEmpty);
				_sensor.TrackingTargetFound.AddListener(OnTrackingTargetFound);
			}
		}

		protected override void Start()
		{
			base.Start();

			if (_shouldInitalizeWithPawnData)
			{
				foreach (var initializable in GetComponents<IInitializable>())
				{
					initializable.Initialize(_enemyData);
				}
			}
		}

		protected void Update()
		{
			GetComponent<Animator>().SetBool("IsMoving", _movementAction.IsMoving);
		}

		#endregion // Unity Callbacks

		// MARK: Callbacks for SensorBase Event

		private void OnTrackingTargetEmpty()
		{
			// NOTE 이 폰이 "추적 대상을 찾을 수 없을 때"의 로직을 여기에 작성
			// SensorBase의 딜레이마다, 추적대상이 없는 경우 호출됨

			Debug.Log($"[CEnemy]: {_sensor}가 추적 대상을 찾을 수 없습니다.");

			State = EEnemyState.Patroling;

			ToPatroling?.Invoke();
		}

		private void OnTrackingTargetFound(GameObject target)
		{
			// NOTE 이 폰이 "추적 대상을 발견한 순간"의 로직을 여기에 작성

			Debug.Log($"[CEnemy]: {_sensor}가 {target}을 발견했습니다.");

			State = EEnemyState.Chasing;

			ToChasing?.Invoke(target);
		}


	} // class CEnemy
} // namespace AlienProject