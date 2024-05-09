using UnityEngine;
using UnityEngine.Events;

namespace AlienProject
{
	public abstract class SensorBase : MonoBehaviour
	{
		// MARK: Events

		/// <summary>
		/// 타겟이 검색하였는데, 타겟이 존재하지 않을 때에 호출되는 이벤트입니다.
		/// </summary>
		public UnityEvent OnTargetEmpty;

		/// <summary>
		/// 타겟을 감지했을 때에 호출되는 이벤트입니다.
		/// </summary>
		public UnityEvent<Transform> OnTrackingTargetDetected;

		/// <summary>
		/// 타겟을 추적 하다가 잃었을 때에 호출되는 이벤트입니다.
		/// </summary>
		public UnityEvent<Transform> OnTargetLost;

		// MARK: Members

		protected readonly string _trackingTargetTag = "Player";
		protected Transform _trackingTarget;

		#region Unity Callbacks

		protected virtual void Awake() { }
		protected virtual void Start() { }

		#endregion
	}

} // namespace AlienProject