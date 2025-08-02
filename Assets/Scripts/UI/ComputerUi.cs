using UnityEngine;
using UnityEngine.UI;

public class ComputerUi : MonoBehaviour
{
  [SerializeField] private AppSo[] apps;
  [SerializeField] private GameObject appButtonPrefab;
  [SerializeField] private Transform appListContainer;
  [SerializeField] private Button closeButton;

  private void Start()
  {
    OfficeSceneManager.Instance.OnComputerOpened += OfficeSceneManager_OnComputerOpened;
    Hide();

    closeButton.onClick.AddListener(() => Hide());
  }

  private void OfficeSceneManager_OnComputerOpened(object sender, System.EventArgs e)
  {
    Show();
    InitializeComputerUi();
  }

  private void InitializeComputerUi()
  {
    // Clear existing app buttons
    foreach (Transform child in appListContainer)
    {
      Destroy(child.gameObject);
    }

    // Instantiate app buttons for each app that should be shown
    foreach (var app in apps)
    {
      if (app.showInAppList)
      {
        GameObject appButton = Instantiate(appButtonPrefab, appListContainer);
        appButton.GetComponent<AppButtonUi>().Initialize(app);

      }
    }
  }

  private void Show()
  {
    gameObject.SetActive(true);
  }

  private void Hide()
  {
    gameObject.SetActive(false);
  }
}
