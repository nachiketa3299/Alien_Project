namespace AlienProject
{
    public struct EffectInfo
    {
        public int bleeding;
        public int sleeping;
    }

    public enum Effect
    {
        Bleeding,
        Sleeping
    }
    public interface IStatusEffectable
    {
        /// <summary>
        /// 무기측에서 상태이상을 축적정보를 쌓기위해 호출합니다.
        /// </summary>
        /// <param name="value"></param>
        public abstract void StackEffect(EffectInfo value);
        public abstract void OnEffectEvent(float percent);
    }
}