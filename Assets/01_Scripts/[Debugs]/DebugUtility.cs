// 2024-04-15 Checked by RZN

#if UNITY_EDITOR

using UnityEngine;

namespace AlienProject
{
	public class DebugUtility
	{

		/// <summary>
		/// 디버깅 목적으로 원을 그립니다.
		/// </summary>
		/// <param name="position">원의 중심의 월드 좌표</param>
		/// <param name="forwardDirection">원의 전방 벡터</param>
		/// <param name="upDirection">원의 상방 벡터</param>
		/// <param name="radius">원의 반지름</param>
		/// <param name="vertexCount">원을 구성할 정점의 갯수</param>
		/// <param name="color">원의 색상</param>
		/// <param name="duration">그려진 원의 잔류 시간</param>
		public static void DrawCircle
		(
			Vector3 position,
			Vector3 forwardDirection, Vector3 upDirection,
			float radius,
			int vertexCount,
			Color color,
			float duration
		)
		{
			var rotation = Quaternion.LookRotation(forwardDirection, upDirection);
			DrawCircle(position, rotation, radius, vertexCount, color, duration);
		}

		public static void DrawCircle
		(
			Vector3 position,
			Quaternion rotation,
			float radius,
			int vertexCount,
			Color color,
			float duration
		)
		{
			float angle = 0f;
			var lastVertex = Vector3.zero;

			for (int i = 0; i < vertexCount; ++i)
			{
				var rad = Mathf.Deg2Rad * angle;
				var thisVertex = new Vector3(Mathf.Sin(rad), Mathf.Cos(rad), 0f) * radius;

				var lineStart = position + rotation * lastVertex;
				var lineEnd = position + rotation * thisVertex;

				if (i > 0)
					Debug.DrawLine(lineStart, lineEnd, color, duration);

				lastVertex = thisVertex;
				angle += 360f / vertexCount;
			}
		}

	} // class DebugUtility
} // namespace AlienProject
#endif // UNITY_EDITOR