#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace AlienProject
{
	public partial class CAIController : PawnControllerBase
	{
		[CustomEditor(typeof(CAIController))]
		private class AIControllerEditor : Editor
		{
			private readonly Color _cornerColor = Color.blue;
			private readonly Color _pathColor = Color.cyan;
			private readonly GUIStyle _labelStyle = new();
			private CAIController _comp;

			private void OnEnable()
			{
				_comp = target as CAIController;

				_labelStyle.normal.textColor = _cornerColor;
				_labelStyle.fontStyle = FontStyle.Bold;
			}

			private void OnSceneGUI()
			{
				if (_comp._navMeshPath == null)
				{
					return;
				}

				for (var i = 0; i < _comp._navMeshPath.corners.Length; ++i)
				{
					// Draw Corner Label
					Handles.Label(_comp._navMeshPath.corners[i], i.ToString(), _labelStyle);

					// Draw Path

					if (i < _comp._navMeshPath.corners.Length - 1)
					{
						Handles.color = _pathColor - new Color(0, 0, 0, 0.7f);
						Handles.DrawDottedLine(_comp._navMeshPath.corners[i], _comp._navMeshPath.corners[i + 1], 5f);
					}
				}
			}
		} // class AIControllerEditor
	} // class CAIController
} // namespace AlienProject
#endif // UNITY_EDITOR