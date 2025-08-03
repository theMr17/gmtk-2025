using System;
using UnityEngine;

public class BoxSceneManager : MonoBehaviour
{
  public static BoxSceneManager Instance { get; private set; }

  [SerializeField] private InteractableObject exitAreaObject;

  private void Start()
  {
    exitAreaObject.button.onClick.AddListener(() => HandleExitAreaInteraction());
    DayTimeManager.Instance.OnMinuteChanged += DayTimeManager_OnMinuteChanged;
  }

  private void DayTimeManager_OnMinuteChanged(object sender, DayTimeManager.TimeChangedEventArgs e)
  {

  }

  private void Awake()
  {
    Instance = this;
  }

  private void HandleExitAreaInteraction()
  {
    GameManager.Instance.TriggerDialogue(exitAreaObject.dialogueNode);

    EventHandler<DialogueManager.OptionsChosenEventArgs> onMoveCompleteOptions = null;
    onMoveCompleteOptions = (sender, e) =>
    {
      DialogueManager.Instance.OnOptionsChosen -= onMoveCompleteOptions;

      if (e.selectedOptionIndex == 0) // Index 0 is the "Leave" option
      {
        SceneLoader.Instance.LoadScene(SceneLoader.Scene.StorageScene);
      }
    };

    DialogueManager.Instance.OnOptionsChosen += onMoveCompleteOptions;
  }

  private void OnDestroy()
  {
    if (DayTimeManager.Instance != null)
    {
      DayTimeManager.Instance.OnMinuteChanged -= DayTimeManager_OnMinuteChanged;
    }
  }
}