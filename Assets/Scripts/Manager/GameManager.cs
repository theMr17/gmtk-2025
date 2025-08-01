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

  [SerializeField] private DialogueNodeSo[] startDialogueNodes;
  [SerializeField] public GameStateSo gameState;

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
      if (gameState.Day <= 2)
      {
        OnDialogueTriggered?.Invoke(this, new DialogueTriggeredEventArgs { dialogueNode = startDialogueNodes[gameState.Day - 1] });
      }
      else
      {
        OnDialogueTriggered?.Invoke(this, new DialogueTriggeredEventArgs { dialogueNode = startDialogueNodes[2] });
      }
    }
  }

  public void TriggerDialogue(DialogueNodeSo dialogueNode)
  {
    if (dialogueNode == null) return;

    OnDialogueTriggered?.Invoke(this, new DialogueTriggeredEventArgs { dialogueNode = dialogueNode });
  }

  public void EndDay()
  {
    DayTimeManager.Instance.EndDay();
  }
}
