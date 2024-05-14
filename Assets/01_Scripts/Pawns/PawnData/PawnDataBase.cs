using UnityEngine;

namespace AlienProject
{
	public abstract class PawnDataBase : ScriptableObject
	{
		[System.Serializable]
		public struct ResourceData
		{
			public float max;
			public float current;
		}

		[System.Serializable]
		public struct MovementData
		{
			public float maxSpeed;
			public float rotationSpeed;
			public float accelerationMagnitude;
			public float decelerationMagnitude;
		}

		public MovementData movementData;
		public ResourceData healthData;

	} // class PawnData
} // namespace AlienProject