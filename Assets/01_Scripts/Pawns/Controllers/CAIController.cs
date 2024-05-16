using UnityEngine.AI;
using UnityEngine;
using System.Collections.Generic;

namespace AlienProject
{
	/// <summary>
	/// 적 폰에게 추상적인 명령을 내리는 AI 컨트롤러입니다.
	/// </summary>
	[RequireComponent(typeof(NavMeshAgent))]
	[AddComponentMenu("Alien Project/Controllers/Enemy AI Controller Component")]
	public partial class CAIController : PawnControllerBase
	{
		// NOTE 비헤이비어 트리 같은 AI 프레임워크가 있다면, 여기에 추가되어야 함.

		private NavMeshAgent _navMeshAgent;
		private NavMeshPath _navMeshPath;

		private List<NavMeshHit> _patrolPoints = new();

		#region Unity Callbacks

		protected override void Awake()
		{
			base.Awake();

			_navMeshAgent = GetComponent<NavMeshAgent>();
			_navMeshPath = new NavMeshPath();
		}

		protected override void Start()
		{
			base.Start();

			var enemyPawn = _possesedPawn as CEnemy;

			enemyPawn.ToPatroling.AddListener(OnPatroling);
			enemyPawn.ToChasing.AddListener(OnChasing);
		}
		private void OnPatroling()
		{
			_navMeshAgent.ResetPath();
			_navMeshPath.ClearCorners();
		}

		private void OnChasing(GameObject target)
		{
			_patrolPoints.Clear();

			_navMeshAgent.ResetPath();
			_navMeshPath.ClearCorners();

			_navMeshAgent.CalculatePath(target.transform.position, _navMeshPath);
		}

		protected void Update()
		{
			// NOTE 제대로 작동하지 않음

			var state = (_possesedPawn as CEnemy).State;
			switch (state)
			{
				case CEnemy.EEnemyState.Patroling:
				case CEnemy.EEnemyState.Chasing:
					if (_navMeshPath.corners.Length > 0)
					{
						_possesedPawn.MoveToWorldPosition(_navMeshPath.corners[1]);
					}
					break;
				case CEnemy.EEnemyState.Attacking:
				case CEnemy.EEnemyState.Idle:
					break;
			}
		}

		#endregion // Unity Callbacks

	} // class CEnemyAIController
} // namespace AlienProject