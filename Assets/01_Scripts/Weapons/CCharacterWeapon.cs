using UnityEngine;

namespace AlienProject
{
	/// <summary>
	/// 캐릭터의 무기 오브젝트에 부착되어 무기의 함수성을 구현하는 클래스
	/// </summary>
	[AddComponentMenu("Alien Project/Weapon/Character Weapon Component")]
	public class CCharacterWeapon : WeaponBase
	{
		// MARK: Inspector

		[SerializeField] private CWeaponSocket _socket;

		#region Unity Callbacks

		#endregion // Unity Callbacks
	} // class CWeapon
} // namespace AlienProject