using UnityEngine;

namespace AlienProject
{
	public abstract class WeaponBase : MonoBehaviour
	{
		private PawnBase _owner;

		protected virtual void Awake()
		{
			_owner = GetComponentInParent<PawnBase>();

			Debug.Log($"[WeaponBase]: 무기의 소유자 {_owner}를 확인했습니다.");
		}
	} // class WeaponBase
} // namespace AlienProject