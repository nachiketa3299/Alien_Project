// 2024-04-15 Checked by RZN

#if UNITY_EDITOR

using UnityEngine;

namespace AlienProject
{
	public partial class CCameraCutoutObject : MonoBehaviour
	{
		[Header("디버그 설정")]

		[SerializeField]
		private bool _enableDraw = true;

		[SerializeField]
		private float _hitSphereRadius = 0.2f;

		private Color _rayColor = Color.cyan;
		private Color _hitSphereColor = Color.red;

		#region Unity Events

		private void OnDrawGizmos()
		{
			if (!Application.isPlaying)
			{
				return;
			}

			if (!_enableDraw)
			{
				return;
			}

			var cameraPosition = transform.position;
			var targetPosition = _targetObject.position;

			var cutoutPositionInViewport = Camera.main.WorldToViewportPoint(targetPosition);
			cutoutPositionInViewport.y /= Screen.width / Screen.height;

			Gizmos.color = Color.yellow;
			DebugUtility.DrawCircle(Camera.main.ViewportToWorldPoint(cutoutPositionInViewport), Camera.main.transform.forward, Camera.main.transform.up, _cutoutSize * 25, 36, Color.yellow, 0f);

			Debug.DrawLine(cameraPosition, targetPosition, _rayColor);

			var rayOrigin = cameraPosition;
			var rayDirection = targetPosition - cameraPosition;

			var hitObjects = Physics.RaycastAll(rayOrigin, rayDirection, rayDirection.magnitude, _wallMask);

			foreach (var hitObject in hitObjects)
			{
				// Draw Ray Hit Sphere
				Gizmos.color = _hitSphereColor;
				Gizmos.DrawWireSphere(hitObject.point, _hitSphereRadius);
			}
		}

		#endregion // Unity Events
	} // class CCameraCutoutObject
} // namespace AlienProject
#endif // UNITY_EDITOR