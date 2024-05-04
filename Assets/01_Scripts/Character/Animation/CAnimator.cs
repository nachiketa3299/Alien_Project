// 2024-05-01 RZN

using UnityEngine;

namespace AlienProject
{
	/// <summary>
	/// 캐릭터의 애니메이션 관련 변수들을 계산하여, Animator 네이티브 컴포넌트에 전달하는 역할을 합니다.
	/// </summary>
	[RequireComponent(typeof(CMovementAction))]
	[RequireComponent(typeof(DodgeActionBase))]
	[AddComponentMenu("Alien Project/Character/Animation/Animator Component")]
	public class CAnimator : MonoBehaviour
	{
		// MARK: Component Caching

		[Header("필요한 캐릭터 컴포넌트")]
		[SerializeField]
		private CMovementAction _movementAction;
		[SerializeField]
		private DodgeActionBase _dodgeAction;

		// MARK: Inspector

		[Header("애니메이션 변수 이름 - 애니메이션 컨트롤러에 정의되어 있어야 합니다.")]
		// NOTE av 는 animation variable의 약자입니다.

		[SerializeField]
		private string _av_IsMoving = "IsMoving";

		[SerializeField]
		private string _av_IsDodging = "IsDodging";

		// private string _av_RotationDegree = "RotationDegree";

		[SerializeField]
		private string _av_RotationRatio = "RotationRatio";

		[SerializeField]
		private string _av_SpeedRatio = "SpeedRatio";

		// MARK: Memebrs

		/// <summary>
		/// 애니메이션 변수를 전달할 타겟 애니메이터입니다. 이 컴포넌트와 같은 오브젝트에 존재하지 않습니다.
		/// </summary>
		private Animator _targetAnimator;

		// MARK: Properties

		private float RotationDegree
		{
			get
			{
				var translatedMovementInput = _movementAction.TranslatedMovementInput;
				var angle = Vector3.Angle(transform.forward, translatedMovementInput);
				var axisSign = Vector3.Cross(transform.forward, translatedMovementInput).y > 0 ? 1f : -1f;

				return angle * axisSign;
			}
		}

		#region Unity Callbacks

		private void Awake()
		{
			_movementAction = GetComponent<CMovementAction>();
			_dodgeAction = GetComponent<DodgeActionBase>();

			if (!_dodgeAction.AllowMovementActionWhileDodging)
			{
				_dodgeAction.OnDodgeActionBegin.AddListener(EnterDodgeState);
				_dodgeAction.OnDodgeActionEnd.AddListener(ExitDodgeState);
			}

			InitializeTargetAnimator();
		}

		private void Update()
		{
			UpdateAnimationVariables();
		}

		#endregion // Unity Callbacks

		private void InitializeTargetAnimator()
		{
			_targetAnimator = GetComponent<Animator>();

			if (!_targetAnimator)
			{
				return;
			}

			_targetAnimator.applyRootMotion = false;
		}

		// MARK: Methods

		private void UpdateAnimationVariables()
		{
			if (!_targetAnimator)
			{
				return;
			}

			if (!_movementAction)
			{
				return;
			}

			_targetAnimator.SetBool(_av_IsMoving, _movementAction.IsMoving);
			_targetAnimator.SetFloat(_av_SpeedRatio, _movementAction.MovementSpeedRatio);
			//_targetAnimator.SetFloat(_av_RotationDegree, RotationDegree);
			_targetAnimator.SetFloat(_av_RotationRatio, RotationDegree / 180f);
		}

		/// <summary>
		/// DodgeActionBase.OnDodgeStart 이벤트에 바인딩되어, 회피 상태로 진입할 때에 호출됩니다.
		/// </summary>
		private void EnterDodgeState()
		{
			if (!_targetAnimator)
			{
				return;
			}

			if (!_dodgeAction)
			{
				return;
			}
			Debug.Log("[CAnimator] EnterDodgeState");

			_targetAnimator.SetTrigger(_av_IsDodging);
			_targetAnimator.applyRootMotion = true;
		}

		/// <summary>
		/// DodgeActionBase.OnDodgeEnd 이벤트에 바인딩되어, 회피 상태에서 빠져나올 때에 호출됩니다.
		/// </summary>
		public void ExitDodgeState()
		{
			if (!_targetAnimator)
			{
				return;
			}
			if (!_dodgeAction)
			{
				return;
			}

			Debug.Log("[CAnimator] ExitDodgeState");

			_targetAnimator.applyRootMotion = false;
		}

	} // class CAnimator
} // namespace AlienProject