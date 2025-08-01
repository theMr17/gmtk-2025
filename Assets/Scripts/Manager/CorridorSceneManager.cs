using UnityEngine;

public class CorridorSceneManager : MonoBehaviour
{
  public static CorridorSceneManager Instance { get; private set; }

  [SerializeField] private DialogueNodeSo corridorIntroDialogueNode;

  [SerializeField] private InteractableObject slipObject;
  [SerializeField] private DialogueNodeSo slipDialogueNodeGuardPresent;
  [SerializeField] private DialogueNodeSo slipDialogueNodeGuardAbsent;

  [SerializeField] private InteractableObject labDoorObject;
  [SerializeField] private DialogueNodeSo labDoorDialogueNodeGuardPresent;
  [SerializeField] private DialogueNodeSo labDoorDialogueNodeGuardAbsent;

  [SerializeField] private InteractableObject officeDoorObject;
  [SerializeField] private InteractableObject guardObject;

  [Header("Time Ranges")]
  [SerializeField] private TimeRange guardTimeRange1 = new TimeRange { StartHour = 8, StartMinute = 0, EndHour = 10, EndMinute = 0 };
  [SerializeField] private TimeRange guardTimeRange2 = new TimeRange { StartHour = 13, StartMinute = 0, EndHour = 14, EndMinute = 30 };
  [SerializeField] private TimeRange guardTimeRange3 = new TimeRange { StartHour = 15, StartMinute = 40, EndHour = 19, EndMinute = 0 };

  [SerializeField] private TimeRange slipTimeRange = new TimeRange { StartHour = 6, StartMinute = 0, EndHour = 13, EndMinute = 0 };

  private void Awake()
  {
    Instance = this;
  }

  private void Start()
  {
    slipObject.button.onClick.AddListener(() => HandleSlipInteraction());
    labDoorObject.button.onClick.AddListener(() => HandleLabDoorInteraction());
    officeDoorObject.button.onClick.AddListener(() => HandleOfficeDoorInteraction());
    guardObject.button.onClick.AddListener(() => HandleGuardInteraction());
  }

  private void Update()
  {
    if (Input.GetKeyDown(KeyCode.Space) && GameManager.Instance.gameState.CorViewed < 1)
    {
      GameManager.Instance.gameState.CorViewed++;
      GameManager.Instance.TriggerDialogue(corridorIntroDialogueNode);
    }
  }

  private void HandleSlipInteraction()
  {
    if (!IsSlipPresent())
    {
      return;
    }

    if (IsGuardPresent())
    {
      GameManager.Instance.TriggerDialogue(slipDialogueNodeGuardPresent);
    }
    else
    {
      GameManager.Instance.TriggerDialogue(slipDialogueNodeGuardAbsent);
    }
  }

  private void HandleLabDoorInteraction()
  {
    if (IsGuardPresent())
    {
      GameManager.Instance.TriggerDialogue(labDoorDialogueNodeGuardPresent);
    }
    else if (GameManager.Instance.gameState.LabAccident == 2)
    {
      GameManager.Instance.TriggerDialogue(labDoorDialogueNodeGuardAbsent);
    }
    else
    {
      // Enter Lab
    }
  }

  private void HandleOfficeDoorInteraction()
  {
    if (IsGuardPresent())
    {
      GameManager.Instance.TriggerDialogue(officeDoorObject.dialogueNode);
    }
    else
    {
      // Enter Office
    }
  }

  private void HandleGuardInteraction()
  {
    if (IsGuardPresent())
    {
      GameManager.Instance.TriggerDialogue(guardObject.dialogueNode);
    }
  }

  public bool IsGuardPresent()
  {
    int hour = DayTimeManager.Instance.Hour;
    int minute = DayTimeManager.Instance.Minute;

    int labAccident = GameManager.Instance.gameState.LabAccident;

    return (guardTimeRange1.Includes(hour, minute)
        || guardTimeRange2.Includes(hour, minute)
        || guardTimeRange3.Includes(hour, minute)) && (labAccident == 0 || labAccident == 2);
  }

  public bool IsSlipPresent()
  {
    int hour = DayTimeManager.Instance.Hour;
    int minute = DayTimeManager.Instance.Minute;

    return slipTimeRange.Includes(hour, minute);
  }
}
