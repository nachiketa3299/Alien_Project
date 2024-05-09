#if false
using UnityEngine;
using System.Collections.Generic;

namespace AlienProject
{
	public partial class CEnemyVision : EnemyPerceptorBase
	{

		private float _meshResolution;
		private Mesh _viewMesh;
		private MeshFilter _viewMeshFilter;

		public struct FViewCastInfo
		{
			public bool isHit;
			public Vector3 point;
			public float destination;
			public float angle;

			public FViewCastInfo(bool _hit, Vector3 _point, float _dst, float _angle)
			{
				isHit = _hit;
				point = _point;
				destination = _dst;
				angle = _angle;
			}
		}

		private FViewCastInfo ViewCast(float globalAngle)
		{
			var direction = CalculateDirectionFromAngle(globalAngle, true);

			if (Physics.Raycast(transform.position, direction, out RaycastHit hitResult, _radius, _obstacleMask))
			{
				return new FViewCastInfo()
				{
					isHit = true,
					point = hitResult.point,
					destination = hitResult.distance,
					angle = globalAngle
				};
			}
			else
			{
				return new FViewCastInfo()
				{
					isHit = false,
					point = transform.position + direction * _radius,
					destination = _radius,
					angle = globalAngle
				};
			}
		}

		public struct FEdge
		{
			public Vector3 start;
			public Vector3 end;
		}

		public int edgeResolveIterations = 100;
		public float edgeDstThreshold = 100;

		private FEdge FindEdge(FViewCastInfo minViewCast, FViewCastInfo maxViewCast)
		{
			var minAngle = minViewCast.angle;
			var maxAngle = maxViewCast.angle;

			var minPoint = Vector3.zero;
			var maxPoint = Vector3.zero;

			for (int i = 0; i < edgeResolveIterations; i++)
			{
				var angle = minAngle + (maxAngle - minAngle) / 2;
				var newViewCast = ViewCast(angle);
				bool edgeDstThresholdExceed = Mathf.Abs(minViewCast.destination - newViewCast.destination) > edgeDstThreshold;
				if (newViewCast.isHit == minViewCast.isHit && !edgeDstThresholdExceed)
				{
					minAngle = angle;
					minPoint = newViewCast.point;
				}
				else
				{
					maxAngle = angle;
					maxPoint = newViewCast.point;
				}
			}

			return new FEdge()
			{
				start = minPoint,
				end = maxPoint
			};
		}

		private void LateUpdate()
		{
			DebugDrawEnemyVision();
		}

		private void DebugDrawEnemyVision()
		{
			var stepCount = Mathf.RoundToInt(_angle * _meshResolution);
			var stepAngleSize = _angle / stepCount;
			var viewPoints = new List<Vector3>();
			var prevViewCast = new FViewCastInfo();

			for (int i = 0; i <= stepCount; i++)
			{
				float angle = transform.eulerAngles.y - _angle / 2 + stepAngleSize * i;
				FViewCastInfo newViewCast = ViewCast(angle);

				// i가 0이면 prevViewCast에 아무 값이 없어 정점 보간을 할 수 없으므로 건너뛴다.
				if (i != 0)
				{
					bool edgeDstThresholdExceed = Mathf.Abs(prevViewCast.destination - newViewCast.destination) > edgeDstThreshold;

					// 둘 중 한 raycast가 장애물을 만나지 않았거나 두 raycast가 서로 다른 장애물에 hit 된 것이라면(edgeDstThresholdExceed 여부로 계산)
					if (prevViewCast.isHit != newViewCast.isHit ||
						(prevViewCast.isHit && newViewCast.isHit && edgeDstThresholdExceed))
					{
						FEdge e = FindEdge(prevViewCast, newViewCast);

						// zero가 아닌 정점을 추가함
						if (e.start != Vector3.zero)
						{
							viewPoints.Add(e.start);
						}

						if (e.end != Vector3.zero)
						{
							viewPoints.Add(e.end);
						}
					}
				}

				viewPoints.Add(newViewCast.point);
				prevViewCast = newViewCast;
			}

			int vertexCount = viewPoints.Count + 1;
			Vector3[] vertices = new Vector3[vertexCount];
			int[] triangles = new int[(vertexCount - 2) * 3];
			vertices[0] = Vector3.zero;
			for (int i = 0; i < vertexCount - 1; i++)
			{
				vertices[i + 1] = transform.InverseTransformPoint(viewPoints[i]);
				if (i < vertexCount - 2)
				{
					triangles[i * 3] = 0;
					triangles[i * 3 + 1] = i + 1;
					triangles[i * 3 + 2] = i + 2;
				}
			}

			_viewMesh.Clear();
			_viewMesh.vertices = vertices;
			_viewMesh.triangles = triangles;
			_viewMesh.RecalculateNormals();
		}
	} // class CEnemyVision
} // namespace AlienProject
#endif