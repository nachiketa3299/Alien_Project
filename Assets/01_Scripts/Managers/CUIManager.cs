using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 
/// </summary>
public class CUIManager : MonoBehaviour
{
	[SerializeField] private GameObject actionSetPanel;

	// private bool actionSetState = false; 
	[SerializeField] private GameObject _damagePrefab;
	[SerializeField] private GameObject _damageCanvas;
	public static CUIManager UIManager;

	private void Awake()
	{
		UIManager = this;
		//actionSetPanel.SetActive(actionSetState);
	}

	// void Start()
	// {
	// }

	// Update is called once per frame
	// void Update()
	// {
	// }

	public void OnEsc(InputAction.CallbackContext context)
	{
		if (context.performed)
		{
			actionSetPanel.SetActive(true);
			Debug.Log("Esc");
		}
	}

	public void GeneratePopUP(float damage, Vector3 position)
	{
		GameObject damagePrefab = Instantiate(_damagePrefab, position, quaternion.identity, _damageCanvas.transform);

		damagePrefab.GetComponent<TextMeshProUGUI>().text = damage.ToString();
	}
}