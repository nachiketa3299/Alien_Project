namespace AlienProject
{
	public interface IDamageable
	{
		/// <summary>
		/// 이 인터페이스를 구현한 개체가 전달된 양만큼 데미지를 받습니다.
		/// </summary>
		public void TakeDamage(float damageAmount);

	} // interface IDamageable
} // namespace AlienProject