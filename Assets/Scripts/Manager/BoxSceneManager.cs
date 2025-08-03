using System;
using UnityEngine;

public class BoxSceneManager : MonoBehaviour
{
  public static BoxSceneManager Instance { get; private set; }

  [Header("Scene References")]
  [SerializeField] private InteractableObject exitAreaObject;
  [SerializeField] private DialogueNodeSo cargoArrivalDialogueNode;

  [Header("Dialogue Nodes")]
  [SerializeField] private DialogueNodeSo dl001DialogueNode;       // DL001
  [SerializeField] private DialogueNodeSo dlFoundDialogueNode;     // DLFound
  [SerializeField] private DialogueNodeSo dlPassDialogueNode;      // DLPass
  [SerializeField] private DialogueNodeSo dlNotRightDialogueNode;  // DLNotRight
  [SerializeField] private DialogueNodeSo dlFailDialogueNode;      // DLFail
  [SerializeField] private DialogueNodeSo dlRightDialogueNode;     // DLRight
  [SerializeField] private DialogueNodeSo dlWrongDialogueNode;     // DLWrong
  [SerializeField] private DialogueNodeSo dlWardenDialogueNode;    // DLWarden
  [SerializeField] private DialogueNodeSo dlExitDialogueNode;      // DLExit

  private void Awake()
  {
    Instance = this;
  }

  private void Start()
  {
    exitAreaObject.button.onClick.AddListener(() => HandleExitAreaInteraction());
    DayTimeManager.Instance.OnMinuteChanged += DayTimeManager_OnMinuteChanged;
  }

  private void OnDestroy()
  {
    if (DayTimeManager.Instance != null)
    {
      DayTimeManager.Instance.OnMinuteChanged -= DayTimeManager_OnMinuteChanged;
    }
  }

  // Handles timed cargo events and inspection trigger
  private void DayTimeManager_OnMinuteChanged(object sender, DayTimeManager.TimeChangedEventArgs e)
  {
    var gs = GameManager.Instance.gameState;

    if (e.Hour == 11 && e.Minute == 15 && gs.OrderCargo == 1)
    {
      GameManager.Instance.TriggerDialogue(cargoArrivalDialogueNode);
    }
    else if (e.Hour == 11 && e.Minute == 30 && gs.OrderCargo == 1)
    {
      GameManager.Instance.TriggerDialogue(dl001DialogueNode);

      EventHandler onDialogueEnd = null;
      onDialogueEnd = (sender2, args) =>
      {
        DialogueManager.Instance.OnDialogueEnd -= onDialogueEnd;
        HandleBoxInspectionOutcome();
      };

      DialogueManager.Instance.OnDialogueEnd += onDialogueEnd;
    }
  }

  // Determines outcome after inspecting the box
  private void HandleBoxInspectionOutcome()
  {
    var gs = GameManager.Instance.gameState;

    if (gs.EnteredBox == 3 || gs.EnteredBox == 6)
    {
      GameManager.Instance.TriggerDialogue(dlFoundDialogueNode);
      // Send to room handled by DialogueManager
    }
    else if (gs.SelMove == 0 || gs.SelMove == 5 || gs.SelMove == 7)
    {
      GameManager.Instance.TriggerDialogue(dlPassDialogueNode);

      EventHandler onPassDialogueEnd = null;
      onPassDialogueEnd = (sender, e) =>
      {
        DialogueManager.Instance.OnDialogueEnd -= onPassDialogueEnd;

        if (gs.EnteredBox == 5)
        {
          GameManager.Instance.TriggerDialogue(dlRightDialogueNode);

          EventHandler onRightDialogueEnd = null;
          onRightDialogueEnd = (s2, e2) =>
          {
            DialogueManager.Instance.OnDialogueEnd -= onRightDialogueEnd;

            if (gs.LabAccident == 1)
            {
              GameManager.Instance.TriggerDialogue(dlExitDialogueNode);
              // SceneLoader.Instance.LoadScene(SceneLoader.Scene.GameEnd); // Successful escape
            }
            else
            {
              GameManager.Instance.TriggerDialogue(dlWardenDialogueNode);
              LockPlayer();
              SceneLoader.Instance.LoadScene(SceneLoader.Scene.S04RoomScene);
            }
          };

          DialogueManager.Instance.OnDialogueEnd += onRightDialogueEnd;
        }
        else
        {
          GameManager.Instance.TriggerDialogue(dlWrongDialogueNode);
          // SceneLoader.Instance.LoadScene(SceneLoader.Scene.CorridorScene); // Failed but not caught
        }
      };

      DialogueManager.Instance.OnDialogueEnd += onPassDialogueEnd;
    }
    else
    {
      GameManager.Instance.TriggerDialogue(dlNotRightDialogueNode);

      EventHandler onNotRightDialogueEnd = null;
      onNotRightDialogueEnd = (sender, e) =>
      {
        DialogueManager.Instance.OnDialogueEnd -= onNotRightDialogueEnd;
        GameManager.Instance.TriggerDialogue(dlFailDialogueNode);
        DayTimeManager.Instance.ResumeTime(11, 45);
        // SceneLoader.Instance.LoadScene(SceneLoader.Scene.CorridorScene); // Almost caught
      };

      DialogueManager.Instance.OnDialogueEnd += onNotRightDialogueEnd;
    }
  }

  // Dialogue and logic when player chooses to exit the area
  private void HandleExitAreaInteraction()
  {
    GameManager.Instance.TriggerDialogue(exitAreaObject.dialogueNode);

    EventHandler<DialogueManager.OptionsChosenEventArgs> onMoveCompleteOptions = null;
    onMoveCompleteOptions = (sender, e) =>
    {
      DialogueManager.Instance.OnOptionsChosen -= onMoveCompleteOptions;

      if (e.selectedOptionIndex == 0) // Index 0 is the "Leave" option
      {
        GameManager.Instance.gameState.EnteredBox = 0;
        GameManager.Instance.gameState.StoredBox = 0;
        SceneLoader.Instance.LoadScene(SceneLoader.Scene.StorageScene);
      }
    };

    DialogueManager.Instance.OnOptionsChosen += onMoveCompleteOptions;
  }

  private void LockPlayer()
  {
    GameManager.Instance.gameState.FreeRoam = false;
  }
}
