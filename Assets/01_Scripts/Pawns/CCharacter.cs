using UnityEngine;

namespace AlienProject
{
	[RequireComponent(typeof(CStaminaPoint))]
	[AddComponentMenu("Alien Project/Character Component")]
	public class CCharacter : PawnBase
	{
		private CStaminaPoint _sp;

		#region IDamageable Impelementation

		public override void TakeDamage(float damageAmount)
		{ }

		#endregion // IDamageable Impelementation

		#region Unity Callbacks

		protected override void Awake()
		{
			base.Awake();

			_sp = GetComponent<CStaminaPoint>();
		}

		#endregion // Unity Callbacks

	} // class CCharacter
} // namespace AlienProject