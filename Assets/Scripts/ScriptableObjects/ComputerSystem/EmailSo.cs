using UnityEngine;

[CreateAssetMenu(fileName = "NewEmail", menuName = "ComputerSystem/Email")]
public class EmailSo : ScriptableObject
{
  [Tooltip("The subject of the email.")]
  public string subject;

  [Tooltip("The sender of the email.")]
  public string sender;

  [Tooltip("The recipient of the email.")]
  public string recipient;

  [Tooltip("The content of the email.")]
  [TextArea(15, 20)]
  public string content;

  [Tooltip("The date when the email was sent.")]
  public string date;

  [Tooltip("Indicates whether the email has been read.")]
  public bool isRead;

  [Tooltip("Indicates whether the email is password protected.")]
  public bool isPasswordProtected;

  [Tooltip("The password required to access the email, if it is password protected.")]
  public string password;
}