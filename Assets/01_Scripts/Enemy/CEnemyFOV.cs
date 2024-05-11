using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace AlienProject
{
	public delegate void FindTargetDelegate(Transform tr);

	[AddComponentMenu("Alien Project/Enemy/Enemy Field Of View Component")]
	public class CEnemyFOV : MonoBehaviour
	{
		public event FindTargetDelegate FindTargetEvent;
		public event FindTargetDelegate TargetEnmptyEvent;

		[FormerlySerializedAs("_Radius")]
		[SerializeField, Range(0, 30)]
		private float _radius;

		[SerializeField, Range(0, 360)] private float _angle;

		public LayerMask _targetMask, _obstacleMask;
		public List<Transform> _visibleTarget = new List<Transform>();

		[SerializeField] private float _delay = 0.2f;

		#region DebugFOVMesh

		public float meshResolution;
		Mesh viewMesh;
		public MeshFilter viewMeshFilter;

		public struct ViewCastInfo
		{
			public bool hit;
			public Vector3 point;
			public float dst;
			public float angle;

			public ViewCastInfo(bool _hit, Vector3 _point, float _dst, float _angle)
			{
				hit = _hit;
				point = _point;
				dst = _dst;
				angle = _angle;
			}
		}

		ViewCastInfo ViewCast(float globalAngle)
		{
			Vector3 dir = DirFromAngle(globalAngle, true);
			RaycastHit hit;
			if (Physics.Raycast(transform.position, dir, out hit, _radius, _obstacleMask))
			{
				return new ViewCastInfo(true, hit.point, hit.distance, globalAngle);
			}
			else
			{
				return new ViewCastInfo(false, transform.position + dir * _radius, _radius, globalAngle);
			}
		}

		public struct Edge
		{
			public Vector3 PointA, PointB;

			public Edge(Vector3 _PointA, Vector3 _PointB)
			{
				PointA = _PointA;
				PointB = _PointB;
			}
		}

		public int edgeResolveIterations = 100;
		public float edgeDstThreshold = 100;

		Edge FindEdge(ViewCastInfo minViewCast, ViewCastInfo maxViewCast)
		{
			float minAngle = minViewCast.angle;
			float maxAngle = maxViewCast.angle;
			Vector3 minPoint = Vector3.zero;
			Vector3 maxPoint = Vector3.zero;

			for (int i = 0; i < edgeResolveIterations; i++)
			{
				float angle = minAngle + (maxAngle - minAngle) / 2;
				ViewCastInfo newViewCast = ViewCast(angle);
				bool edgeDstThresholdExceed = Mathf.Abs(minViewCast.dst - newViewCast.dst) > edgeDstThreshold;
				if (newViewCast.hit == minViewCast.hit && !edgeDstThresholdExceed)
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

			return new Edge(minPoint, maxPoint);
		}

		private void LateUpdate()
		{
			DrawFieldOfView();
		}

		/// <summary>
		/// Debug 용 
		/// </summary>
		private void DrawFieldOfView()
		{
			int stepCount = Mathf.RoundToInt(_angle * meshResolution);
			float stepAngleSize = _angle / stepCount;
			List<Vector3> viewPoints = new List<Vector3>();
			ViewCastInfo prevViewCast = new ViewCastInfo();

			for (int i = 0; i <= stepCount; i++)
			{
				float angle = transform.eulerAngles.y - _angle / 2 + stepAngleSize * i;
				ViewCastInfo newViewCast = ViewCast(angle);

				// i가 0이면 prevViewCast에 아무 값이 없어 정점 보간을 할 수 없으므로 건너뛴다.
				if (i != 0)
				{
					bool edgeDstThresholdExceed = Mathf.Abs(prevViewCast.dst - newViewCast.dst) > edgeDstThreshold;

					// 둘 중 한 raycast가 장애물을 만나지 않았거나 두 raycast가 서로 다른 장애물에 hit 된 것이라면(edgeDstThresholdExceed 여부로 계산)
					if (prevViewCast.hit != newViewCast.hit ||
						(prevViewCast.hit && newViewCast.hit && edgeDstThresholdExceed))
					{
						Edge e = FindEdge(prevViewCast, newViewCast);

						// zero가 아닌 정점을 추가함
						if (e.PointA != Vector3.zero)
						{
							viewPoints.Add(e.PointA);
						}

						if (e.PointB != Vector3.zero)
						{
							viewPoints.Add(e.PointB);
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

			viewMesh.Clear();
			viewMesh.vertices = vertices;
			viewMesh.triangles = triangles;
			viewMesh.RecalculateNormals();
		}

		#endregion // DebugFOVMesh

		#region Property

		public float Radius => _radius;

		public float Angle => _angle;

		#endregion // Property

		private void Start()
		{
			viewMesh = new Mesh();
			viewMesh.name = "View Mesh";
			viewMeshFilter.mesh = viewMesh;
			StartCoroutine(FindTarget(_delay));
		}

		/// <summary>
		/// <see cref="_delay"/> 만큼 <see cref="FindVisibleTarget"/>을 반복하는 코루틴
		/// </summary>
		/// <param name="delay"></param>
		/// <returns></returns>
		private IEnumerator FindTarget(float delay)
		{
			while (true)
			{
				FindVisibleTarget();
				yield return new WaitForSeconds(delay);
			}
		}

		private GameObject temp;

		/// <summary>
		/// 1. Sphere로 주변 콜라이더를 감지한다.
		/// 2. 지정한 각도와 거리를 통해 범위안에 들어와있는지 판단한다
		/// <see cref="_visibleTarget"/> 추가한다
		/// </summary>
		private void FindVisibleTarget()
		{
			_visibleTarget.Clear();
			Collider[] targetInRadius = Physics.OverlapSphere(transform.position, _radius, _targetMask);

			if (targetInRadius.Length == 0)
			{
				TargetEnmptyEvent(null);
				return;
			}

			for (int i = 0; i < targetInRadius.Length; i++)
			{
				Transform target = targetInRadius[i].transform;
				Vector3 dirToTarget = (target.position - transform.position).normalized;

				// 플레이어와 forward와 target이 이루는 각이 설정한 각도 내라면
				if (Vector3.Angle(transform.forward, dirToTarget) < _angle / 2)
				{
					float dstToTarget = Vector3.Distance(transform.position, target.transform.position);

					// 타겟으로 가는 레이캐스트에 obstacleMask가 걸리지 않으면 visibleTargets에 Add
					if (!Physics.Raycast(transform.position, dirToTarget, dstToTarget, _obstacleMask))
					{
						_visibleTarget.Add(target);
						FindTargetEvent(target);
					}
				}
			}
		}

		public Vector3 DirFromAngle(float angleDegrees, bool angleIsGlobal)
		{
			if (!angleIsGlobal)
			{
				angleDegrees += transform.eulerAngles.y;
			}

			return new Vector3(Mathf.Cos((-angleDegrees + 90) * Mathf.Deg2Rad), 0,
				Mathf.Sin((-angleDegrees + 90) * Mathf.Deg2Rad));
		}
	}
}