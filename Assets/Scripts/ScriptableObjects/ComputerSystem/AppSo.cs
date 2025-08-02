using UnityEngine;

[CreateAssetMenu(fileName = "NewApp", menuName = "ComputerSystem/App")]
public class AppSo : ScriptableObject
{
  [Tooltip("The name of the app as it appears on the UI.")]
  public string appName;

  [Tooltip("A brief description of what this app does.")]
  [TextArea(2, 5)]
  public string description;

  [Tooltip("The icon representing the app.")]
  public Sprite icon;

  [Tooltip("The GameObject that will be activated when this app is opened.")]
  public GameObject appContentPrefab;

  [Tooltip("Is the app password protected?")]
  public bool isPasswordProtected;

  [Tooltip("If protected, the password required to access this app.")]
  public string password;

  [Tooltip("Should this app appear on the desktop?")]
  public bool showInAppList = true;
}
