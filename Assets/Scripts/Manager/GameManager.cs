using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
  public static GameManager Instance { get; private set; }

  public event EventHandler<DialogueTriggeredEventArgs> OnDialogueTriggered;
  public class DialogueTriggeredEventArgs : EventArgs
  {
    public DialogueNodeSo dialogueNode;
  }

  [SerializeField] private DialogueNodeSo startDialogueNode;

  private void Awake()
  {
    if (Instance == null)
    {
      Instance = this;
      DontDestroyOnLoad(gameObject);
    }
    else
    {
      Destroy(gameObject);
    }
  }

  private void Update()
  {
    if (Input.GetKeyDown(KeyCode.D))
    {
      OnDialogueTriggered?.Invoke(this, new DialogueTriggeredEventArgs { dialogueNode = startDialogueNode });
    }
  }

  public void TriggerDialogue(DialogueNodeSo dialogueNode)
  {
    if (dialogueNode == null) return;

    OnDialogueTriggered?.Invoke(this, new DialogueTriggeredEventArgs { dialogueNode = dialogueNode });
  }

  public void EndDay()
  {
    GetComponent<GameState>().EndDay();
  }
}
