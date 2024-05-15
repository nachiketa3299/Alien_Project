using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

public class CUIManager : MonoBehaviour
{
    [SerializeField] private GameObject actionSetPanel;

    [Header("Damage Pop Up")] [SerializeField]
    private GameObject _damagePrefab;

    [SerializeField] private GameObject _damageCanvas;

    [Header("Player UI")] [SerializeField] private Image _playerHP;
    [SerializeField] private Image _playerSP;
    [SerializeField] private Image _playerWeapon;

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

    public void SetPlayerHP(float ratio)
    {
        _playerHP.fillAmount = ratio;
    }
}