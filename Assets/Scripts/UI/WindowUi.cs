using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WindowUi : MonoBehaviour
{
  public event EventHandler OnClose;

  [SerializeField] private Button closeButton;
  [SerializeField] private GameObject windowContent;
  [SerializeField] private TextMeshProUGUI titleText;

  private GameObject currentContent;

  private void Awake()
  {
    if (closeButton != null)
      closeButton.onClick.AddListener(Close);
  }

  public void Initialize(AppSo app)
  {
    if (app == null || app.appContentPrefab == null)
    {
      return;
    }

    titleText.text = app.appName;

    // Clear previous content
    foreach (Transform child in windowContent.transform)
    {
      Destroy(child.gameObject);
    }

    currentContent = Instantiate(app.appContentPrefab, windowContent.transform);
    windowContent.SetActive(true);
  }

  public void Close()
  {
    OnClose?.Invoke(this, EventArgs.Empty);

    // Optionally also clear UI
    if (currentContent != null)
    {
      Destroy(currentContent);
      currentContent = null;
    }

    windowContent.SetActive(false);
  }
}
