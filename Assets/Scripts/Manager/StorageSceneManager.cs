using System;
using System.Collections.Generic;
using UnityEngine;

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
  [SerializeField] private DialogueNodeSo bxMoveHalfFalseDialogueNode;
  [SerializeField] private DialogueNodeSo bxMoveEmptyDialogueNode;

  public event EventHandler<OnBoxSelectionStartedEventArgs> OnBoxSelectionStarted;
  public class OnBoxSelectionStartedEventArgs : EventArgs
  {
    public List<int> excludedBoxIds;
    public bool isBoxMoveSelection = false;
  }

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

  private void HandleExitAreaInteraction()
  {
    SceneLoader.Instance.LoadScene(SceneLoader.Scene.CorridorScene);
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

    EventHandler<DialogueManager.OptionsChosenEventArgs> onOptionsChosenHandler = null;
    onOptionsChosenHandler = (sender, e) =>
    {
      DialogueManager.Instance.OnOptionsChosen -= onOptionsChosenHandler;

      if (e.selectedOptionIndex == 0) // Open Box
      {
        GameManager.Instance.TriggerDialogue(bx001DialogueNode);
        HandleBX001Selection();
      }
      // else: leave, do nothing
    };

    DialogueManager.Instance.OnOptionsChosen += onOptionsChosenHandler;
  }

  private void HandleBX001Selection()
  {
    // Compute boxes to exclude if needed (e.g., previously entered/stored)
    var excludedBoxIds = new List<int>();

    // Fire event to UI listeners to enable box selection
    OnBoxSelectionStarted?.Invoke(this, new OnBoxSelectionStartedEventArgs
    {
      excludedBoxIds = excludedBoxIds
    });
  }


  private void HandleBXEmpty()
  {
    EventHandler<DialogueManager.OptionsChosenEventArgs> onEmptyBoxOptions = null;
    onEmptyBoxOptions = (sender, e) =>
    {
      DialogueManager.Instance.OnOptionsChosen -= onEmptyBoxOptions;

      if (e.selectedOptionIndex == 0) // Enter
      {
        GameManager.Instance.gameState.EnteredBox = GameManager.Instance.gameState.SelOrg;
        GameManager.Instance.gameState.StoredBox = 0;
        GameManager.Instance.gameState.SelOrg = 0;
        EnterBox();
      }
      else // Leave
      {
        GameManager.Instance.gameState.SelOrg = 0;
      }
    };

    DialogueManager.Instance.OnOptionsChosen += onEmptyBoxOptions;
  }

  private void HandleBXFullOrHalf()
  {
    EventHandler<DialogueManager.OptionsChosenEventArgs> onFullHalfOptions = null;
    onFullHalfOptions = (sender, e) =>
    {
      DialogueManager.Instance.OnOptionsChosen -= onFullHalfOptions;

      if (e.selectedOptionIndex == 0) // Move bag
      {
        GameManager.Instance.TriggerDialogue(bxMoveDialogueNode);
        HandleBXMoveSelection();
      }
      else
      {
        GameManager.Instance.gameState.SelOrg = 0;
      }
    };

    DialogueManager.Instance.OnOptionsChosen += onFullHalfOptions;
  }

  private void HandleBXMoveSelection()
  {
    // Exclude current box (SelOrg) from valid move targets
    var excludedBoxIds = new List<int> { GameManager.Instance.gameState.SelOrg };

    // Trigger UI to show move target selection
    OnBoxSelectionStarted?.Invoke(this, new OnBoxSelectionStartedEventArgs
    {
      excludedBoxIds = excludedBoxIds,
      isBoxMoveSelection = true
    });
  }

  private void HandleBXMoveSuccess()
  {
    EventHandler<DialogueManager.OptionsChosenEventArgs> onMoveCompleteOptions = null;
    onMoveCompleteOptions = (sender, e) =>
    {
      DialogueManager.Instance.OnOptionsChosen -= onMoveCompleteOptions;

      if (e.selectedOptionIndex == 0) // Enter box
      {
        GameManager.Instance.gameState.EnteredBox = GameManager.Instance.gameState.SelOrg;
        GameManager.Instance.gameState.StoredBox = GameManager.Instance.gameState.SelMove;
        GameManager.Instance.gameState.SelOrg = 0;
        GameManager.Instance.gameState.SelMove = 0;
        EnterBox();
      }
      else // Leave
      {
        GameManager.Instance.TriggerDialogue(bxLeaveDialogueNode);
        ResetBoxVars();
      }
    };

    DialogueManager.Instance.OnOptionsChosen += onMoveCompleteOptions;
  }

  private void ResetBoxVars()
  {
    GameManager.Instance.gameState.SelOrg = 0;
    GameManager.Instance.gameState.SelMove = 0;
  }

  private void EnterBox()
  {
    SceneLoader.Instance.LoadScene(SceneLoader.Scene.BoxScene);
  }

  public void OnBoxSelectedFromUI(int boxId)
  {
    GameManager.Instance.gameState.SelOrg = boxId;

    if (boxId == 3 || boxId == 6)
    {
      GameManager.Instance.TriggerDialogue(bxEmptyDialogueNode);
      HandleBXEmpty();
    }
    else if (boxId == 8 || boxId == 11)
    {
      GameManager.Instance.TriggerDialogue(bxFullDialogueNode);
      HandleBXFullOrHalf();
    }
    else
    {
      GameManager.Instance.TriggerDialogue(bxHalfDialogueNode);
      HandleBXFullOrHalf();
    }
  }

  public void OnBoxMoveTargetSelectedFromUI(int boxId)
  {
    GameManager.Instance.gameState.SelMove = boxId;

    if (boxId == GameManager.Instance.gameState.SelOrg)
    {
      GameManager.Instance.TriggerDialogue(bxLeaveDialogueNode);
      ResetBoxVars();
      return;
    }

    if (boxId == 8 || boxId == 11)
    {
      GameManager.Instance.TriggerDialogue(bxMoveFullDialogueNode);
      ResetBoxVars();
    }
    else if (boxId == 3 || boxId == 6)
    {
      GameManager.Instance.TriggerDialogue(bxMoveEmptyDialogueNode);
      HandleBXMoveSuccess();
    }
    else
    {
      if (GameManager.Instance.gameState.SelOrg == 8 || GameManager.Instance.gameState.SelOrg == 11)
      {
        GameManager.Instance.TriggerDialogue(bxMoveHalfFalseDialogueNode);
        ResetBoxVars();
      }
      else
      {
        GameManager.Instance.TriggerDialogue(bxMoveEmptyDialogueNode);
        HandleBXMoveSuccess();
      }
    }
  }
}
