using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace AlienProject
{
	public abstract class SensorBase : MonoBehaviour
	{
		// MARK: Events

		/// <summary>
		/// 추적할 타겟을 검색하였는데, 존재하지 않을 때에 호출되는 이벤트입니다.
		/// </summary>
		[HideInInspector] public UnityEvent TrackingTargetEmpty;

		/// <summary>
		/// 추적할 타겟을 감지했을 때에 호출되는 이벤트입니다.
		/// 추적한 타겟이 여러 개일 경우, 각 타겟마다 호출되므로 주의하십시오.
		/// </summary>
		[HideInInspector] public UnityEvent<GameObject> TrackingTargetFound;


		// MARK: Inspector

		[SerializeField, Tooltip("이 감각기가 최종적으로 인식할 오브젝트의 레이어 마스크입니다.")]
		protected LayerMask _trackingTargetLayerMask;

		// MARK: Members

		protected List<GameObject> _trackingTargets = new();

		// MARK: Properties

		public List<GameObject> TrackingTargets => _trackingTargets;
		public LayerMask TrackingTargetLayerMask => _trackingTargetLayerMask;

		#region Unity Callbacks

		protected abstract void Awake();
		protected abstract void Start();

		#endregion
	}

} // namespace AlienProject