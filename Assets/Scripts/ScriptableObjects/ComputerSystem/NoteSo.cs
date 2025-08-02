using UnityEngine;

[CreateAssetMenu(fileName = "NewNote", menuName = "ComputerSystem/Note")]
public class NoteSo : ScriptableObject
{
  [Tooltip("The title of the note.")]
  public string title;

  [Tooltip("The content of the note, which can be a longer text.")]
  [TextArea(15, 20)]
  public string content;

  [Tooltip("The date when the note was created or last modified.")]
  public string date;
}