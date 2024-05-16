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
		{
		}

		#endregion // IDamageable Impelementation

		// MARK: Component Caching

		private SensorBase _sensor;
		private CEnemyUI _enemyUI;

		// MARK: Members

		private EffectInfo enemEffectInfo;
		private BleedingEffect _bleedingEffect;
		private SleepingEffect _sleepingEffect;

		// MARK: Inspector

		[SerializeField] private OEnemyData _enemyData;

		// MARK: Properties

		public EEnemyState State { get; private set; }

		[SerializeField]
		private float _attackStateStartRange = 10f;

		// MARK: Properties

		#region Unity Callbacks

		protected override void Awake()
		{
			base.Awake();

			_sensor = GetComponent<SensorBase>();

			_enemyUI = GetComponent<CEnemyUI>();

			if (!_enemyUI)
			{
				return;
			}

			_enemyUI.Init(3, 3);

			InitEffectManager();
		}

		protected override void Start()
		{
			base.Start();

			{  // SensorBase 초기화
				_sensor = GetComponent<SensorBase>();

				_sensor.TrackingTargetEmpty.AddListener(OnTrackingTargetEmpty);
				_sensor.TrackingTargetFound.AddListener(OnTrackingTargetFound);
			}

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

		#region 상태이상

		private void InitEffectManager()
		{
			enemEffectInfo = new EffectInfo();

			//출혈
			_bleedingEffect = new BleedingEffect();
			_bleedingEffect.InitEffect(3, 20);
			_bleedingEffect.bleedEvent += OnEffectEvent;

			//수면
			_sleepingEffect = new SleepingEffect();
			_sleepingEffect.InitEffect(3, 20);
			_sleepingEffect.sleepEvent += OnSleepEffectEvent;
		}

		/// <summary>
		/// <see cref="IStatusEffectable"/> 인터페이스 추상함수 구현
		/// <list type="number">
		/// <item>무기측으로부터 상태이상 수치를 전달받아옵니다.</item>
		/// <item><see cref="BleedingEffect"/>를 통해 상태를 축적합니다.</item>
		/// <item><see cref="OnEffectEvent"/>를 통해 발현합니다</item> 
		/// </list>
		/// </summary>
		/// <param name="value"></param>
		public void StackEffect(EffectInfo value)
		{
			_bleedingEffect.CurrentValue += value.bleeding;
			_sleepingEffect.CurrentValue += value.sleeping;
			//모든 이벤트 실행
			//
			enemEffectInfo.bleeding = _bleedingEffect.CurrentValue;
			enemEffectInfo.sleeping = _sleepingEffect.CurrentValue;

			//현재 리셋되고나서 업데이트를 함
			UpdateStatusUI();
			Debug.Log("StackEffect");
		}

		/// <summary>
		/// 출혈 이벤트 <para/>
		/// 최대체력 비례 데미지
		/// </summary>
		/// <param name="percent"></param>
		public void OnEffectEvent(float percent)
		{
			_hp.ApplyDamage(_enemyData.healthData.max * percent / 100);
			_enemyUI.ResetUI(Effect.Bleeding);

			Debug.Log("<color=red>[상태이상]출혈 현재체력 : " + _enemyData.healthData.current + " </color>");
		}

		/// <summary>
		/// 수면 이벤트 <para/>
		/// 속도를 제어합니다.
		/// </summary>
		/// <param name="percent"></param>
		public void OnSleepEffectEvent(float percent)
		{
			_enemyUI.ResetUI(Effect.Sleeping);
			Debug.Log("<color=red>[상태이상]수면 현재이동속도 : " + _enemyData.healthData.current + " </color>");
		}

		/// <summary>
		/// 적의 상태이상에 따른 UI를 업데이트 합니다
		/// </summary>
		public void UpdateStatusUI()
		{
			_enemyUI.UpdateUI(_enemyData.healthData.current, enemEffectInfo);
		}

		#endregion // 상태이상

	} // class CEnemy
} // namespace AlienProject