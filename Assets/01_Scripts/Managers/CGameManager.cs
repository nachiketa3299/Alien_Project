// 2024-04-10 Checked by RZN
// 현재는 딱히 하는게 없습니다.

using UnityEngine;

namespace AlienProject
{
	/// <summary>
	/// <para>게임 전반을 관리하는 싱글턴 매니저 클래스</para>
	/// </summary>
	[AddComponentMenu("Alien Project/Managers/Game Manager")]
	public class CGameManager : Singleton<CGameManager>
	{
		[SerializeField]
		private string _playerTag = "Player";

		private GameObject _inScenePlayer;
		public GameObject InScenePlayer => _inScenePlayer;

		private void Awake()
		{
			_inScenePlayer = GameObject.FindWithTag(_playerTag);

#if UNITY_EDITOR
			if (!_inScenePlayer)
			{
				Debug.LogError($"[CGameManager]: 씬에 {_playerTag} 태그를 가진 오브젝트가 존재하지 않습니다.");
			}
			else
			{
				Debug.Log($"[CGameManager]: 플레이어 {_inScenePlayer} 확인 완료");
			}
#endif
		}
	}
} // AlienProject
