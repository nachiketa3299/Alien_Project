using UnityEngine;
using UnityEngine.InputSystem;

namespace AlienProject
{
	[RequireComponent(typeof(PlayerInput))]
	[AddComponentMenu("Alien Project/Controllers/Character Controller Component")]
	public class CCharacterController : PawnControllerBase
	{
		public Quaternion ControlRotation
		{
			get
			{
				var cameraYawRotation = Quaternion.Euler(0f, Camera.main.transform.eulerAngles.y, 0f);
				return cameraYawRotation;
			}
		}

		#region Unity Callbacks

		protected override void Awake()
		{
			base.Awake();
		}

		public void OnInput_MovementAction(InputAction.CallbackContext context)
		{
			var input2 = context.ReadValue<Vector2>();
			var input3 = new Vector3(input2.x, 0, input2.y);

			var movementWorldDirection = ControlRotation * input3;

			MoveTowardWorldDirection(movementWorldDirection);
		}

		public void OnInput_DodgeAction(InputAction.CallbackContext context)
		{
			throw new System.NotImplementedException();
		}

		public void OnInput_AttackAction_Primary(InputAction.CallbackContext context)
		{
			throw new System.NotImplementedException();
		}

		public void OnInput_AttackAction_Secondary(InputAction.CallbackContext context)
		{
			throw new System.NotImplementedException();
		}

		public void OnInput_AttackAction_RotateCamera(InputAction.CallbackContext context)
		{
			throw new System.NotImplementedException();
		}

		#endregion // Unity Callbacks
	} // class CPlayerCharacterController
} // namespace AlienProject