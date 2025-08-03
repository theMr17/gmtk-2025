using System;
using UnityEditor.SearchService;
using UnityEngine;

public class BoxSceneManager : MonoBehaviour
{
  public static BoxSceneManager Instance { get; private set; }

  [Header("Scene References")]
  [SerializeField] private InteractableObject exitAreaObject;
  [SerializeField] private DialogueNodeSo cargoArrivalDialogueNode;
  [SerializeField] private DialogueNodeSo timeUpDialogueNode;

  [Header("Dialogue Nodes")]
  [SerializeField] private DialogueNodeSo dl001DialogueNode;
  [SerializeField] private DialogueNodeSo dlFoundDialogueNode;
  [SerializeField] private DialogueNodeSo dlPassDialogueNode;
  [SerializeField] private DialogueNodeSo dlNotRightDialogueNode;
  [SerializeField] private DialogueNodeSo dlFailDialogueNode;
  [SerializeField] private DialogueNodeSo dlRightDialogueNode;
  [SerializeField] private DialogueNodeSo dlWrongDialogueNode;
  [SerializeField] private DialogueNodeSo dlWardenDialogueNode;
  [SerializeField] private DialogueNodeSo dlExitDialogueNode;

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
    if (e.Hour == 11 && e.Minute == 30 && gs.OrderCargo == 1)
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
    if (e.Hour == 19 && e.Minute == 45)
    {
      GameManager.Instance.TriggerDialogue(timeUpDialogueNode);
      EventHandler onTimeUpDialogueEnd = null;
      onTimeUpDialogueEnd = (sender2, args) =>
      {
        DialogueManager.Instance.OnDialogueEnd -= onTimeUpDialogueEnd;
        SceneLoader.Instance.LoadScene(SceneLoader.Scene.StorageScene);
      };
      DialogueManager.Instance.OnDialogueEnd += onTimeUpDialogueEnd;
    }
  }

  // Determines outcome after inspecting the box
  private void HandleBoxInspectionOutcome()
  {
    var gs = GameManager.Instance.gameState;

    if (gs.EnteredBox is 1 or 2 or 3 or 4 or 6 or 9 or 10 or 12)
    {
      GameManager.Instance.TriggerDialogue(dlFoundDialogueNode);
      DayTimeManager.Instance.ResumeTime(11, 45); // Resume time to 11:45
      // Send to room handled by DialogueManager
    }
    else if (gs.SelMove is 0 or 5 or 7)
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
              // SceneLoader.Instance.LoadScene(SceneLoader.Scene.GameEndScene);

            }
            else
            {
              GameManager.Instance.TriggerDialogue(dlWardenDialogueNode);
              DayTimeManager.Instance.ResumeTime(11, 45); // Resume time to 11:45
              // should send to room automatically by DialogueManager
            }
          };

          DialogueManager.Instance.OnDialogueEnd += onRightDialogueEnd;
        }
        else
        {
          GameManager.Instance.TriggerDialogue(dlWrongDialogueNode);
          // should send to corridor automatically by DialogueManager
          DayTimeManager.Instance.ResumeTime(11, 45); // Resume time to 11:45
        }
      };

      DialogueManager.Instance.OnDialogueEnd += onPassDialogueEnd;
    }
    else
    {
      GameManager.Instance.TriggerDialogue(dlNotRightDialogueNode);
      // should transition to fail dialogue automatically by DialogueManager
      DayTimeManager.Instance.ResumeTime(11, 45); // Resume time to 11:45
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
}
