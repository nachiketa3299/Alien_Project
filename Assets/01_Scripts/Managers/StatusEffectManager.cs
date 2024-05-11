using UnityEngine.InputSystem.Android;

namespace AlienProject
{
    /// <summary>
    /// 퍼센트 에이지 (체력비래)
    /// </summary>
    public delegate void EffectDelegate(float percent);

    public class StatusEffectManager
    {
        private float currentValue;
        public float maxValue;

        /// <summary>
        /// 상태축적
        /// </summary>
        public float CurrentValue
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
        }
    }

    public class BleedingEffect : StatusEffectManager
    {
        /// <summary>
        /// 출현 발현 이벤트
        /// </summary>
        public event EffectDelegate bleedEvent;
        public float percent;
        public void InitEffect(float maxValue,float percent)
        {
            //this.CurrentValue = value;
            this.maxValue = maxValue;
            this.percent = percent;
        }

        public override void OnEffect()
        {
            base.OnEffect();
            bleedEvent?.Invoke(percent);
            //CalculateBleeding();
        }

        public float CalculateBleeding(float maxHP,float currentHP)
        {
            currentHP -= maxHP / 10;
            return currentHP;
        }
    }

    public class SleepingEffect : StatusEffectManager
    {
        public event EffectDelegate bleedEvent;

        public float CalculateBleeding()
        {
            float a = 0;
            return a;
        }
    }
}