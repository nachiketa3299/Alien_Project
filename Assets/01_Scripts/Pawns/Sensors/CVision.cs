using System.Collections;
using UnityEngine;

namespace AlienProject
{
	/// <summary>
	/// <para>눈(Eye)으로 지정된 오브젝트 기준으로 시야를 검사하여 타겟(플레이어)를 찾는 컴포넌트입니다.</para>
	/// <listheader>아래와 같은 단계를 순차적으로 밟아, 최종적으로 명시된 태그의 타겟을 찾아냅니다.</listheader>
	/// </summary>
	[AddComponentMenu("Alien Project/Sensors/Vision Component")]
	public partial class CVision : SensorBase, IInitializable
	{
		public struct FVisionData
		{
			public GameObject target;
			public bool isCoveredByObstacle;
			public RaycastHit obstacleHitInfo;
			public bool isTrackingTarget;
		}

		private class VisionDataCache
		{

			private readonly FVisionData[] _cache = new FVisionData[MAX_CACHE_SIZE];
			private int _size = 0;
			public int Length => _size;

			public ref readonly FVisionData Data(int index)
			{
				return ref _cache[index];
			}

			// NOTE MAX_CACHE_SIZE 에 의존함
			public void Add(in FVisionData data)
			{
				_cache[_size] = data;
				_size++;
			}

			public void Clear() => _size = 0;
		}

		// MARK: Members

		private VisionDataCache _visionDataCache;

		// NOTE 10개 이상의 오브젝트를 검출할 일은 거의 없다고 판단하여, 10개로 제한
		private const int MAX_CACHE_SIZE = 10;

		private readonly Collider[] _colliderCache = new Collider[MAX_CACHE_SIZE];
		private int _n_colliderCache;

		// MARK: Properties

		// 안 쓰지만 추후 사용할 수도 있으므로 남겨둚
		public float VisionRadius => _visionRadius;
		public float VisionAngle => _visionAngle;
		public Transform EyeTransform => _eyeObject.transform;

		#region IInitializable Implementation

		public void Initialize(PawnDataBase data)
		{
			var enemyData = data as OEnemyData;

			_visionRadius = enemyData.visionData.visionRadius;
			_visionAngle = enemyData.visionData.visionAngle;
			_searchDelay = enemyData.visionData.searchDelay;
			_trackingTargetLayerMask = enemyData.visionData.targetLayerMask;
			_searchLayerMask = enemyData.visionData.searchLayerMask;
			_obstacleLayerMask = enemyData.visionData.obstacleLayerMask;
		}

		#endregion

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

		#region Unity Callbacks

		protected override void Awake()
		{
		}

		protected override void Start()
		{
			_visionDataCache = new VisionDataCache();

			StartCoroutine(VisionRoutine(_searchDelay));
		}

		#endregion // Unity Callbacks

		// MARK: Coroutines

		private IEnumerator VisionRoutine(float delay)
		{
			while (true)
			{
				_visionDataCache.Clear();

				_n_colliderCache = Physics.OverlapSphereNonAlloc
				(
					position: _eyeObject.transform.position,
					radius: _visionRadius,
					results: _colliderCache, // MAX_CACHE_SIZE
					layerMask: _searchLayerMask
				);

				for (var i = 0; i < _n_colliderCache; ++i)
				{
					if (!IsColliderInVisionAngle(in _colliderCache[i]))
					{
						continue;
					}

					var data = new FVisionData()
					{
						target = _colliderCache[i].gameObject,
						isCoveredByObstacle = false,
						obstacleHitInfo = new(),
						isTrackingTarget = false
					};

					data.isCoveredByObstacle = IsThereObtacleBetween(in _colliderCache[i], out data.obstacleHitInfo);
					data.isTrackingTarget = IsLayerInLayerMask(data.target.layer, _trackingTargetLayerMask);

					_visionDataCache.Add(in data);

					// Event Invocations

					if (data.isTrackingTarget && !data.isCoveredByObstacle)
					{
						TrackingTargetFound?.Invoke(data.target);
					}
				}

				if (_visionDataCache.Length == 0)
				{
					TrackingTargetEmpty?.Invoke();
				}

				yield return new WaitForSeconds(delay);
			}
		}
		bool IsThereObtacleBetween(in Collider targetCollider, out RaycastHit hitInfo)
		{
			var isSomethingHit = Physics.Linecast
			(
				start: _eyeObject.transform.position,
				end: targetCollider.bounds.center,
				hitInfo: out hitInfo,
				layerMask: _obstacleLayerMask
			);

			if (!isSomethingHit)
			{
				return false;
			}

			var isSelfHit = hitInfo.transform.gameObject == targetCollider.gameObject;

			if (isSelfHit)
			{
				return false;
			}

			return true;
		}

		bool IsColliderInVisionAngle(in Collider collider)
		{
			var halfAngle = Vector3.Angle
			(
				from: _eyeObject.transform.forward,
				to: collider.bounds.center - _eyeObject.transform.position
			);
			return halfAngle * 2f < _visionAngle;
		}

		public static bool IsLayerInLayerMask(int layer, LayerMask layerMask) => (layerMask & (1 << layer)) != 0;

	} // class CEnemyVision
} // namespace AlienProject