using System;
using UnityEngine;

namespace AlienProject
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(MovementActionBase))]
	[RequireComponent(typeof(CHealthPoint))]
	public abstract class PawnBase : MonoBehaviour, IDamageable
	{
		// MARK: Component Caching

		protected MovementActionBase _movementAction;
		protected CHealthPoint _hp;

		// MARK: Inspector

		[SerializeField]
		protected bool _shouldInitalizeWithPawnData = false;

		#region IDamageable Impelementation
		public abstract void TakeDamage(float damageAmount);

		#endregion // IDamageable Impelementation

		#region Unity Callbacks

		protected virtual void Awake()
		{
			_movementAction = GetComponent<MovementActionBase>();
			_hp = GetComponent<CHealthPoint>();
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