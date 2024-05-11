using UnityEngine;

namespace AlienProject
{
	[AddComponentMenu("Alien Project/Action Components/Attack Component")]
	public class CAttack : AttackActionBase
	{
		// MARK: Inspector

		[SerializeField, Tooltip("생성할 무기 프리팹입니다.")]
		private WeaponBase _weaponPrefab;

		[SerializeField, Tooltip("무기를 부착할 캐릭터 모델의 위치입니다.")]
		private CWeaponSocket _socket;

		// MARK: Member

		private WeaponBase _weapon;

		#region Unity Callbacks

		protected void Awake()
		{
			_weapon = Instantiate(_weaponPrefab, _socket.transform);
		}

		protected void Start()
		{

		}

		#endregion // Unity Callbacks
	}
} // namespace AlienProject