using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

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
	public void OnEsc(InputAction.CallbackContext context)
	{
		if (context.performed)
		{
			actionSetPanel.SetActive(true);
			Debug.Log("Esc");
		}
	}

	public void GeneratePopUP(int damage, Vector3 position)
	{
		GameObject damagePrefab = Instantiate(_damagePrefab, position, quaternion.identity, _damageCanvas.transform);

		damagePrefab.GetComponent<TextMeshProUGUI>().text = "1";
	}
	
	
}