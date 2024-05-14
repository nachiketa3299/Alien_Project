using System;
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
		private AttackEffectInfo _effectInfo;
		protected override void Awake()
		{
			base.Awake();
			_effectInfo = new AttackEffectInfo{bleeding = 1,sleeping = 0};
			
		}

		#region Unity Callbacks

		private void OnTriggerEnter(Collider other)
		{
			if(other.transform.tag.Equals("Enemy"))
			{
				Transform tr = other.transform;
				tr.GetComponent<IStatusEffectable>().StackEffect(_effectInfo);
				
				Debug.Log("<color=cyan>[공격 성공]</color>");
				
			}
		}

		#endregion // Unity Callbacks
	} // class CWeapon
} // namespace AlienProject