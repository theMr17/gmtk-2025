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
  [SerializeField] private DialogueNodeSo feelingSleepyDialogueNode;
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

  private void Start()
  {
    DayTimeManager.Instance.OnHourChanged += DayTimeManager_OnHourChanged;
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
    SceneLoader.Instance.LoadScene(SceneLoader.Scene.S04RoomScene);
  }

  private void DayTimeManager_OnHourChanged(object sender, DayTimeManager.HourChangedEventArgs e)
  {
    // Sleep time at 10 PM
    if (e.Hour == 22)
    {
      TriggerDialogue(feelingSleepyDialogueNode);
    }
  }

  private void OnDestroy()
  {
    DayTimeManager.Instance.OnHourChanged -= DayTimeManager_OnHourChanged;
  }

  public void SendPlayerToRoom()
  {
    gameState.FreeRoam = false;
    SceneLoader.Instance.LoadScene(SceneLoader.Scene.S04RoomScene);
  }
}
