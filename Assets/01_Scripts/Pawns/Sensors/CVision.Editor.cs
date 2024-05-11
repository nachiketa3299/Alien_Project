#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AlienProject
{
	partial class CVision : SensorBase
	{
		[CustomEditor(typeof(CVision))]
		public class VisionEditor : Editor
		{
			private CVision _comp;

			private GUIStyle _labelStyle;

			private readonly Color _visionArcColor = Color.green - new Color(0f, 0f, 0f, 0.95f);

			private readonly Color _trackingColor = Color.red - new Color(0f, 0f, 0f, 0.95f);
			private readonly float _trackingLineThickness = 3f;

			private void OnEnable()
			{
				_comp = target as CVision;

				_labelStyle = new GUIStyle()
				{
					richText = true,
				};
			}

			private void Awake()
			{
				if (!_comp)
				{
					_comp = target as CVision;
				}
			}

			private void OnSceneGUI()
			{
				var eyeTransform = _comp._eyeObject.transform;
				var visionPivot = eyeTransform.position;

				{ // Draw Vision Solid Arc
					var arcStartDirection = Quaternion.Euler
					(
						0f, -_comp.VisionAngle / 2f, 0f
					) * eyeTransform.forward;

					Handles.color = _visionArcColor;

					Handles.DrawSolidArc
					(
						center: visionPivot,
						normal: eyeTransform.up,
						from: arcStartDirection,
						angle: _comp._visionAngle,
						radius: _comp._visionRadius
					);
				}

				{ // Draw Each Target Mask Line

					var drawedTargets = new List<GameObject>();

					{ // Draw Tracking Targets

						for (var i = 0; i < _comp._nOf_objs_V_NO_TR; ++i)
						{
							var obj = _comp._objs_V_NO_TR[i];

							var color = Color.red;
							Handles.color = color;
							Handles.DrawLine(visionPivot, GetCenterPos(obj), _trackingLineThickness);

							drawedTargets.Add(obj);
						}
					}

					{  // Draw Visible Targets
						for (var i = 0; i < _comp._nOf_objs_V_NO; ++i)
						{
							var obj = _comp._objs_V_NO[i];

							if (drawedTargets.Contains(obj))
							{
								continue;
							}

							var color = Color.yellow;

							Handles.color = color;
							Handles.DrawLine(visionPivot, GetCenterPos(obj));

							drawedTargets.Add(obj);
						}
					}
					{  // Draw Obstacle Covered Targets
						for (var i = 0; i < _comp._nOf_objs_V_O; ++i)
						{
							var obj = _comp._objs_V_O[i];

							if (drawedTargets.Contains(obj))
							{
								continue;
							}

							var obs_pos = _comp._obs_hit_poss[i];

							var vp_o_color = Color.red;
							var o_t_color = Color.yellow;

							Handles.color = vp_o_color;

							Handles.DrawLine(visionPivot, obs_pos);

							Handles.color = o_t_color;

							Handles.DrawDottedLine(obs_pos, GetCenterPos(obj), 5f);

							drawedTargets.Add(obj);
						}
					}

					{
						// In Vision Radius & Vision Angle Targets
						for (var i = 0; i < _comp._nOf_objs_V; ++i)
						{
							var obj = _comp._objs_V[i];

							if (drawedTargets.Contains(obj))
							{
								continue;
							}

							var color = Color.green;

							Handles.color = color;
							Handles.DrawDottedLine(visionPivot, GetCenterPos(obj), 5f);

							drawedTargets.Add(obj);
						}
					}

					drawedTargets.Clear();
				}
			}

			private Vector3 GetCenterPos(GameObject obj)
			{
				return GetColliderCenter(obj.GetComponent<Collider>());
			}

		} // class VisionEditor
	} // class CVision
} // namespace AlienProject
#endif // UNITY_EDITOR