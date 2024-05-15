using UnityEngine.InputSystem.Android;

namespace AlienProject
{
    /// <summary>
    /// 퍼센트 에이지 (체력비래)
    /// </summary>
    public delegate void EffectDelegate(float percent);

    public delegate void StackDelegate();

    public class StatusEffectManager
    {
        /// <summary>
        /// 상태이상을 축적할때마다 호출합니다.<example>UI 업데이트</example>
        /// </summary>
        public event StackDelegate stackEvent;

        private int currentValue;
        public int maxValue;
        public float percent;

        /// <summary>
        /// 상태이상을 축적합니다.
        /// </summary>
        public int CurrentValue
        {
            get { return currentValue; }
            set
            {
                currentValue = value;
                if (currentValue >= maxValue)
                {
                    OnEffect();
                }
            }
        }

        /// <summary>
        /// 상태이상 발현
        /// </summary>
        public virtual void OnEffect()
        {
            stackEvent?.Invoke();
            currentValue = 0;
        }

        /// <summary>
        /// 상태이상을 적용하는 적에 대해 초기화합니다.
        /// </summary>
        /// <param name="maxValue">발현 수치</param>
        /// <param name="percent"></param>
        public void InitEffect(int maxValue, float percent)
        {
            //this.CurrentValue = value;
            this.maxValue = maxValue;
            this.percent = percent;
        }
    }

    public class BleedingEffect : StatusEffectManager
    {
        /// <summary>
        /// 출현 발현 이벤트
        /// </summary>
        public event EffectDelegate bleedEvent;
        
        /// <summary>
        /// 상태이상을 적용하는 적에 대해 초기화합니다.
        /// </summary>
        /// <param name="maxValue">발현 수치</param>
        /// <param name="percent"></param>
        public void InitEffect(int maxValue, float percent)
        {
            //this.CurrentValue = value;
            this.maxValue = maxValue;
            this.percent = percent;
        }

        /// <summary>
        /// 상태이상을 발현합니다.<para/>
        /// <see cref="bleedEvent"/> 이벤트를 할당하세요
        /// </summary>
        public override void OnEffect()
        {
            bleedEvent?.Invoke(percent);
            base.OnEffect();
            //CalculateBleeding();
        }

        public float CalculateBleeding(float maxHP, float currentHP)
        {
            currentHP -= maxHP / 10;
            return currentHP;
        }
    }

    public class SleepingEffect : StatusEffectManager
    {
        /// <summary>
        /// 수면 발현 이벤트
        /// </summary>
        public event EffectDelegate sleepEvent;

        /// <summary>
        /// 상태이상을 발현합니다.<para/>
        /// <see cref="bleedEvent"/>이벤트를 할당하세요
        /// </summary>
        public override void OnEffect()
        {
            sleepEvent?.Invoke(percent);
            base.OnEffect();
        }

        public void SleepEffect()
        {
        }
    }
}