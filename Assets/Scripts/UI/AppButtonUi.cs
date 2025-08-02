using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AppButtonUi : MonoBehaviour
{
  [SerializeField] private TextMeshProUGUI appNameText;
  [SerializeField] private Image appIconImage;

  public void Initialize(AppSo app)
  {
    appNameText.text = app.appName;
    appIconImage.sprite = app.icon;
  }
}
