using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AlienProject
{
	public delegate void EnemyFoundTargetDelegate(Transform tr);

	/// <summary>
	/// 눈(Eye)으로 지정된 오브젝트 기준으로 시야를 검사하여 타겟(플레이어)를 찾는 컴포넌트입니다.
	/// </summary>
	[AddComponentMenu("Alien Project/Sensors/Vision Component")]
	public partial class CVision : SensorBase
	{
		// public event EnemyFoundTargetDelegate OnTargetDetected;
		// public event EnemyFoundTargetDelegate OnTargetEmpty;

		// MARK: Inspector

		[Header("시야 설정")]

		[SerializeField, Tooltip("시야의 중심이 되는 오브젝트 입니다.")]
		Transform _eyeTransform;

		[SerializeField, Range(0f, 30f)]
		private float _targetSearchRadius;

		[SerializeField, Range(0f, 360f)]
		private float _targetSearchAngle;

		[SerializeField, Range(0f, 1f)]
		private float _targetSearchDelay = 0.2f;


		[SerializeField]
		private LayerMask _targetLayerMask;

		[SerializeField]
		private LayerMask _obstacleLayerMask;

		// MARK: Members

		private List<Transform> _allVisibleTargets = new();
		private Collider[] _collidersInRadius = new Collider[10];
		private int _nOfCollidersInRadius;

		// MARK: Properties

		public List<Transform> VisibleTargets => _allVisibleTargets;
		private float TargetTrackingRadius => 1.5f * _targetSearchRadius;
		private float TargetTrackingAngle => _targetSearchAngle;

		#region Unity Callbacks

		protected override void Awake()
		{
			base.Awake();

			// Set Eye
			if (_eyeTransform)
			{
				Debug.Log($"[CEnemyVision] 눈 역할의 오브젝트를 찾았습니다. ({_eyeTransform.name}).");
			}
			else
			{
				Debug.LogWarning($"[CEnemyVision] 눈 역할의 오브젝트를 찾지 못했습니다. 인스펙터에서 할당해 주세요.");
			}
		}

		protected override void Start()
		{
			base.Start();

			StartCoroutine(SearchAllVisibleTargets(_targetSearchDelay));
		}

		#endregion // Unity Callbacks

		// MARK: Coroutines

		/// <summary>
		/// 오버랩 스피어로 감지되는 모든 타겟 중 (1) 시야각 안에 들어오고, (2) 장애물에 가리지 않는 타겟을 찾아 
		/// _visibleTargets에 추가합니다. 코루틴으로, 주어진 시간 간격 만큼 반복하여 실행됩니다.
		/// </summary>
		private IEnumerator SearchAllVisibleTargets(float delay)
		{
			while (true)
			{
				_allVisibleTargets.Clear();

				_nOfCollidersInRadius = Physics.OverlapSphereNonAlloc
				(
					position: _eyeTransform.position,
					radius: _targetSearchRadius,
					results: _collidersInRadius,
					layerMask: _targetLayerMask
				);

				if (_nOfCollidersInRadius == 0)
				{
					// NOTE OnTargetEmpty 이벤트 호출
					OnTargetEmpty?.Invoke();

					yield return new WaitForSeconds(delay);
				}

				for (var i = 0; i < _nOfCollidersInRadius; ++i)
				{
					var targetTransform = _collidersInRadius[i].transform;

					if (!IsTargetInVisionAngle(targetTransform))
					{
						continue;
					}

					if (IsThereObstacleBetween(targetTransform))
					{
						continue;
					}

					_allVisibleTargets.Add(targetTransform);
				}

				foreach (var visibleTarget in _allVisibleTargets)
				{
					if (visibleTarget.CompareTag(_trackingTargetTag))
					{
						_trackingTarget = visibleTarget;

						// NOTE OnTargetDetected 이벤트 호출
						OnTrackingTargetDetected?.Invoke(_trackingTarget);
						Debug.Log($"[CEnemyVision] FindVisibleTarget (Target Detected {_trackingTarget.name})");

						yield return new WaitForSeconds(delay);
					}
				}

				yield return new WaitForSeconds(delay);
			}
		}

		// MARK: Methods

		private bool IsTargetInVisionAngle(Transform target)
		{
			var angle = Vector3.Angle(_eyeTransform.forward, target.position - _eyeTransform.position) * 2f;

			return angle < _targetSearchAngle;
		}

		private bool IsThereObstacleBetween(Transform target)
		{
			var directionToTarget = (target.position - _eyeTransform.position).normalized;
			var distanceToTarget = Vector3.Distance(_eyeTransform.position, target.position);

			return Physics.Raycast
			(
				origin: _eyeTransform.position,
				direction: directionToTarget,
				maxDistance: distanceToTarget,
				layerMask: _obstacleLayerMask
			);
		}



	} // class CEnemyVision
} // namespace AlienProject