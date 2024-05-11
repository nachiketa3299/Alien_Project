using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace AlienProject
{
	/// <summary>
	/// <para>눈(Eye)으로 지정된 오브젝트 기준으로 시야를 검사하여 타겟(플레이어)를 찾는 컴포넌트입니다.</para>
	/// <listheader>아래와 같은 단계를 순차적으로 밟아, 최종적으로 명시된 태그의 타겟을 찾아냅니다.</listheader>
	/// <list type="number">
	/// 	<item>
	/// 		<term>Is In Vision Radius?</term>
	/// 		<description>지정된 반경 내에 있는 모든 대상을 검색합니다.</description>
	/// 	</item>
	/// 	<item>
	/// 		<term>Is In Vision Angle?</term>
	/// 		<description>지정된 시야 각 내에 있는 모든 대상을 필터링합니다.</description>
	/// 	</item>
	/// 	<item>
	/// 		<term>Is There Obstacle Between?</term>
	/// 		<description>필터링 된 대상들과 이 객체 사이에 장애물이 있는지 확인합니다.</description>
	///		</item>
	/// 	<item>
	/// 		<term>Is Tagged Target?</term>
	/// 		<description>필터링 된 대상들 중, 태그가 지정된 타겟을 찾습니다.</description>
	///		</item>
	/// </list>
	/// </summary>
	[AddComponentMenu("Alien Project/Sensors/Vision Component")]
	public partial class CVision : SensorBase
	{
		// MARK: Events

		/// <summary>
		/// 타겟 검색의 첫 단계에서, 장애물과 관계 없이 타겟이 범위 내에 감지되었을 때에 호출되는 이벤트입니다.
		/// </summary>
		[HideInInspector] public UnityEvent<Transform> OnSearchedTargetInVisionRadius;

		[HideInInspector] public UnityEvent<Transform> OnSearchedTargetInVisionAngle;


		/// <summary>
		/// 타겟이 범위 내에 감지되었으나, 타겟과 눈 사이에 장애물이 있어 타겟을 볼 수 없을때 호출되는 이벤트입니다. <br/>
		/// 첫 번째 인자로는 타겟의 트랜스폼을 전달하고, <br/>
		/// 두 번째 인자로는 타겟과 눈 사이, 시선을 가리는 장애물의 충돌 위치(Vector3)를 전달합니다.
		/// </summary>
		[HideInInspector] public UnityEvent<Transform, Vector3> OnSearchedTargetCovered;

		[HideInInspector] public UnityEvent<Transform> OnTargetInVision;

		// MARK: Inspector

		[Header("눈(Eye) 오브젝트 설정")]

		[SerializeField, Tooltip("시야의 중심이 되는 오브젝트 입니다.")]
		GameObject _eyeObject;

		[Header("시각(Vision)과 검색(Search) 설정")]

		[SerializeField, Range(0f, 30f)]
		private float _visionRadius;

		[SerializeField, Range(0f, 360f)]
		private float _visionAngle;

		[SerializeField, Range(0f, 1f)]
		private float _searchDelay = 0.2f;

		[Header("검색 필터")]

		[SerializeField]
		private LayerMask _searchLayerMask; // TODO 이름!!


		[SerializeField]
		private LayerMask _obstacleLayerMask;

		// MARK: NonAlloc Caching Members
		private const int MAX_CACHE_SIZE = 10;

		private readonly Collider[] _collidersInVisionRadius = new Collider[MAX_CACHE_SIZE];
		private int _nOfCollidersInRadius;

		// Objects in Vision Radius & Vision Angle  (_searchLayerMask)  -> "In Vision"
		private readonly GameObject[] _objs_V = new GameObject[MAX_CACHE_SIZE];
		private int _nOf_objs_V;

		// Objects in Vision & No Obstacle Between (_searchLayerMask)
		private readonly GameObject[] _objs_V_NO = new GameObject[MAX_CACHE_SIZE];
		private int _nOf_objs_V_NO;

		// Objects in Vision & Has Obstacle Between (_searchLayerMask)
		private readonly GameObject[] _objs_V_O = new GameObject[MAX_CACHE_SIZE];
		private readonly Vector3[] _obs_hit_poss = new Vector3[MAX_CACHE_SIZE];
		private int _nOf_objs_V_O;

		// Objects in Vision & No Obstacle Between & Tracking Target (_searchLayerMask & _trackingTargetLayerMask)
		private readonly GameObject[] _objs_V_NO_TR = new GameObject[MAX_CACHE_SIZE];
		private int _nOf_objs_V_NO_TR;

		private void ClearAllNonAllocCacheSizes()
		{
			_nOfCollidersInRadius = 0; // 얘는 이거 아니더라도 초기화 되긴 하지만, 명시적으로 초기화
			_nOf_objs_V = 0;
			_nOf_objs_V_NO = 0;
			_nOf_objs_V_O = 0;
			_nOf_objs_V_NO_TR = 0;
		}

		private List<GameObject> _visibleTargets = new();

		// MARK: Properties

		public List<GameObject> VisibleTargets => _visibleTargets;
		public float VisionRadius => _visionRadius;
		public float VisionAngle => _visionAngle;
		public Transform EyeTransform => _eyeObject.transform;

		#region Unity Callbacks

		protected override void Awake()
		{
			// Set Eye Object
			if (_eyeObject)
			{
				Debug.Log($"[CVision]: 눈 역할의 오브젝트({_eyeObject.name})를 찾았습니다.");
			}
			else
			{
				Debug.LogWarning($"[CVision]: 눈 역할의 오브젝트를 찾지 못했습니다. 인스펙터에서 할당해 주세요.");
			}
		}

		protected override void Start()
		{
			StartCoroutine(VisionRoutine(_searchDelay));
		}

		#endregion // Unity Callbacks

		// MARK: Coroutines

		/// <summary>
		/// 오버랩 스피어로 감지되는 모든 타겟 중 (1) 시야각 안에 들어오고, (2) 장애물에 가리지 않는 타겟을 찾아 
		/// _visibleTargets에 추가합니다. 코루틴으로, 주어진 시간 간격 만큼 반복하여 실행됩니다.
		/// </summary>
		private IEnumerator VisionRoutine(float delay)
		{
			while (true)
			{
				_visibleTargets.Clear();
				_trackingObjects.Clear();

				ClearAllNonAllocCacheSizes();

				var visionPivot = _eyeObject.transform.position;

				// [Filter 0]: Is In Vision Radius with LayerMask(_searchLayerMask)?

				_nOfCollidersInRadius = Physics.OverlapSphereNonAlloc
				(
					position: visionPivot,
					radius: _visionRadius,
					results: _collidersInVisionRadius,
					layerMask: _searchLayerMask
				);

				if (_nOfCollidersInRadius == 0)
				{
					if (_trackingObjects.Count > 0)
					{
						_trackingObjects.ForEach
						(
							obj => OnTrackingTargetLost?.Invoke(obj.transform)
						);
					}
					else
					{
						OnTrackingTargetEmpty?.Invoke();
					}

					goto WaitAndPrepareNewSearch; // break 도 가능하나, 흐름을 명확하게 하기 위해 goto 사용
				}

				// 하나라도 타겟이 감지되었을 때에만 아래 for 문에 진입

				for (var i = 0; i < _nOfCollidersInRadius; ++i)
				{
					var currentTargetObject = _collidersInVisionRadius[i].gameObject;
					var currentTargetObjectCenter = GetColliderCenter(_collidersInVisionRadius[i]);

					// [Filter 1]: Is In Vision Angle?

					var visionPivotForward = _eyeObject.transform.forward;
					var directionToTarget = (currentTargetObjectCenter - visionPivot).normalized;

					var halfAngle = Vector3.Angle
					(
						from: visionPivotForward,
						to: directionToTarget
					);
					var isInVisionAngle = halfAngle * 2f < _visionAngle;

					if (!isInVisionAngle)
					{
						continue;
					}

					_objs_V[_nOf_objs_V++] = currentTargetObject;

					// [Filter 2]: Is There Obstacle(layerMask) Between Searched Target and Eye?

					var isSomethingHit = Physics.Linecast
					(
						start: visionPivot,
						end: currentTargetObjectCenter,
						hitInfo: out RaycastHit obstacleHitInfo,
						layerMask: _obstacleLayerMask
					);

					if (isSomethingHit)
					{
						// 자기 자신과 충돌하는 경우(그런 경우도 있다!), 장애물이 없는 것으로 간주
						var hitObject = obstacleHitInfo.transform.gameObject;
						var isSelfHit = hitObject == currentTargetObject;
						var isThereObstacle = isSomethingHit && !isSelfHit;

						if (isThereObstacle)
						{
							Debug.Log($"[CVision]: 장애물({hitObject})에 가려져 타겟({currentTargetObject})을 볼 수 없습니다.");

							_objs_V_O[_nOf_objs_V_O] = currentTargetObject;
							_obs_hit_poss[_nOf_objs_V_O] = obstacleHitInfo.point;
							_nOf_objs_V_O++;

							var obstacleHitPosition = obstacleHitInfo.point;

							OnSearchedTargetCovered?.Invoke(currentTargetObject.transform, obstacleHitPosition);

							continue;
						}
					}

					// After [Filter 1] & [Filter 2], call that target is "visible".

					_objs_V_NO[_nOf_objs_V_NO++] = currentTargetObject;

					_visibleTargets.Add(currentTargetObject); // TODO
					OnTargetInVision?.Invoke(currentTargetObject.transform);

					// [Filter 3] Is Tracking Target?

					bool isInTrackingLayer = IsLayerInLayerMask(currentTargetObject.layer, _trackingTargetLayerMask);

					if (!isInTrackingLayer)
					{
						continue;
					}

					_objs_V_NO_TR[_nOf_objs_V_NO_TR++] = currentTargetObject;

					_trackingObjects.Add(currentTargetObject); // TODO
					OnTrackingTargetDetected?.Invoke(currentTargetObject.transform);

					Debug.Log($"[CVision] 추적 타겟 ({currentTargetObject}) 발견");
				}

			WaitAndPrepareNewSearch:
				yield return new WaitForSeconds(delay);
			}
		}

		// MARK: Methods

		// TODO 유틸리티 함수이지만 여기에 위치하는 것이 과연 맞는지?
		public static bool IsLayerInLayerMask(int layer, LayerMask layerMask) => (layerMask & (1 << layer)) != 0;

		private static Vector3 GetColliderCenter(Collider collider) => collider.bounds.center;

	} // class CEnemyVision
} // namespace AlienProject