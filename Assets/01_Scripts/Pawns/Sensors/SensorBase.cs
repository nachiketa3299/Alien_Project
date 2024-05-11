using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace AlienProject
{
	public abstract class SensorBase : MonoBehaviour
	{
		// MARK: Events

		/// <summary>
		/// 타겟을 검색하였는데, 타겟이 존재하지 않을 때에 호출되는 이벤트입니다.
		/// </summary>
		[HideInInspector] public UnityEvent OnTrackingTargetEmpty;

		/// <summary>
		/// 타겟을 감지했을 때에 호출되는 이벤트입니다.
		/// </summary>
		[HideInInspector] public UnityEvent<Transform> OnTrackingTargetDetected;

		/// <summary>
		/// 타겟을 추적 하다가 잃었을 때에 호출되는 이벤트입니다.
		/// </summary>
		[HideInInspector] public UnityEvent<Transform> OnTrackingTargetLost;

		// MARK: Inspector

		[SerializeField, Tooltip("이 감각기가 최종적으로 인식할 오브젝트의 레이어 마스크입니다.")]
		protected LayerMask _trackingTargetLayerMask;

		// MARK: Members

		protected List<GameObject> _trackingObjects = new();

		// MARK: Properties

		public List<GameObject> TrackingObjects => _trackingObjects;
		public LayerMask TrackingTargetLayerMask => _trackingTargetLayerMask;

		#region Unity Callbacks

		protected abstract void Awake();
		protected abstract void Start();

		#endregion
	}

} // namespace AlienProject