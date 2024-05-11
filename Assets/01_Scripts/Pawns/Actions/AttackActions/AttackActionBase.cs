using UnityEngine;

namespace AlienProject
{
	public abstract class AttackActionBase : MonoBehaviour
	{
	} // class AttackActionBase
} // namespace AlienProject

/*

공격을 위한 액션

키 입력 -> 공격 상태 -> 공격 애니메이션이 재생되어야 한다는 판정

딱 여기까지만 처리하면 됨

실제로 데미지 판정 등을 주는 것은 이 상태에 영향받는 하위 오브젝트가 처리해야 함 (캐릭터의 칼 처럼)

이건 모든 폰들이 사용할 수 있는 공통적인 액션인 것 같음

*/