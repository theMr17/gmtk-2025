using System;
using UnityEngine;

public class OfficeSceneManager : MonoBehaviour
{
  public static OfficeSceneManager Instance { get; private set; }

  [SerializeField] private DialogueNodeSo officeIntroDialogueNode;

  [SerializeField] private InteractableObject computerObject;
  [SerializeField] private InteractableObject coffeeMachineObject;
  [SerializeField] private InteractableObject bookShelfObject;
  [SerializeField] private InteractableObject exitAreaObject;

  [SerializeField] private TimeRange wardenTimeRange = new TimeRange { StartHour = 11, StartMinute = 0, EndHour = 16, EndMinute = 0 };

  public event EventHandler OnComputerOpened;

  private void Awake()
  {
    Instance = this;
  }

  private void Start()
  {
    computerObject.button.onClick.AddListener(() => HandleComputerInteraction());
    coffeeMachineObject.button.onClick.AddListener(() => HandleCoffeeMachineInteraction());
    bookShelfObject.button.onClick.AddListener(() => HandleBookShelfInteraction());
    exitAreaObject.button.onClick.AddListener(() => HandleExitAreaInteraction());
  }

  private void Update()
  {
    if (GameManager.Instance.gameState.OfficeViewed < 1)
    {
      GameManager.Instance.gameState.OfficeViewed++;
      GameManager.Instance.TriggerDialogue(officeIntroDialogueNode);
    }
  }

  private void HandleComputerInteraction()
  {
    OnComputerOpened?.Invoke(this, EventArgs.Empty);
  }

  private void HandleCoffeeMachineInteraction()
  {
    GameManager.Instance.TriggerDialogue(coffeeMachineObject.dialogueNode);
  }

  private void HandleBookShelfInteraction()
  {
    GameManager.Instance.TriggerDialogue(bookShelfObject.dialogueNode);
  }

  private void HandleExitAreaInteraction()
  {
    SceneLoader.Instance.LoadScene(SceneLoader.Scene.CorridorScene);
  }

  public bool IsWardenPresent()
  {
    int hour = DayTimeManager.Instance.Hour;
    int minute = DayTimeManager.Instance.Minute;

    return wardenTimeRange.Includes(hour, minute);
  }
}
