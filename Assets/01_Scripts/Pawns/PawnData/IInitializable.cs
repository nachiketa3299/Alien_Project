namespace AlienProject
{
	// NOTE 잘못짠거가틈 나중에 들어내야함 초기화 방법

	/// <summary>
	/// 정적인 값으로 초기화 가능한 컴포넌트들이 구현해야 하는 인터페이스입니다.<br/>
	/// data 에서 필요한 값들을 선언하고 가져와 초기화하세요
	/// </summary>
	public interface IInitializable
	{
		public void Initialize(PawnDataBase data);
	}
}