using UnityEngine.AI;
using UnityEngine;
using UnityEditor;

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

		#region Unity Callbacks


		protected override void Awake()
		{
			base.Awake();

			_navMeshAgent = GetComponent<NavMeshAgent>();
			_navMeshPath = new();

		}

		protected override void Start()
		{
			base.Start();

			var enemyPawn = _possesedPawn as CEnemy;

			enemyPawn.OnTargetDetected.AddListener
			(
				(tr) =>
				{
					_navMeshAgent.CalculatePath(tr.position, _navMeshPath);
				}
			);

			enemyPawn.OnTargetEmpty.AddListener
			(
				(tr) =>
				{
					_navMeshPath.ClearCorners();
				}
			);
		}

		#endregion // Unity Callbacks
	} // class CEnemyAIController
} // namespace AlienProject