// 2024-04-25 RZN

using UnityEditor;
using UnityEngine;

namespace AlienProject
{
	public partial class CMovementAction : MonoBehaviour
	{

		[Header("디버그 설정")]

		public bool EnableDraw = true;
		public float DrawScale = 1f;

		public float PositionTrailDrawDuration = 5f;
		public float RayDrawDuration = 0f;
		private readonly float OffsetY = 2f;
		protected readonly float OffsetRadius = 1f;

		protected float RawInputLength => 0.3f * DrawScale;
		protected float TranslatedInputLength => RawInputLength * 1.3f;
		private Color RawInputColor = Color.red;
		private Color TranslatedInputColor = Color.magenta;

		protected float VelocityLength => 2f * DrawScale;
		protected Color VelocityColor => Color.Lerp(Color.red, Color.cyan, MovementSpeedRatio);
		protected float PlayerForwardLength => 1f * DrawScale;
		protected Color PlayerForwardColor = Color.yellow;

		private float AccelerationLength => 1f * DrawScale;
		private Color AccelerationColor = Color.blue;

		private float DecelerationLength => AccelerationLength;
		private Color DecelerationColor = Color.magenta;

		private Vector3 _previousPosition;
		protected Color _positionTrailColor = Color.green;

		#region Unity Callbacks

		private partial void Start()
		{
			_previousPosition = transform.position;
		}

		private partial void OnDrawGizmos()
		{
			if (!EnableDraw)
			{
				return;
			}

			if (!Application.isPlaying)
			{
				return;
			}

			var characterRoot = transform.position;
			var offset = transform.up * OffsetY;

			var cameraStart = Camera.main.transform.position;
			var cameraEnd = characterRoot + offset;

			Debug.DrawLine(cameraStart, cameraEnd, Color.blue);

			if (IsPlayerDesiredToMove)
			{
				var rawInputTarget = _rawMovementInput * RawInputLength;
				var rawInputPivot = characterRoot + offset;

				var translatedInputTarget = TranslatedMovementInput * TranslatedInputLength;
				var translatedInputPivot = rawInputPivot;

				Debug.DrawRay(rawInputPivot, rawInputTarget, RawInputColor);
				Debug.DrawRay(translatedInputPivot, translatedInputTarget, TranslatedInputColor);
			}

			var playerForwardPivot = characterRoot + offset;
			var playerForwardTarget = transform.forward * PlayerForwardLength;

			Debug.DrawRay(playerForwardPivot, playerForwardTarget, PlayerForwardColor);

			if (IsMoving)
			{
				var velocityTarget = MovementSpeedRatio * VelocityLength * _movementVelocity.normalized;
				var velocityPivot = characterRoot + velocityTarget.normalized * OffsetRadius;

				Debug.DrawRay(velocityPivot, velocityTarget, VelocityColor, RayDrawDuration);
			}

			var currentPosition = transform.position;
			Debug.DrawLine(_previousPosition, currentPosition, _positionTrailColor, PositionTrailDrawDuration);
			_previousPosition = currentPosition;

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

		#endregion // Unity Callbacks

	} // class CMovementAction
} // namespace AlienProject