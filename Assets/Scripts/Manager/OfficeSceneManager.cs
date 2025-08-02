using UnityEngine;

public class OfficeSceneManager : MonoBehaviour
{
  public static OfficeSceneManager Instance { get; private set; }

  [SerializeField] private DialogueNodeSo officeIntroDialogueNode;

  [SerializeField] private InteractableObject coffeeMachineObject;
  [SerializeField] private InteractableObject bookShelfObject;

  [SerializeField] private TimeRange wardenTimeRange = new TimeRange { StartHour = 11, StartMinute = 0, EndHour = 16, EndMinute = 0 };

  private void Awake()
  {
    Instance = this;
  }

  private void Start()
  {
    coffeeMachineObject.button.onClick.AddListener(() => HandleCoffeeMachineInteraction());
    bookShelfObject.button.onClick.AddListener(() => HandleBookShelfInteraction());
  }

  private void Update()
  {
    if (Input.GetKeyDown(KeyCode.Space) && GameManager.Instance.gameState.OfficeViewed < 1)
    {
      GameManager.Instance.gameState.OfficeViewed++;
      GameManager.Instance.TriggerDialogue(officeIntroDialogueNode);
    }
  }

  private void HandleCoffeeMachineInteraction()
  {
    GameManager.Instance.TriggerDialogue(coffeeMachineObject.dialogueNode);
  }

  private void HandleBookShelfInteraction()
  {
    GameManager.Instance.TriggerDialogue(bookShelfObject.dialogueNode);
  }

  public bool IsWardenPresent()
  {
    int hour = DayTimeManager.Instance.Hour;
    int minute = DayTimeManager.Instance.Minute;

    return wardenTimeRange.Includes(hour, minute);
  }
}
