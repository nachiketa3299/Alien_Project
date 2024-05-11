#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace AlienProject
{
	public partial class CWeaponSocket : MonoBehaviour
	{
		[CustomEditor(typeof(CWeaponSocket))]
		private class WeaponSocketEditor : Editor
		{
			private GameObject _prefab;
			private GameObject _meshInstance;
			private Color _sphereColor = Color.white - new Color(0, 0, 0, 0.5f);
			private CWeaponSocket _comp;
			private readonly float _viewHandleRadius = 0.1f;
			private readonly float _anchorThickness = 4f;

			protected void OnValidate()
			{
				if (!_prefab)
				{
					DestroyImmediate(_meshInstance);
				}
				else
				{
					if (_meshInstance)
					{
						if (_meshInstance != _prefab)
						{
							DestroyImmediate(_meshInstance);
							InstantiatePreviewMesh();
						}
					}
					else
					{
						InstantiatePreviewMesh();
					}
				}
			}

			private void InstantiatePreviewMesh()
			{
				_meshInstance = Instantiate(_prefab, _comp.gameObject.transform);
				_meshInstance.name = "Preview Mesh";
				_meshInstance.hideFlags = HideFlags.NotEditable;
			}

			protected void OnDisable()
			{
				DestroyImmediate(_meshInstance);
			}

			private void OnEnable()
			{
				_comp = target as CWeaponSocket;
			}

			public override void OnInspectorGUI()
			{
				base.OnInspectorGUI();

				GUILayout.Space(10);
				GUILayout.Label("커스텀 에디터 옵션", "boldLabel");

				EditorGUI.BeginChangeCheck();

				EditorGUILayout.HelpBox
				(
					"1. 프리뷰 메쉬를 설정하면, 해당 프리뷰 메쉬가 무기 소켓의 위치에 표시됩니다. (Prefabs/Weapons 의 무기 프리팹을 사용해주세요.)\n" +
					"2. 프리뷰 메쉬는 에디터에서만 표시되며, 플레이 모드나 빌드 시에는 표시되지 않습니다.\n" +
					"3. 프리뷰 메쉬를 기준으로, 캐릭터에 재생중인 애니메이션에 맞게 무기 소켓의 위치와 회전을 조정할 수 있습니다.\n" +
					"4. 무기 소켓의 위치와 회전을 조정하고 나면, 이 오브젝트의 설정된 Transform을 프리셋으로 저장해 주세요.\n" +
					"5. 프리셋은 PS_무기를 사용하는 캐릭터@무기 이름_SocketTransform으로 저장해 주세요."
					, MessageType.Info, true
				);

				_prefab = EditorGUILayout.ObjectField("Preview Mesh", _prefab, typeof(GameObject), false) as GameObject;

				if (EditorGUI.EndChangeCheck())
				{
					OnValidate();
				}
			}

			private void OnSceneGUI()
			{
				var comp = target as CWeaponSocket;

				var tr = comp.transform;

				Handles.color = _sphereColor;
				Handles.RadiusHandle(tr.rotation, tr.position, _viewHandleRadius);

				Handles.color = Color.red;
				Handles.DrawLine(tr.position, tr.position + tr.right * _viewHandleRadius, _anchorThickness);

				Handles.color = Color.green;
				Handles.DrawLine(tr.position, tr.position + tr.up * _viewHandleRadius, _anchorThickness);

				Handles.color = Color.blue;
				Handles.DrawLine(tr.position, tr.position + tr.forward * _viewHandleRadius, _anchorThickness);
			}

			private void Awake()
			{
				DestroyImmediate(_meshInstance);
			}

		} // class WeaponSocketEditor
#endif // UNITY_EDITOR

	} // class CWeaponSocket
} // namespace AlienProject