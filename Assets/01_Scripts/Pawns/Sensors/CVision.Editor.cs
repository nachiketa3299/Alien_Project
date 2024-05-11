#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace AlienProject
{
	public partial class CVision : SensorBase
	{
		[CustomEditor(typeof(CVision))]
		public class VisionEditor : Editor
		{
			private readonly Color _visionArcTargetEmptyColor = Color.green - new Color(0f, 0f, 0f, 0.95f);
			private readonly Color _visionArcTargetFoundColor = Color.yellow - new Color(0f, 0f, 0f, 0.95f);
			private readonly Color _visionRayTrackingColor = Color.red;
			private readonly Color _visionRayVisibleColor = Color.red - new Color(0f, 0f, 0f, 0.95f);

			private void OnSceneGUI()
			{

				var cvision = target as CVision;

				var isThereTarget = cvision._allVisibleTargets.Count > 0;

				Handles.color = isThereTarget ? _visionArcTargetFoundColor : _visionArcTargetEmptyColor;

				var arcStartQuat = Quaternion.Euler(0f, -cvision._targetSearchAngle / 2f, 0f) * cvision._eyeTransform.forward;

				Handles.DrawSolidArc
				(
					center: cvision._eyeTransform.position,
					normal: cvision._eyeTransform.up,
					from: arcStartQuat,
					angle: cvision._targetSearchAngle,
					radius: cvision._targetSearchRadius
				);

				foreach (var target in cvision._allVisibleTargets)
				{
					var rayColor = cvision._trackingTarget == target ? _visionRayTrackingColor : _visionRayVisibleColor;

					Handles.color = rayColor;
					Handles.DrawLine(cvision._eyeTransform.position, target.transform.position);
				}
			}
		} // class VisionEditor
	} // class CVision
} // namespace AlienProject
#endif // UNITY_EDITOR