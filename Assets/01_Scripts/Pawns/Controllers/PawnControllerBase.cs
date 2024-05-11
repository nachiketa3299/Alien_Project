using UnityEngine;

namespace AlienProject
{
	[DisallowMultipleComponent]
	[Icon("Assets/Editor/Icons/PawnControllerBase.png")]
	public abstract class PawnControllerBase : MonoBehaviour
	{
		// MARK: Component Caching

		protected PawnBase _possesedPawn = null;

		#region Unity Callbacks
		protected virtual void Awake()
		{
			_possesedPawn = GetComponent<PawnBase>();

			if (_possesedPawn)
			{
				Debug.Log($"[PawnControllerBase]: 폰 {_possesedPawn}에 빙의 완료");
			}
			else
			{
				Debug.LogWarning($"[PawnControllerBase]: 빙의할 폰을 찾을 수 없습니다.");
			}
		}

		protected virtual void Start()
		{
		}

		#endregion // Unity Callbacks

		// Below means "Desired to Move"

		/// <summary>
		/// 이 폰 컨트롤러가 조종 중인 폰을 주어진 월드 방향으로 이동시킵니다.
		/// </summary>
		/// <param name="movementWorldDirection">폰의 정규화된 월드 기준 이동 방향</param>
		public void MoveTowardWorldDirection(Vector3 worldDirection)
		{
			_possesedPawn.MoveTowardWorldDirection(worldDirection);
		}

		/// <summary>
		/// 이 폰 컨트롤러가 조종 중인 폰을 주어진 월드 좌표로 이동시킵니다.
		/// </summary>
		/// <param name="worldPosition">폰의 월드 기준 이동 도착점</param>
		public void MoveToWorldPosition(Vector3 worldPosition)
		{
			_possesedPawn.MoveToWorldPosition(worldPosition);
		}

	} // class PawnControllerBase
} // namespace AlienProject