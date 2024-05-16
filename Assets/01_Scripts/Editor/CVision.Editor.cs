#if false
// NOTE 자꾸 오류를 일으켜서, 일단 비활성화

#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.Threading;
using UnityEditor;
using UnityEngine;

namespace AlienProject
{
	partial class CVision : SensorBase
	{
		private void OnDrawGizmoSelected()
		{
			Handles.color = Color.green - new Color(0f, 0f, 0f, 0.95f);

			Handles.DrawSolidArc
			(
				center: _eyeObject.transform.position,
				normal: _eyeObject.transform.up,
				from: Quaternion.Euler(0f, -VisionAngle / 2f, 0f) * _eyeObject.transform.forward,
				angle: VisionAngle,
				radius: VisionRadius
			);

			for (var i = 0; i < _visionDataCache.Length; ++i)
			{
				ref readonly var data = ref _visionDataCache.Data(i);

				var lineThickness = data.isTrackingTarget ? 3f : 1f;
				Handles.color = data.isTrackingTarget ? Color.red : Color.green;

				if (!data.isCoveredByObstacle)
				{
					Handles.DrawLine
					(
						 p1: _eyeObject.transform.position,
						 p2: data.target.GetComponent<Collider>().bounds.center,
						 thickness: lineThickness
					);
				}
				else
				{
					Handles.DrawLine
					(
						p1: _eyeObject.transform.position,
						p2: data.obstacleHitInfo.point
					);
					Handles.color = Color.yellow;

					Handles.DrawDottedLine
					(
						p1: data.obstacleHitInfo.point,
						p2: data.target.GetComponent<Collider>().bounds.center,
						5f
					);
				}
			}
		}

		// [CustomEditor(typeof(CVision))]
		// public class VisionEditor : Editor
		// {
		// 	private CVision _comp;

		// 	private GUIStyle _labelStyle;

		// 	private readonly Color _visionArcColor = Color.green - new Color(0f, 0f, 0f, 0.95f);

		// 	private readonly Color _trackingColor = Color.red - new Color(0f, 0f, 0f, 0.95f);
		// 	private readonly float _trackingLineThickness = 3f;

		// 	private void OnEnable()
		// 	{
		// 		_comp = target as CVision;

		// 		_labelStyle = new GUIStyle()
		// 		{
		// 			richText = true,
		// 		};
		// 	}

		// 	private void Awake()
		// 	{
		// 		if (!_comp)
		// 		{
		// 			_comp = target as CVision;
		// 		}
		// 	}

		// 	private void DrawVisionBound()
		// 	{
		// 	}


		// 	private void OnSceneGUI()
		// 	{
		// 		Handles.color = _visionArcColor;

		// 		Handles.DrawSolidArc
		// 		(
		// 			center: _comp._eyeObject.transform.position,
		// 			normal: _comp._eyeObject.transform.up,
		// 			from: Quaternion.Euler(0f, -_comp.VisionAngle / 2f, 0f) * _comp._eyeObject.transform.forward,
		// 			angle: _comp.VisionAngle,
		// 			radius: _comp.VisionRadius
		// 		);

		// 		for (var i = 0; i < _comp._visionDataCache.Length; ++i)
		// 		{
		// 			ref readonly var data = ref _comp._visionDataCache.Data(i);

		// 			var lineThickness = data.isTrackingTarget ? 3f : 1f;
		// 			Handles.color = data.isTrackingTarget ? Color.red : Color.green;

		// 			if (!data.isCoveredByObstacle)
		// 			{
		// 				Handles.DrawLine
		// 				(
		// 					 p1: _comp._eyeObject.transform.position,
		// 					 p2: data.target.GetComponent<Collider>().bounds.center,
		// 					 thickness: lineThickness
		// 				);
		// 			}
		// 			else
		// 			{
		// 				Handles.DrawLine
		// 				(
		// 					p1: _comp._eyeObject.transform.position,
		// 					p2: data.obstacleHitInfo.point
		// 				);
		// 				Handles.color = Color.yellow;

		// 				Handles.DrawDottedLine
		// 				(
		// 					p1: data.obstacleHitInfo.point,
		// 					p2: data.target.GetComponent<Collider>().bounds.center,
		// 					5f
		// 				);
		// 			}
		// 		}
		// 	}


		// 	// private void OnSceneGUIii()
		// 	// {
		// 	// 	var eyeTransform = _comp._eyeObject.transform;
		// 	// 	var visionPivot = eyeTransform.position;

		// 	// 	{ // Draw Vision Solid Arc
		// 	// 		var arcStartDirection = Quaternion.Euler
		// 	// 		(
		// 	// 			0f, -_comp.VisionAngle / 2f, 0f
		// 	// 		) * eyeTransform.forward;

		// 	// 		Handles.color = _visionArcColor;

		// 	// 		Handles.DrawSolidArc
		// 	// 		(
		// 	// 			center: visionPivot,
		// 	// 			normal: eyeTransform.up,
		// 	// 			from: arcStartDirection,
		// 	// 			angle: _comp._visionAngle,
		// 	// 			radius: _comp._visionRadius
		// 	// 		);
		// 	// 	}

		// 	// 	{ // Draw Each Target Mask Line

		// 	// 		var drawedTargets = new List<GameObject>();

		// 	// 		{ // Draw Tracking Targets

		// 	// 			for (var i = 0; i < _comp._n_s_V_no_t; ++i)
		// 	// 			{
		// 	// 				var obj = _comp._s_V_no_t[i];

		// 	// 				var color = Color.red;
		// 	// 				Handles.color = color;
		// 	// 				Handles.DrawLine(visionPivot, GetCenterPos(obj), _trackingLineThickness);

		// 	// 				drawedTargets.Add(obj);
		// 	// 			}
		// 	// 		}

		// 	// 		{  // Draw Visible Targets
		// 	// 			for (var i = 0; i < _comp._n_s_V_no; ++i)
		// 	// 			{
		// 	// 				var obj = _comp._s_V_no[i];

		// 	// 				if (drawedTargets.Contains(obj))
		// 	// 				{
		// 	// 					continue;
		// 	// 				}

		// 	// 				var color = Color.yellow;

		// 	// 				Handles.color = color;
		// 	// 				Handles.DrawLine(visionPivot, GetCenterPos(obj));

		// 	// 				drawedTargets.Add(obj);
		// 	// 			}
		// 	// 		}
		// 	// 		{  // Draw Obstacle Covered Targets
		// 	// 			for (var i = 0; i < _comp._n_s_V_o; ++i)
		// 	// 			{
		// 	// 				var obj = _comp._s_V_o[i];

		// 	// 				if (drawedTargets.Contains(obj))
		// 	// 				{
		// 	// 					continue;
		// 	// 				}

		// 	// 				var obs_pos = _comp._obstacle_hit_position[i];

		// 	// 				var vp_o_color = Color.red;
		// 	// 				var o_t_color = Color.yellow;

		// 	// 				Handles.color = vp_o_color;

		// 	// 				Handles.DrawLine(visionPivot, obs_pos);

		// 	// 				Handles.color = o_t_color;

		// 	// 				Handles.DrawDottedLine(obs_pos, GetCenterPos(obj), 5f);

		// 	// 				drawedTargets.Add(obj);
		// 	// 			}
		// 	// 		}

		// 	// 		{
		// 	// 			// In Vision Radius & Vision Angle Targets
		// 	// 			for (var i = 0; i < _comp._n_s_V; ++i)
		// 	// 			{
		// 	// 				var obj = _comp._s_V[i];

		// 	// 				if (drawedTargets.Contains(obj))
		// 	// 				{
		// 	// 					continue;
		// 	// 				}

		// 	// 				var color = Color.green;

		// 	// 				Handles.color = color;
		// 	// 				Handles.DrawDottedLine(visionPivot, GetCenterPos(obj), 5f);

		// 	// 				drawedTargets.Add(obj);
		// 	// 			}
		// 	// 		}

		// 	// 		drawedTargets.Clear();
		// 	// 	}
		// 	// }

		// 	// private Vector3 GetCenterPos(GameObject obj)
		// 	// {
		// 	// 	return GetColliderCenter(obj.GetComponent<Collider>());
		// 	// }

		// } // class VisionEditor
	} // class CVision
} // namespace AlienProject
#endif // UNITY_EDITOR
#endif // false