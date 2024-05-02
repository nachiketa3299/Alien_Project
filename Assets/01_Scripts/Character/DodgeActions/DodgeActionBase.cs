// 2024-04-30 by RZN

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace AlienProject
{
	/// <summary>
	/// 캐릭터의 회피 동작을 구현하는 추상 클래스입니다.
	/// </summary>
	[RequireComponent(typeof(PlayerInput))]
	public abstract class DodgeActionBase : MonoBehaviour
	{
		/// <summary>
		/// 회피 동작이 시작될 때 호출되는 이벤트입니다.
		/// </summary>
		[HideInInspector]
		public UnityEvent OnDodgeActionBegin;

		/// <summary>
		/// 회피 동작이 끝날 때 호출되는 이벤트입니다.
		/// </summary>
		[HideInInspector]
		public UnityEvent OnDodgeActionEnd;

		[HideInInspector]
		public bool IsAnimationDodge => this is CAnimationDodge;

		// MARK: Inspector

		[Header("회피 동작 설정")]

		public bool AllowMovementActionWhileDodging = false;

		// MARK: Properties

		// NOTE 타 클래스에서는 Dodge 여부를 읽기만 할 수 있도록 프로퍼티로 제공
		public bool IsDodging => _isDodging;
		protected bool _isDodging = false;

		#region Unity Callbacks

		public void OnInput_DodgeAction(InputAction.CallbackContext context)
		{
			if (context.performed)
			{
				if (!IsDodging)
				{
					DodgeActionBegin();
				}
			}
		}

		#endregion // Unity Callbacks

		/// <summary>
		/// 회피 동작의 시작은 플레이어의 입력으로 시작됩니다.
		/// </summary>
		public virtual void DodgeActionBegin()
		{
			_isDodging = true;
			OnDodgeActionBegin.Invoke();
		}

		/// <summary>
		/// <para>이 함수는 회피 동작의 종류에 따라서 콜백되는 시점이 다릅니다.</para>
		/// <para>애니메이션 회피 동작의 경우, 애니메이션 종료 이벤트에 의해 콜백됩니다.</para>
		/// <para>버프형 회피 동작의 경우, 버프 지속 시간이 끝나면 콜백됩니다.</para>
		/// </summary>
		public virtual void DodgeActionEnd()
		{
			_isDodging = false;
			OnDodgeActionEnd.Invoke();
		}

	} // class DodgeActionBase
} // namespace AlienProject