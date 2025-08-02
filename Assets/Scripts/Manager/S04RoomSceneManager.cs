using System;
using UnityEngine;

public class S04RoomSceneManager : MonoBehaviour
{
  public static S04RoomSceneManager Instance { get; private set; }

  [SerializeField] private InteractableObject bedObject;
  [SerializeField] private InteractableObject calendarObject;
  [SerializeField] private InteractableObject doorObject;
  [SerializeField] private InteractableObject tvObject;
  [SerializeField] private InteractableObject musicPlayerObject;
  [SerializeField] private InteractableObject booksObject;

  private void Awake()
  {
    Instance = this;
  }

  private void Start()
  {
    bedObject.button.onClick.AddListener(() => HandleBedInteraction());
    calendarObject.button.onClick.AddListener(() => HandleCalendarInteraction());
    doorObject.button.onClick.AddListener(() => HandleDoorInteraction());
    tvObject.button.onClick.AddListener(() => HandleTvInteraction());
    musicPlayerObject.button.onClick.AddListener(() => HandleMusicPlayerInteraction());
    booksObject.button.onClick.AddListener(() => HandleBooksInteraction());
  }

  private void HandleBedInteraction()
  {
    GameManager.Instance.TriggerDialogue(bedObject.dialogueNode);
  }

  private void HandleCalendarInteraction()
  {
    GameManager.Instance.TriggerDialogue(calendarObject.dialogueNode);

    EventHandler onDialogueEndHandler = null;
    onDialogueEndHandler = (sender, e) =>
    {
      DialogueManager.Instance.OnDialogueEnd -= onDialogueEndHandler;

      if (GameManager.Instance.gameState.CalViewed is 0 or 2 or 4)
      {
        GameManager.Instance.gameState.CalViewed++;
      }
    };
    DialogueManager.Instance.OnDialogueEnd += onDialogueEndHandler;
  }

  private void HandleTvInteraction()
  {
    GameManager.Instance.TriggerDialogue(tvObject.dialogueNode);

    EventHandler onDialogueEndHandler = null;
    onDialogueEndHandler = (sender, e) =>
    {
      DialogueManager.Instance.OnDialogueEnd -= onDialogueEndHandler;

      if (GameManager.Instance.gameState.TvViewed is 0 or 2)
      {
        GameManager.Instance.gameState.TvViewed++;
      }
    };
    DialogueManager.Instance.OnDialogueEnd += onDialogueEndHandler;
  }

  private void HandleDoorInteraction()
  {
    if (GameManager.Instance.gameState.FreeRoam)
    {
      SceneLoader.Instance.LoadScene(SceneLoader.Scene.CorridorScene);
    }
    else
    {
      GameManager.Instance.TriggerDialogue(doorObject.dialogueNode);
    }
  }

  private void HandleMusicPlayerInteraction()
  {
    GameManager.Instance.TriggerDialogue(musicPlayerObject.dialogueNode);
  }

  private void HandleBooksInteraction()
  {
    GameManager.Instance.TriggerDialogue(booksObject.dialogueNode);
  }
}
