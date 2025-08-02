using UnityEngine;
using UnityEngine.UI;

public class ComputerUi : MonoBehaviour
{
  [SerializeField] private AppSo[] apps;
  [SerializeField] private GameObject appButtonPrefab;
  [SerializeField] private Transform appListContainer;
  [SerializeField] private Button closeButton;

  [SerializeField] private GameObject desktop;
  [SerializeField] private GameObject computerWindow;

  private WindowUi windowUi;

  private void Awake()
  {
    if (computerWindow != null)
    {
      windowUi = computerWindow.GetComponent<WindowUi>();
    }

    if (closeButton != null)
    {
      closeButton.onClick.AddListener(Hide);
    }
  }

  private void Start()
  {
    if (OfficeSceneManager.Instance != null)
      OfficeSceneManager.Instance.OnComputerOpened += HandleComputerOpened;

    if (windowUi != null)
      windowUi.OnClose += HandleAppClosed;
    Hide();
  }

  private void HandleComputerOpened(object sender, System.EventArgs e)
  {
    Show();
    PopulateAppList();
  }

  private void HandleAppClosed(object sender, System.EventArgs e)
  {
    desktop.SetActive(true);
    computerWindow.SetActive(false);
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
      {
        buttonUi.Initialize(app);
      }

      if (appButton.TryGetComponent(out Button btn))
      {
        btn.onClick.AddListener(() => OpenApp(app));
      }
    }
  }

  private void OpenApp(AppSo app)
  {
    if (app == null || app.appContentPrefab == null)
    {
      return;
    }

    desktop.SetActive(false);
    computerWindow.SetActive(true);
    windowUi.Initialize(app);
  }

  private void Show()
  {
    gameObject.SetActive(true);
    desktop.SetActive(true);
    computerWindow.SetActive(false);
  }

  private void Hide()
  {
    gameObject.SetActive(false);
  }
}
