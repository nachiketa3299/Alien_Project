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

			private void OnSceneGUI()
			{
				var cAIController = target as CAIController;

				if (cAIController._navMeshPath == null)
				{
					return;
				}

				Handles.color = _cornerColor;

				foreach (var corner in cAIController._navMeshPath.corners)
				{
					Handles.DrawSolidDisc(corner, Vector3.up, 0.1f);
				}

				Handles.color = _pathColor;

				for (int i = 0; i < cAIController._navMeshPath.corners.Length - 1; i++)
				{
					Handles.DrawLine(cAIController._navMeshPath.corners[i], cAIController._navMeshPath.corners[i + 1]);
				}
			}
		} // class AIControllerEditor
	} // class CAIController
} // namespace AlienProject
#endif // UNITY_EDITOR