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

        // MARK: Members
        private BleedingEffect _bleedingEffect;

        // MARK: Inspector

        [SerializeField] private OEnemyData _enemyData;

        // MARK: Properties

        public EEnemyState State { get; private set; }

        #region Unity Callbacks

        protected override void Awake()
        {
            base.Awake();

            _sensor = GetComponent<SensorBase>();
            _bleedingEffect = new BleedingEffect();
            _bleedingEffect.InitEffect(2, 20);
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


        /// <summary>
        /// <see cref="IStatusEffectable"/> 인터페이스 추상함수 구현
        /// <list type="number">
        /// <item>무기측으로부터 상태이상 수치를 전달받아옵니다.</item>
        /// <item><see cref="BleedingEffect"/>를 통해 상태를 축적합니다.</item>
        /// <item><see cref="OnEffectEvent"/>를 통해 발현합니다</item> 
        /// </list>
        /// </summary>
        /// <param name="value"></param>
        public void StackEffect(AttackEffectInfo value)
        {
            _bleedingEffect.CurrentValue += value.bleeding;
            _bleedingEffect.bleedEvent += OnEffectEvent;
        }

        /// <summary>
        /// 출혈 이벤트
        /// </summary>
        /// <param name="percent"></param>
        public void OnEffectEvent(float percent)
        {
            _enemyData.healthData.current -= _enemyData.healthData.max * percent / 100;
            Debug.Log("<color=red>[상태이상 발현] 현재체력 : " + _enemyData.healthData.current + " </color>");
        }
    } // class CEnemy
} // namespace AlienProject