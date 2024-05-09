#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace AlienProject
{
	public partial class CAccelerationMovement : MovementActionBase
	{
		[CustomEditor(typeof(CAccelerationMovement))]
		private class AccelerationMovementEditor : Editor
		{
			private readonly float _drawScale = 1f;
			private readonly float _offsetRadius = 1f;

			private float VelocityLength => 2f * _drawScale;
			private Color VelocityColor(float speedRatio)
			{
				var lerped = Color.Lerp(Color.red, Color.cyan, speedRatio);
				lerped.a = 1f;
				return lerped;
			}

			private float AccelerationLength => 1f * _drawScale;
			private Color AccelerationColor = Color.blue;

			private float DecelerationLength => AccelerationLength;
			private Color DecelerationColor = Color.magenta;

			private float CharacterForwardLength => 1f * _drawScale;
			private Color CharacterForwardColor = Color.yellow;

			private void OnSceneGUI()
			{
				if (!Application.isPlaying)
				{
					return;
				}

				var comp = target as CAccelerationMovement;
				var rootTransform = comp.gameObject.transform;
				var root = rootTransform.position;

				{ // Rotation Disc
					Handles.color = CharacterForwardColor;
					//Handles.DrawWireArc(root, rootTransform.up, rootTransform.forward, 360f, _offsetRadius * 0.9f, 2f);

					var target = rootTransform.forward * CharacterForwardLength;
					var pivot = root;
					Handles.DrawLine(pivot, pivot + target, 5f);

					if (comp.NeedToRotate)
					{
						Handles.color = Color.yellow - new Color(0f, 0f, 0f, 0.7f);
						Handles.DrawSolidArc(root, rootTransform.up, rootTransform.forward, Vector3.SignedAngle(rootTransform.forward, comp._accelerationDirection, rootTransform.up), _offsetRadius * 0.9f);
					}
				}

				{ // Movement Disc
					Color movDiscColor;

					if (comp.IsMoving)
					{
						movDiscColor = comp.IsDesiredToMove ? AccelerationColor : DecelerationColor;
					}
					else
					{
						movDiscColor = CharacterForwardColor;
					}

					Handles.color = movDiscColor;
					Handles.DrawWireArc(root, rootTransform.up, rootTransform.forward, 360f, _offsetRadius, 2f);
				}


				if (comp.IsMoving)
				{
					{ // Velocity
						Handles.color = VelocityColor(comp.MovementSpeedRatio);

						var target = comp.MovementSpeedRatio * VelocityLength * comp.MovementDirection;
						var pivot = root + target.normalized * _offsetRadius;

						Handles.DrawLine(pivot, pivot + target, 10f);
						Handles.Label(pivot + target, $"Speed: {comp.MovementSpeed}");
					}
				}

				if (comp.IsDesiredToMove)
				{
					{ // Acceleration
						Handles.color = AccelerationColor;

						var target = comp._accelerationDirection * AccelerationLength;
						var pivot = root + comp._accelerationDirection * _offsetRadius;

						Handles.DrawLine(pivot, pivot + target, 5f);
						Handles.Label(pivot + target, $"Acc: {comp._accelerationMagnitude}");
					}
				}
				else
				{
					{ // Deceleration
						Handles.color = DecelerationColor;

						var target = comp._decelerationDirection * DecelerationLength;
						var pivot = root + comp._decelerationDirection * _offsetRadius;

						Handles.DrawLine(pivot, pivot + target, 5f);
						Handles.Label(pivot + target, $"Dec: {comp._decelerationMagnitude}");
					}
				}
			}
		} // class CAccelerationMovementEditor
	} // class CAccelerationMovement
} // namespace AlienProject
#endif // UNITY_EDITOR