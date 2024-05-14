using UnityEngine;

namespace AlienProject
{
	[CreateAssetMenu(fileName = "SO_NewCharacterData", menuName = "Alien Project/Data/Character Data")]
	public class OCharacterData : PawnDataBase
	{
		// NOTE 만일 레벨별로 캐릭터의 스탯을 다르게 하고 싶다면, 나름의 자료구조를 다시 만들어야 겠지만,
		// 아직 레벨이라는 개념이 없으므로, 기본적인 스탯의 실제 값을 저장하고 있는 오브젝트로 사용하겠습니다.
		// 급하게 만드는 거라서, 여기에 있는 값들중 일부는 기초 클래스 PawnData로 이동할 수도 있습니다.

		// NOTE 캐릭터를 구성하는 여러 컴포넌트들이 가지고 있어야 할 초깃값을 정의합니다.

		public ResourceData staminaData;

	} // class OCharacterData
} // namespace AlienProject