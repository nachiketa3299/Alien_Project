using UnityEngine;

namespace AlienProject
{
	public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
	{
		static T instance;
		static bool isApplicationQuitting = false;
		public static T Instance
		{
			get
			{
				if (isApplicationQuitting)
				{
					return null;
				}
				if (instance == null)
				{
					instance = (T)FindObjectOfType(typeof(T));

					if (FindObjectsOfType(typeof(T)).Length > 1)
					{
						Debug.LogError($"[Singletone]: 하나 이상의 {typeof(T).ToString()}형 싱글턴 객체가 가 씬에 존재합니다.");
						return instance;
					}

					if (instance == null)
					{
						GameObject singleton = new GameObject();
						instance = singleton.AddComponent<T>();
						singleton.name = typeof(T).ToString();

						Debug.Log
						(
							"[AlienSingleton] " + typeof(T).ToString() +
							" Ÿ���� �̱��� ��ü�� �ʿ��Ͽ�, " + singleton + "�� �����߽��ϴ�."
						);
					}
				}

				return instance;
			}
		}

		static bool IsDontDestroyOnLoad
		{
			get
			{
				if (instance == null)
				{
					return false;
				}

				if ((instance.gameObject.hideFlags & HideFlags.DontSave) == HideFlags.DontSave)
				{
					return true;
				}

				return false;
			}
		}

		public void OnDestroy()
		{
			if (IsDontDestroyOnLoad)
				isApplicationQuitting = true;
		}
	}

} // namespace AlienProject