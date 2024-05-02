// 2024-04-25 RZN

using UnityEngine;

namespace AlienProject
{
	public partial class CAccelerationMovement : MovementActionBase
	{
		private float AccelerationLength => 1f * DrawScale;
		private Color AccelerationColor = Color.blue;

		private float DecelerationLength => AccelerationLength;
		private Color DecelerationColor = Color.magenta;

		public override partial void OnDrawGizmos()
		{
			base.OnDrawGizmos();

			if (!EnableDraw)
			{
				return;
			}

			if (!Application.isPlaying)
			{
				return;
			}

			var characterRoot = transform.position;

			if (IsPlayerDesiredToMove)
			{
				var accelerationTarget = _accelerationDirection * AccelerationLength;
				var accelerationPivot = characterRoot + _accelerationDirection * OffsetRadius;
				Debug.DrawRay(accelerationPivot, accelerationTarget, AccelerationColor, RayDrawDuration);
			}
			else
			{
				var decelerationTarget = -_decelerationDirection * DecelerationLength;
				var decelerationPivot = characterRoot + -_decelerationDirection * OffsetRadius;
				Debug.DrawRay(decelerationPivot, decelerationTarget, DecelerationColor, RayDrawDuration);
			}
		}

	} // class CAccelerationMovement
} // namespace AlienProject