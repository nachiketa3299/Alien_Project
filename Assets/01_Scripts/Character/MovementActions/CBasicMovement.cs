// 2024-04-25 RZN

using UnityEngine;

namespace AlienProject
{
	[AddComponentMenu("Alien Project/Character/Movement Actions/Basic Movement Component")]
	public class CBasicMovement : MovementActionBase
	{
		protected override void Move()
		{
			_movementVelocity = TranslatedMovementInput * _maxSpeed;

			TryClampMaxMovementVelocity();
			TryClampMaxMovementVelocity();

			_characterController.Move(_movementVelocity * Time.deltaTime);
		}
	}
}