// 2024-04-25 RZN

#if UNITY_EDITOR

using UnityEngine;

namespace AlienProject
{
	public abstract partial class MovementActionBase : MonoBehaviour
	{
		[Header("디버그 설정")]
		public bool EnableDraw = true;
		public float PositionTrailDrawDuration = 5f;
		public float RayDrawDuration = 0f;
		private readonly float OffsetY = 2f;
		protected readonly float DrawScale = 1f;
		protected readonly float OffsetRadius = 1f;

		protected float RawInputLength => 0.3f * DrawScale;
		protected float TranslatedInputLength => RawInputLength * 1.3f;
		private Color RawInputColor = Color.red;
		private Color TranslatedInputColor = Color.magenta;

		protected float VelocityLength => 2f * DrawScale;
		protected Color VelocityColor => Color.Lerp(Color.red, Color.cyan, SpeedRatio);
		protected float PlayerForwardLength => 1f * DrawScale;
		protected Color PlayerForwardColor = Color.yellow;

		protected Vector3 _previousPosition;
		protected Color _positionTrailColor = Color.green;

		#region Unity Callbacks
		private partial void Start()
		{
			_previousPosition = transform.position;
		}

		public virtual partial void OnDrawGizmos()
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
				var velocityTarget = SpeedRatio * VelocityLength * _movementVelocity.normalized;
				var velocityPivot = characterRoot + velocityTarget.normalized * OffsetRadius;

				Debug.DrawRay(velocityPivot, velocityTarget, VelocityColor, RayDrawDuration);
			}

			var currentPosition = transform.position;
			Debug.DrawLine(_previousPosition, currentPosition, _positionTrailColor, PositionTrailDrawDuration);
			_previousPosition = currentPosition;
		}

		#endregion // Unity Callbacks
	} // class MovementActionBase
} // namespace AlienProject

#endif
