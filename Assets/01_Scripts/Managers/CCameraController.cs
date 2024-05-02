// [] 2024-04-10 Checked by RZN

using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;
using UnityEditor.EditorTools;

namespace AlienProject
{
	/// <summary>
	/// <para>Cinemachine 카메라와 해당 시네머신에 영향받는 메인 카메라에 어태치된 CinemachineBrain 컴포넌트의 전반적인 설정을 관리하는 매니저 클래스이다.</para>
	/// <para>씬에 VirtualCamera 태그를 가진 시네머신 카메라가 존재해야 하며, 기본적으로 4개의 카메라가 씬에 존재한다고 가정한다.</para>
	/// </summary>
	// TODO 이게 싱글턴이어야 하는지 진지하게 고려할 필요가 있음
	[AddComponentMenu("Alien Project/Managers/Camera Manager")]
	[RequireComponent(typeof(CGameManager))] // 씬의 플레이어를 받아와야 함
	public partial class CCameraController : Singleton<CCameraController>
	{
		private CGameManager _gameManagerBase;
		private Camera _mainCamera;
		// TODO Ortho / Perspective 인스펙터 전환 구현

		private CinemachineBrain _mainCameraBrain;
		private GameObject[] _virtualCameraObjects;
		private CinemachineVirtualCamera[] _virtualCameraComponents;

		public GameObject InScenePlayer => _gameManagerBase.InScenePlayer;
		private int VirtualCameraCounts => _virtualCameraObjects.Length;
		private int _currentCameraIndex = 0;
		private float _cameraYawDifference;

		// 인스펙터 설정
		// TODO Orthographic / Perspective 전환 설정
		[Header("메인 카메라 설정")]


		[Header("매니저가 관리할 가상 카메라 설정")]

		[SerializeField]
		[Tooltip("가상 카메라 오브젝트가 가질 태그를 설정(기본값: VirtualCamera)")]
		private string _virtualCameraTag = "VirtualCamera";

		[SerializeField]
		private Transform _virtualCamerasFollow;

		[Space]

		// TODO 충돌 시 카메라가 오브젝트를 자동으로 피할 수 있다면 좋을 것 같은데, 이게 가능할 지는 모르겠음. (Cinemachine의 기능을 더 찾아봐야 할 듯)
		[SerializeField]
		[Range(1f, 100f)]
		[Tooltip("가상 카메라의 거리를 설정(기본값: 10), 너무 낮은 경우 카메라가 오브젝트와 겹칠 수 있음")]
		private float _virtualCameraDistance = 60f;

		[SerializeField]
		[Range(1f, 100f)]
		[Tooltip("가상 카메라의 Orthographic Size를 설정(기본값: 15)")]
		private float _orthoSize = 15f;

		[Space]

		[SerializeField]
		[Range(0f, 90f)]
		[Tooltip("0번 카메라가 캐릭터를 바라보는 각도를 설정(기본값: 45)")]
		private float _cameraYawOffset = 45f;

		[SerializeField]
		[Range(0f, 90f)]
		[Tooltip("카메라가 캐릭터를 바라보는 Pitch를 설정(기본값: 30)")]
		private float _cameraPitch = 30f;

		[Header("가상 카메라 간 블렌딩 설정")]

		[SerializeField]
		[CinemachineBlendDefinitionProperty]
		[Tooltip("블렌딩 방법 설정(기본값: Hardout, 0.5)")]
		private CinemachineBlendDefinition _transitionBlend = new(CinemachineBlendDefinition.Style.HardOut, 0.5f);

		[SerializeField]
		[Tooltip("카메라 블렌딩 궤적 설정(기본값: CylindricalPosition)")]
		private CinemachineVirtualCamera.BlendHint _blendHint = CinemachineVirtualCamera.BlendHint.CylindricalPosition;

		[SerializeField]
		[Tooltip("카메라 회전 입력이 반대로 적용될 지를 설정(기본값: false)")]
		private bool _flipRotateDirection = false;

		#region Unity Events

		void Awake()
		{
			_gameManagerBase = GetComponent<CGameManager>();
		}

		void OnValidate()
		{
			// (참고) OnValidate는 인스펙터에서 변경 사항이 있을 때마다 Callback됨
			if (!_mainCamera)
			{
				return;
			}

			UpdateMainCameraSettings();
			UpdateVirtualCameraSettings();
		}

		void Start()
		{
			InitializeCameras();
		}


		#endregion // Unity Events

		void InitializeCameras()
		{
			_mainCamera = Camera.main;
			_mainCameraBrain = _mainCamera.GetComponent<CinemachineBrain>();

			_virtualCameraObjects = GameObject.FindGameObjectsWithTag(_virtualCameraTag);
			_virtualCameraComponents = new CinemachineVirtualCamera[VirtualCameraCounts];

			_cameraYawDifference = 360f / VirtualCameraCounts;

			for (int i = 0; i < VirtualCameraCounts; ++i)
			{
				_virtualCameraComponents[i] = _virtualCameraObjects[i].GetComponent<CinemachineVirtualCamera>();
				_virtualCameraObjects[i].SetActive(false);
			}

			UpdateMainCameraSettings();
			UpdateVirtualCameraSettings();

			_virtualCameraObjects[_currentCameraIndex].SetActive(true);
		}

		private void UpdateVirtualCameraSettings()
		{
			for (int i = 0; i < VirtualCameraCounts; ++i)
			{
				var cameraYaw = _cameraYawOffset + _cameraYawDifference * i;
				var cameraRotation = Quaternion.Euler(_cameraPitch, cameraYaw, 0f);

				_virtualCameraObjects[i].transform.rotation = cameraRotation;

				_virtualCameraComponents[i].Follow = _virtualCamerasFollow;

				_virtualCameraComponents[i].m_Transitions.m_BlendHint = _blendHint;

				_virtualCameraComponents[i].GetCinemachineComponent<CinemachineFramingTransposer>().m_CameraDistance = _virtualCameraDistance;
				_virtualCameraComponents[i].m_Lens.OrthographicSize = _orthoSize;
			}
		}

		private void UpdateMainCameraSettings()
		{
			_mainCameraBrain.m_DefaultBlend = _transitionBlend;
			_virtualCamerasFollow = _gameManagerBase.InScenePlayer.transform;
		}


		private void ChangeToNextVirtualCamera()
		{
			_virtualCameraObjects[_currentCameraIndex].SetActive(false);
			var nextCameraIndex = (_currentCameraIndex < VirtualCameraCounts - 1) ? _currentCameraIndex + 1 : 0;
			_virtualCameraObjects[nextCameraIndex].SetActive(true);

			_currentCameraIndex = nextCameraIndex;
		}

		private void ChangeToPreviousVirtualCamera()
		{
			_virtualCameraObjects[_currentCameraIndex].SetActive(false);
			var prevCameraIndex = (_currentCameraIndex > 0) ? _currentCameraIndex - 1 : VirtualCameraCounts - 1;
			_virtualCameraObjects[prevCameraIndex].SetActive(true);

			_currentCameraIndex = prevCameraIndex;
		}
	}
} // namespace AlienProject
