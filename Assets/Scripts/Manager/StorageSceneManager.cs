using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.XR;

public class StorageSceneManager : MonoBehaviour
{
  public static StorageSceneManager Instance { get; private set; }

  [SerializeField] private DialogueNodeSo storageIntroDialogueNode;


  [SerializeField] private InteractableObject staffRecordsObject;
  [SerializeField] private InteractableObject ventObject;
  [SerializeField] private InteractableObject LadderObject;
  [SerializeField] private InteractableObject boxesObject;
  [SerializeField] private InteractableObject exitAreaObject;

  [SerializeField] private DialogueNodeSo noLadderVentDialogueNode;
  [SerializeField] private DialogueNodeSo pastTimeVentDialogueNode;
  [SerializeField] private DialogueNodeSo alreadyEnteredBoxesDialogueNode;
  [SerializeField] private DialogueNodeSo afterTimerBoxesDialogueNode;

  [SerializeField] private DialogueNodeSo boxInteractionDialogueNode;
  [SerializeField] private DialogueNodeSo bx001DialogueNode;
  [SerializeField] private DialogueNodeSo bxEmptyDialogueNode;
  [SerializeField] private DialogueNodeSo bxFullDialogueNode;
  [SerializeField] private DialogueNodeSo bxHalfDialogueNode;
  [SerializeField] private DialogueNodeSo bxLeaveDialogueNode;

  [SerializeField] private DialogueNodeSo bxMoveDialogueNode;
  [SerializeField] private DialogueNodeSo bxMoveFullDialogueNode;
  [SerializeField] private DialogueNodeSo bxMoveHalfDialogueNode;

  private void Awake()
  {
    Instance = this;
  }

  private void Start()
  {
    staffRecordsObject.button.onClick.AddListener(() => HandleStaffRecordsInteraction());
    ventObject.button.onClick.AddListener(() => HandleVentInteraction());
    LadderObject.button.onClick.AddListener(() => HandleLadderInteraction());
    boxesObject.button.onClick.AddListener(() => HandleBoxesInteraction());
    exitAreaObject.button.onClick.AddListener(() => HandleExitAreaInteraction());
  }

  private void Update()
  {
    if (Input.GetKeyDown(KeyCode.Space) && GameManager.Instance.gameState.StorViewed < 1)
    {
      GameManager.Instance.gameState.StorViewed++;
      GameManager.Instance.TriggerDialogue(storageIntroDialogueNode);
    }
  }

  private void HandleStaffRecordsInteraction()
  {
    // Handle staff records interaction
    GameManager.Instance.TriggerDialogue(staffRecordsObject.dialogueNode);
  }

  private void HandleVentInteraction()
  {
    if (GameManager.Instance.gameState.LadderVent == 0)
    {
      GameManager.Instance.TriggerDialogue(noLadderVentDialogueNode);
    }
    else if (DayTimeManager.Instance.Hour >= 19 && DayTimeManager.Instance.Minute >= 45)
    {
      GameManager.Instance.TriggerDialogue(pastTimeVentDialogueNode);
    }
    else
    {
      SceneLoader.Instance.LoadScene(SceneLoader.Scene.VentScene);
    }
  }

  private void HandleLadderInteraction()
  {
    GameManager.Instance.TriggerDialogue(LadderObject.dialogueNode);
    GameManager.Instance.gameState.LadderVent++;
  }

  private void HandleBoxesInteraction()
  {
    if (GameManager.Instance.gameState.EnteredBox != 0)
    {
      GameManager.Instance.TriggerDialogue(alreadyEnteredBoxesDialogueNode);
    }
    else if (DayTimeManager.Instance.Hour >= 19 && DayTimeManager.Instance.Minute >= 45)
    {
      GameManager.Instance.TriggerDialogue(afterTimerBoxesDialogueNode);
    }
    else
    {
      StartBoxMiniGame();
    }
  }

  private void StartBoxMiniGame()
  {
    GameManager.Instance.TriggerDialogue(boxInteractionDialogueNode);
  }

  private void HandleExitAreaInteraction()
  {
    SceneLoader.Instance.LoadScene(SceneLoader.Scene.CorridorScene);
  }
}