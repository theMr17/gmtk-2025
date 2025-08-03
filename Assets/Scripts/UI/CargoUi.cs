using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CargoUi : MonoBehaviour
{
  [Header("UI Elements")]
  [TextArea(3, 10)][SerializeField] private string cargoDetailsBeforeOrder;
  [TextArea(3, 10)][SerializeField] private string cargoDetailsAfterOrder;

  [SerializeField] private TextMeshProUGUI cargoDetailsText;
  [SerializeField] private Button reissueOrderButton;

  [Header("Password Protection")]
  [SerializeField] private GameObject enterPasswordPanel;
  [SerializeField] private TMP_InputField passwordInputField;
  [SerializeField] private Button submitPasswordButton;
  [SerializeField] private TextMeshProUGUI incorrectPasswordText;
  [SerializeField] private string password;

  private void Awake()
  {
    if (reissueOrderButton != null)
      reissueOrderButton.onClick.AddListener(ReissueOrder);

    if (submitPasswordButton != null)
      submitPasswordButton.onClick.AddListener(VerifyPassword);
  }

  private void Start()
  {
    enterPasswordPanel.SetActive(true);
    incorrectPasswordText.gameObject.SetActive(false);
  }

  private void VerifyPassword()
  {
    if (passwordInputField.text == password)
    {
      enterPasswordPanel.SetActive(false);
      incorrectPasswordText.gameObject.SetActive(false);
      ShowCargoContent();
    }
    else
    {
      incorrectPasswordText.text = "Incorrect password";
      incorrectPasswordText.gameObject.SetActive(true);
    }
  }

  private void ShowCargoContent()
  {
    bool isCargoOrdered = GameManager.Instance.gameState.OrderCargo == 1;
    if (isCargoOrdered)
    {
      cargoDetailsText.text = cargoDetailsAfterOrder;
      reissueOrderButton.gameObject.SetActive(false);
    }
    else
    {
      cargoDetailsText.text = cargoDetailsBeforeOrder;
      reissueOrderButton.gameObject.SetActive(true);
    }
  }

  private void ReissueOrder()
  {
    GameManager.Instance.gameState.OrderCargo = 1;
    cargoDetailsText.text = cargoDetailsAfterOrder;
    reissueOrderButton.gameObject.SetActive(false);
  }
}
