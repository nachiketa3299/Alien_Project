using UnityEngine;

namespace AlienProject
{
	[AddComponentMenu("Alien Project/Enemy/Default Enemy Component")]
	public class CEnemyDefault : EnemyBase
	{

		// MARK: Component Caching
		private CVision _fieldOfView;

		// MARK: Members
		private Transform _target = null;
		private EEnemyState _state = EEnemyState.Idle;

		#region Unity Callbacks

		protected override void Awake()
		{
			base.Awake();

			_fieldOfView = GetComponentInChildren<CVision>();
			// _fieldOfView.OnTrackingTargetDetected += StartFollow;
			// _fieldOfView.OnTargetEmpty += StopFollow;
		}

		private void Update()
		{
			switch (_state)
			{
				case EEnemyState.Idle:
					{
						_agent.isStopped = true;
						break;
					}
				case EEnemyState.Move:
					{
						_agent.SetDestination(_target.position);
						break;
					}
				case EEnemyState.Patrol:
					{
						break;
					}
			}
		}

		private void StartFollow(Transform target)
		{
			Debug.Log("StartFollow");

			if (this._target != target)
			{
				_agent.isStopped = false;
				_state = EEnemyState.Move;
				this._target = target;
			}
		}

		#endregion // Unity Callbacks

		private void StopFollow(Transform tr)
		{
			Debug.Log("StopFollow");

			_agent.isStopped = true;
			_state = EEnemyState.Idle;
			this._target = null;
		}

		public override void InitializeEnemey()
		{
		}

		public override void OnDrawGizmos()
		{
		}

	} // class E_Default
} // namespace AlienProject