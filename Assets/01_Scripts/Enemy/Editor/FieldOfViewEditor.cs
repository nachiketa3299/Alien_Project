using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AlienProject
{
	[CustomEditor(typeof(CEnemyFOV))]
	public class EnemyFOVEditor : Editor
	{
		private void OnSceneGUI()
		{
			CEnemyFOV fow = (CEnemyFOV)target;

			//Range
			Handles.color = Color.magenta;
			Handles.DrawWireArc(fow.transform.position, Vector3.up, Vector3.forward, 360, fow.Radius);
			Vector3 viewAngleA = fow.DirFromAngle(-fow.Angle / 2, false);
			Vector3 viewAngleB = fow.DirFromAngle(fow.Angle / 2, false);

			Handles.DrawLine(fow.transform.position, fow.transform.position + viewAngleA * fow.Radius);
			Handles.DrawLine(fow.transform.position, fow.transform.position + viewAngleB * fow.Radius);

			//Target
			Handles.color = Color.red;
			foreach (Transform visible in fow._visibleTarget)
			{
				Handles.DrawLine(fow.transform.position, visible.transform.position);
			}
		}
	}
}