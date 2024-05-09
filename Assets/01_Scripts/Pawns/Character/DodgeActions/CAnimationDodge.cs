// 2024-05-01 RZN

using UnityEngine;

namespace AlienProject
{
	/// <summary>
	/// 회피 동작을 실행하면 플레이어가 애니메이션을 재생하는 방식으로 회피 동작을 수행하는 컴포넌트 입니다.
	/// </summary>
	[AddComponentMenu("Alien Project/Character/Dodge Actions/Animation Dodge Component")]
	public class CAnimationDodge : DodgeActionBase
	{
		/// <summary>
		/// Dodge 애니메이션이 시작될 때 실행되는 이벤트로, 애니메이션 자체에 해당 이름을 가진 이벤트를 추가해 줘야 한다.
		/// </summary>
		public void DodgeAnimationBegin()
		{
			// TODO 아마도 무적 프레임 등을 여기서 구현
			// TODO 아래처럼 해야 하는데 어떻게 해야 할 지 구상중임
			// 참고로, 무적 프레임의 시작과 끝은 런타임에서 변경될 수도 있게 하고 싶은데, 어떻게 해야 하는지는 잘 모르겠음.
			// 연구가 필요함
			// [Dodge 상태의 시작] -> [Dodge 애니메이션의 시작] -> [Dodge 애니메이션 내 무적 프레임의 시작]
			// -> [Dodge 애니메이션 내 무적 프레임의 끝] -> [Dodge 애니메이션의 끝] -> [Dodge 상태의 끝]
		}

		/// <summary>
		/// Dodge 애니메이션이 끝날 때 실행되는 이벤트로, 애니메이션 자체에 해당 이름을 가진 이벤트를 추가해 줘야 한다.
		/// </summary>
		public void DodgeAnimationEnd()
		{
			DodgeActionEnd();
		}
	}
}