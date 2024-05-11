using UnityEngine;

namespace AlienProject
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(MovementActionBase))]
	[Icon("Assets/Editor/Icons/PawnBase.png")]
	public abstract class PawnBase : MonoBehaviour
	{
		protected MovementActionBase _movementAction;

		#region Unity Callbacks
		protected virtual void Awake()
		{
			_movementAction = GetComponent<MovementActionBase>();
		}

		protected virtual void Start()
		{
		}

		#endregion // Unity Callbacks

		public void MoveTowardWorldDirection(Vector3 worldDirection)
		{
			_movementAction.MoveToward(worldDirection);
		}

		public void MoveToWorldPosition(Vector3 worldPosition)
		{
			_movementAction.MoveTo(worldPosition);
		}

	} // class PawnBase
} // namespace AlienProject