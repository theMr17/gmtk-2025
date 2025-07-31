using System;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
  public static DialogueManager Instance { get; private set; }

  public event EventHandler<DialogueEventArgs> OnDialogueStart;
  public event EventHandler<DialogueEventArgs> OnDialogueChange;
  public event EventHandler OnDialogueEnd;
  public class DialogueEventArgs : EventArgs
  {
    public string characterName;
    public Sprite characterBust;
    public string dialogue;
  }

  public event EventHandler<ChoicesEventArgs> OnChoicesAdded;
  public class ChoicesEventArgs : EventArgs
  {
    public string[] choices;
  }

  private void Awake()
  {
    Instance = this;
    DontDestroyOnLoad(gameObject);
  }
}
