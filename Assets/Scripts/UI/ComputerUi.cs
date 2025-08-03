using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ComputerUi : MonoBehaviour
{
  [Header("App UI")]
  [SerializeField] private AppSo[] apps;
  [SerializeField] private GameObject appButtonPrefab;
  [SerializeField] private Transform appListContainer;

  [Header("UI Elements")]
  [SerializeField] private Button closeButton;
  [SerializeField] private GameObject desktop;
  [SerializeField] private GameObject computerWindow;
  [SerializeField] private GameObject enterPasswordPanel;

  [Header("Password Settings")]
  [SerializeField] private TMP_InputField passwordInputField;
  [SerializeField] private Button submitPasswordButton;
  [SerializeField] private TextMeshProUGUI passwordErrorText;
  [SerializeField] private string password;
  [SerializeField] private Button closeButton2;

  private WindowUi windowUi;

  private void Awake()
  {
    if (computerWindow != null)
      windowUi = computerWindow.GetComponent<WindowUi>();

    if (closeButton != null)
      closeButton.onClick.AddListener(Hide);

    if (closeButton2 != null)
      closeButton2.onClick.AddListener(Hide);

    if (submitPasswordButton != null)
      submitPasswordButton.onClick.AddListener(VerifyPassword);
  }

  private void Start()
  {
    if (OfficeSceneManager.Instance != null)
      OfficeSceneManager.Instance.OnComputerOpened += HandleComputerOpened;

    if (LabManager.Instance != null)
      LabManager.Instance.OnComputerOpened += HandleComputerOpened;

    if (windowUi != null)
      windowUi.OnClose += HandleAppClosed;

    Hide();
  }

  private void HandleComputerOpened(object sender, System.EventArgs e)
  {
    gameObject.SetActive(true);
    enterPasswordPanel.SetActive(true);
    desktop.SetActive(false);
    computerWindow.SetActive(false);
    passwordInputField.text = "";
    passwordErrorText.text = "";
  }

  private void VerifyPassword()
  {
    if (passwordInputField.text == password)
    {
      enterPasswordPanel.SetActive(false);
      ShowDesktop();
      PopulateAppList();
    }
    else
    {
      passwordErrorText.text = "Incorrect password. Please try again.";
    }
  }

  private void ShowDesktop()
  {
    desktop.SetActive(true);
    computerWindow.SetActive(false);
  }

  private void HandleAppClosed(object sender, System.EventArgs e)
  {
    ShowDesktop();
  }

  private void PopulateAppList()
  {
    foreach (Transform child in appListContainer)
    {
      Destroy(child.gameObject);
    }

    foreach (var app in apps)
    {
      if (!app.showInAppList) continue;

      GameObject appButton = Instantiate(appButtonPrefab, appListContainer);
      if (appButton.TryGetComponent(out AppButtonUi buttonUi))
        buttonUi.Initialize(app);

      if (appButton.TryGetComponent(out Button btn))
        btn.onClick.AddListener(() => OpenApp(app));
    }
  }

  private void OpenApp(AppSo app)
  {
    if (app == null || app.appContentPrefab == null)
      return;

    desktop.SetActive(false);
    computerWindow.SetActive(true);
    windowUi.Initialize(app);
  }

  private void Hide()
  {
    gameObject.SetActive(false);
  }
}
