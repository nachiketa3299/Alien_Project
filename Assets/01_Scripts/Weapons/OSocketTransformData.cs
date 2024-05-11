using UnityEngine;

namespace AlienProject
{
	[CreateAssetMenu(fileName = "SO_NewSocketTransformData", menuName = "Alien Project/Data/Weapon Socket Transform Data")]
	public class OSocketTransformData : ScriptableObject
	{
		[System.Serializable]
		public struct SocketData
		{
			public Vector3 position;
			public Vector3 rotation;
			public Vector3 scale;
		}

		public SocketData socketData;
	}
} // namespace AlienProject