using UnityEngine;
using UnityEngine.Events;

namespace AlienProject
{
    [AddComponentMenu("Alien Project/Enemy Component")]
    [RequireComponent(typeof(SensorBase))]
    public class CEnemy : PawnBase, IStatusEffectable
    {
        #region IDamageable Impelementation

        public override void TakeDamage(float damageAmount)
        {
        }

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
        private CEnemyUI _enemyUI;

        // MARK: Members
        private EffectInfo enemEffectInfo;
        private BleedingEffect _bleedingEffect;
        private SleepingEffect _sleepingEffect;


        // MARK: Inspector

        [SerializeField] private OEnemyData _enemyData;

        // MARK: Properties

        public EEnemyState State { get; private set; }

        #region Unity Callbacks

        protected override void Awake()
        {
            base.Awake();

            _sensor = GetComponent<SensorBase>();
            _enemyUI = GetComponent<CEnemyUI>();
            _enemyUI.Init(3, 3);
            InitEffectManager();
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
            _enemyData.healthData.current -= _enemyData.healthData.max * percent / 100;
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

        #endregion
    } // class CEnemy
} // namespace AlienProject