using UnityEngine;

namespace AlienProject
{
	[RequireComponent(typeof(CStaminaPoint))]
	[AddComponentMenu("Alien Project/Character Component")]
	public class CCharacter : PawnBase
	{
		private CStaminaPoint _sp;

		// MARK: Inspector

		#region IDamageable Impelementation

		public override void TakeDamage(float damageAmount)
		{ }

		#endregion // IDamageable Impelementation

		// MARK: Inspector

		[SerializeField]
		private OCharacterData _characterData;

		#region Unity Callbacks

		protected override void Awake()
		{
			base.Awake();

			_sp = GetComponent<CStaminaPoint>();
		}

		protected override void Start()
		{
			base.Start();

			if (_shouldInitalizeWithPawnData)
			{
				foreach (var initializable in GetComponents<IInitializable>())
				{
					initializable.Initialize(_characterData);
				}
			}
		}

		#endregion // Unity Callbacks

	} // class CCharacter
} // namespace AlienProject