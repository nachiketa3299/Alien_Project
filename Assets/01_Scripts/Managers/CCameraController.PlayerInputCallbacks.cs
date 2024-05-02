// 2024-04-10 Checked by RZN
// 이 콜백 이벤트가 PlayerCharacter 오브젝트의 PlayerInput 컴포넌트에서 관리되어야 하는지, Managers 오브젝트의 PlayerInput 컴포넌트에서 관리되어야 하는지 잘 모르겠습니다.

using UnityEngine.InputSystem;

namespace AlienProject
{
	public partial class CCameraController : Singleton<CCameraController>
	{
		public void OnRotateVirtualCamera(InputAction.CallbackContext context)
		{
			if (context.started)
			{
				if (_mainCameraBrain.IsBlending)
				{
					return;
				}

				var inFlipDirection = context.ReadValue<float>();
				var flipCoeff = _flipRotateDirection ? -1 : 1;

				if (flipCoeff * inFlipDirection < 0)
				{
					ChangeToNextVirtualCamera();
				}
				else if (flipCoeff * inFlipDirection > 0)
				{
					ChangeToPreviousVirtualCamera();
				}
			}
		}
	}
} // AlienProject