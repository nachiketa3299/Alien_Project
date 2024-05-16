using UnityEngine;

namespace AlienProject
{
	[CreateAssetMenu(fileName = "SO_NewEnemyData", menuName = "Alien Project/Data/Enemy Data")]
	public class OEnemyData : PawnDataBase
	{
		// NOTE 임시!!! 뭔가 초기화 방법이 잘못 되었음 지금

		[System.Serializable]
		public struct VisionData
		{
			public float searchDelay;
			public float visionRadius;
			public float visionAngle;
			public LayerMask targetLayerMask;
			public LayerMask searchLayerMask;
			public LayerMask obstacleLayerMask;
		}

		public VisionData visionData;

	} // class OEnemyData
} // namespace AlienProject