// 2024-04-14 Checked By RZN
// 실험 단계이므로 기본적으로 비활성화된 상태로 MainCamera에 부착하겠습니다.
// 사용하려면 씬에서 Wall 레이어로 설정된 오브젝트의 머터리얼을 M_CameraCutout 으로 설정해야 합니다.

using UnityEngine;

namespace AlienProject
{

	/// <summary>
	/// 카메라에서 지정된 타겟(플레이어) RayCast를 수행 후 지정된 WallMask에 맞는 오브젝트를 감지하고 Cutout을 적용하는 클래스
	/// </summary>
	[AddComponentMenu("Alien Project/Experimental/Camera Cutout Object Component")]
	public partial class CCameraCutoutObject : MonoBehaviour
	{
		[SerializeField]
		[Tooltip("Cutout을 적용할 타겟 오브젝트(플레이어 오브젝트)")]
		private Transform _targetObject;

		[SerializeField]
		[Tooltip("Cutout을 적용할 벽 레이어 마스크(기본값: Walls)")]
		private LayerMask _wallMask;

		[SerializeField]
		[Tooltip("Cutout의 크기(기본값: 0.2)")]
		[Range(0f, 10f)]
		private float _cutoutSize = 0.2f;

		[SerializeField]
		[Tooltip("Falloff의 크기(기본값: 0.1)")]
		[Range(0f, 10f)]
		private float _falloffSize = 0.10f;

		#region Unity Events

		private void FixedUpdate()
		{
			var targetPosition = _targetObject.position;
			var targetDirection = _targetObject.position - transform.position;

			Vector2 cutOutPositionInViewport = Camera.main.WorldToViewportPoint(targetPosition);
			cutOutPositionInViewport.y /= Screen.width / Screen.height;

			var hitObjects = Physics.RaycastAll(transform.position, targetDirection, targetDirection.magnitude, _wallMask);
			foreach (var hitObject in hitObjects)
			{
				var materials = hitObject.transform.GetComponent<Renderer>().materials;
				foreach (var material in materials)
				{
					material.SetVector("_CutoutPosition", cutOutPositionInViewport);
					material.SetFloat("_CutoutSize", _cutoutSize);
					material.SetFloat("_FalloffSize", _falloffSize);
				}
			}
		}

		#endregion // Unity Events
	}
}