using UnityEngine;

namespace AlienProject
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(CharacterController))]
	[Icon("Assets/Editor/Icons/MovementActionBase.png")]
	public abstract class MovementActionBase : MonoBehaviour
	{
		public enum EMovingState
		{
			Idle, Moving
		}
		// MARK: Component Caching

		protected CharacterController _characterController;

		// MARK: Inspector

		[SerializeField] protected float _maxSpeed = 5f;
		[SerializeField] protected bool _applyGravity = true;

		// MARK: Members

		protected Vector3 _movementVelocity;
		protected bool _isMovementDisabled = false;

		public void DisableMovement() => _isMovementDisabled = true;
		public void EnableMovement() => _isMovementDisabled = false;

		// MARK: Properties

		public bool IsGrounded => _characterController.isGrounded;
		public float MovementSpeed => _movementVelocity.magnitude;
		public Vector3 MovementDirection => _movementVelocity.normalized;
		public float MovementSpeedRatio => MovementSpeed / _maxSpeed;
		public bool IsMoving => MovementSpeed > 0f;

		#region Unity Callbacks

		protected virtual void Reset()
		{
			TryInitializeDodgeAction();
		}

		protected virtual void Awake()
		{
			_characterController = GetComponent<CharacterController>();
			TryInitializeDodgeAction();
		}

		#endregion // Unity Callbacks

		/// <summary>
		/// 이 컴포넌트가 부착된 게임 오브젝트(폰)의 이동 방향을 주어진 월드 좌표로 설정합니다.
		/// </summary>
		public abstract void MoveTo(Vector3 worldPosition);

		/// <summary>
		/// 이 컴포넌트가 부착된 게임 오브젝트(폰)의 이동 방향을 주어진 월드 방향으로 설정합니다.
		/// </summary>
		public abstract void MoveToward(Vector3 worldDirection);

		/// <summary>
		/// 실질적으로 이동을 처리하는 메서드입니다. <br/>
		/// 이 메서드는 FixedUpdate 에서 호출됩니다. <br/>
		/// 특정한 이동 방식을 구현하고 싶으면, 이 메서드를 오버라이드 하세요.
		/// </summary>
		protected abstract void Move();

		protected void TryApplyGravity()
		{
			if (IsGrounded)
			{
				return;
			}

			_characterController.Move(new Vector3(0, Physics.gravity.y, 0) * Time.deltaTime);
		}

		protected void TryInitializeDodgeAction()
		{
			var dodgeAction = GetComponent<DodgeActionBase>();

			if (!dodgeAction)
			{
				return;
			}

			// 회피 동작 중에도 이동이 가능하도록 설정되어 있지 않다면 이동을 막는다.
			if (dodgeAction.AllowMovementActionWhileDodging)
			{
				return;
			}

			dodgeAction.OnDodgeActionBegin.AddListener(DisableMovement);
			dodgeAction.OnDodgeActionBegin.AddListener(EnableMovement);
		}

	} // class MovementActionBase
} // namespace AlienProject

