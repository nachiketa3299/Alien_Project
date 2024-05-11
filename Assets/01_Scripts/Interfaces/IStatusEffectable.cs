namespace AlienProject
{
    public struct AttackEffectInfo
    {
        public float bleeding;
        public float sleeping;
    }
    public interface IStatusEffectable
    {
        public abstract void StackEffect(AttackEffectInfo value);
    }
}