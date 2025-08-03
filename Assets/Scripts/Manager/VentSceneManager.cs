using System;
using UnityEngine;

public class VentSceneManager : MonoBehaviour
{
  public static VentSceneManager Instance { get; private set; }

  [SerializeField] private DialogueNodeSo ventIntroDialogueNode;
  [SerializeField] private DialogueNodeSo meetingTimeDialogueNode;
  [SerializeField] private DialogueNodeSo lookingForSubjectDialogueNode;
  [SerializeField] private DialogueNodeSo meetingIntroDialogueNode;
  [SerializeField] private DialogueNodeSo meetingDialogueNode;
  [SerializeField] private DialogueNodeSo meetingWitnessDialogueNode;
  [SerializeField] private InteractableObject exitAreaObject;

  private void Start()
  {
    exitAreaObject.button.onClick.AddListener(() => HandleExitAreaInteraction());
    DayTimeManager.Instance.OnMinuteChanged += DayTimeManager_OnMinuteChanged;
  }

  private void DayTimeManager_OnMinuteChanged(object sender, DayTimeManager.TimeChangedEventArgs e)
  {
    if (e.Hour == 15 && e.Minute == 45 && GameManager.Instance.gameState.LearnMeeting == 1)
    {
      GameManager.Instance.TriggerDialogue(meetingTimeDialogueNode);
    }
    if (e.Hour == 16 && e.Minute == 00)
    {
      HandleMeeting();
    }
    if (e.Hour == 19 && e.Minute == 45)
    {
      GameManager.Instance.TriggerDialogue(lookingForSubjectDialogueNode);
    }
  }

  private void Awake()
  {
    Instance = this;
  }

  private void Update()
  {
    if (GameManager.Instance.gameState.VentViewed < 1)
    {
      GameManager.Instance.gameState.VentViewed++;
      GameManager.Instance.TriggerDialogue(ventIntroDialogueNode);
    }
  }

  private void HandleMeeting()
  {
    DialogueNodeSo dialogueNode = GameManager.Instance.gameState.LearnMeeting == 0 ? meetingIntroDialogueNode : meetingDialogueNode;
    if (GameManager.Instance.gameState.LearnMeeting == 0)
    {
      GameManager.Instance.gameState.LearnMeeting++;
      dialogueNode = meetingIntroDialogueNode;
    }
    else
    {
      dialogueNode = meetingDialogueNode;
    }

    GameManager.Instance.TriggerDialogue(dialogueNode);

    EventHandler onDialogueEndHandler = null;
    onDialogueEndHandler = (sender, e) =>
    {
      DialogueManager.Instance.OnDialogueEnd -= onDialogueEndHandler;

      DayTimeManager.Instance.ResumeTime(hours: 16, mins: 45);

      HandleMeetingWitnessDialogue();
    };
    DialogueManager.Instance.OnDialogueEnd += onDialogueEndHandler;
  }

  private void HandleMeetingWitnessDialogue()
  {
    if (GameManager.Instance.gameState.MeetingWitnessed == 0)
    {
      GameManager.Instance.gameState.MeetingWitnessed++;
      GameManager.Instance.TriggerDialogue(meetingWitnessDialogueNode);
    }
  }

  private void HandleExitAreaInteraction()
  {
    SceneLoader.Instance.LoadScene(SceneLoader.Scene.StorageScene);
  }

  private void OnDestroy()
  {
    if (DayTimeManager.Instance != null)
    {
      DayTimeManager.Instance.OnMinuteChanged -= DayTimeManager_OnMinuteChanged;
    }
  }
}