using System;
using UnityEngine;
using System.Collections.Generic;

public class DialogueManager : MonoBehaviour
{
  public static DialogueManager Instance { get; private set; }

  private GameObject interactionButtonsContainer;

  public event EventHandler<DialogueEventArgs> OnDialogueStart;
  public event EventHandler<DialogueEventArgs> OnDialogueChange;
  public event EventHandler OnDialogueEnd;

  public class DialogueEventArgs : EventArgs
  {
    public string characterName;
    public Sprite characterBust;
    public string dialogue;
    public bool isThought;
  }

  public event EventHandler<OptionsEventArgs> OnOptionsAdded;
  public class OptionsEventArgs : EventArgs
  {
    public string[] options;
  }

  public event EventHandler<OptionsChosenEventArgs> OnOptionsChosen;
  public class OptionsChosenEventArgs : EventArgs
  {
    public int selectedOptionIndex;
  }

  private DialogueNodeSo _currentNode;

  private Queue<DialogueLine> _dialogueQueue;
  private List<DialogueOption> _dialogueOptions = new();
  private bool _isDialogueActive = false;
  private bool _isShowingOptions = false;

  private OptionsChosenEventArgs _pendingOptionChosenArgs;

  private void Awake()
  {
    if (Instance == null)
    {
      Instance = this;
      DontDestroyOnLoad(gameObject);
      _dialogueQueue = new Queue<DialogueLine>();
    }
    else
    {
      Destroy(gameObject);
    }
  }

  private void Start()
  {
    interactionButtonsContainer = GameObject.Find("InteractionButtons");
    GameManager.Instance.OnDialogueTriggered += GameManager_OnDialogueTriggered;
  }

  private void Update()
  {
    if (interactionButtonsContainer == null)
    {
      interactionButtonsContainer = GameObject.Find("InteractionButtons");
    }
    if (Input.GetMouseButtonDown(0) && _isDialogueActive && !_isShowingOptions)
    {
      ShowNextLine();
    }
  }

  private void GameManager_OnDialogueTriggered(object sender, GameManager.DialogueTriggeredEventArgs e)
  {
    StartDialogueNode(e.dialogueNode);
  }

  public void StartDialogueNode(DialogueNodeSo dialogueNode)
  {
    Reset();

    _currentNode = dialogueNode;

    if (_currentNode == null || !_currentNode.showNodeCondition.IsMet(GameManager.Instance.gameState))
    {
      EndDialogueNode();
      return;
    }

    if (_currentNode.dialogueLines.Length == 0)
    {
      ShowOptions();
      return;
    }

    _dialogueQueue.Clear();

    foreach (var line in _currentNode.dialogueLines)
    {
      if (line.showLineCondition.IsMet(GameManager.Instance.gameState))
      {
        _dialogueQueue.Enqueue(line);
      }
    }

    DayTimeManager.Instance.PauseTime();

    ShowNextLine(isDialogueStart: true);
  }

  void ShowNextLine(bool isDialogueStart = false)
  {
    if (_dialogueQueue.Count == 0)
    {
      ShowOptions();
      return;
    }

    _isDialogueActive = true;
    DialogueLine currentLine = _dialogueQueue.Dequeue();

    string characterName = currentLine.character != null ? currentLine.character.characterName : "Unknown";
    Sprite characterBust = currentLine.character != null ? currentLine.character.characterBust : null;

    if (currentLine.hideCharacterIfConditionMet && currentLine.hideCharacterCondition.IsMet(GameManager.Instance.gameState))
    {
      characterName = "???";
      characterBust = null;
    }

    string dialogue = currentLine.text;

    // If textVariants is not null and has entries, randomly select one; otherwise use default text
    if (currentLine.textVariants != null && currentLine.textVariants.Length > 0)
    {
      int randomIndex = UnityEngine.Random.Range(0, currentLine.textVariants.Length + 1);
      // If randomIndex equals textVariants.Length, use line.text; else use the variant
      // This allows for a random selection of the default text or one of the variants
      dialogue = (randomIndex == currentLine.textVariants.Length) ? currentLine.text : currentLine.textVariants[randomIndex];
    }

    var eventArgs = new DialogueEventArgs
    {
      characterName = characterName,
      characterBust = characterBust,
      dialogue = dialogue,
      isThought = currentLine.character.name == "Thoughts"
    };

    if (isDialogueStart)
    {
      OnDialogueStart?.Invoke(this, eventArgs);
      interactionButtonsContainer.SetActive(false); // Hide interaction buttons at start
    }
    else
    {
      OnDialogueChange?.Invoke(this, eventArgs);
      interactionButtonsContainer.SetActive(false); // Hide interaction buttons at start
    }

    if (currentLine.action != DialogueActionType.None)
    {
      TriggerDialogueAction(currentLine.action);
    }
  }

  private void ShowOptions()
  {
    if (_currentNode.options == null || _currentNode.options.Length == 0)
    {
      EndDialogueNode();
      return;
    }

    _isShowingOptions = true;
    _dialogueOptions.Clear();

    for (int i = 0; i < _currentNode.options.Length; i++)
    {
      if (_currentNode.options[i].showOptionCondition.IsMet(GameManager.Instance.gameState))
      {
        _dialogueOptions.Add(_currentNode.options[i]);
      }
    }

    OnOptionsAdded?.Invoke(this, new OptionsEventArgs
    {
      options = _dialogueOptions.ConvertAll(dialogueOption => dialogueOption.option).ToArray()
    });
  }

  public void ChooseOption(int selectedOptionIndex)
  {
    if (_currentNode == null || _currentNode.options == null || selectedOptionIndex < 0 || selectedOptionIndex >= _dialogueOptions.Count)
    {
      EndDialogueNode();
      return;
    }

    if (_dialogueOptions[selectedOptionIndex].action != DialogueActionType.None)
    {
      TriggerDialogueAction(_dialogueOptions[selectedOptionIndex].action);
    }

    DialogueNodeSo nextNode = _dialogueOptions[selectedOptionIndex].nextNode;

    _pendingOptionChosenArgs = new OptionsChosenEventArgs
    {
      selectedOptionIndex = selectedOptionIndex
    };

    if (nextNode != null)
    {
      StartNextDialogueNode(nextNode);
      return;
    }

    EndDialogueNode();
  }

  private void StartNextDialogueNode(DialogueNodeSo nextNode)
  {
    if (nextNode == null)
    {
      EndDialogueNode();
      return;
    }

    Reset();

    StartDialogueNode(nextNode);
  }

  private void EndDialogueNode()
  {
    if (_currentNode != null && _currentNode.nextNode != null)
    {
      StartNextDialogueNode(_currentNode.nextNode);
      return;
    }

    Reset();

    OnDialogueEnd?.Invoke(this, EventArgs.Empty);
    interactionButtonsContainer.SetActive(true); // Show interaction buttons again

    // Defer OnOptionsChosen until after full cleanup
    if (_pendingOptionChosenArgs != null)
    {
      var args = _pendingOptionChosenArgs;
      _pendingOptionChosenArgs = null;

      OnOptionsChosen?.Invoke(this, args);
    }
  }

  private void Reset()
  {
    _isDialogueActive = false;
    _isShowingOptions = false;
    _currentNode = null;
    _dialogueQueue.Clear();
    _dialogueOptions.Clear();
  }

  private void TriggerDialogueAction(DialogueActionType action)
  {
    switch (action)
    {
      case DialogueActionType.EndDay:
        GameManager.Instance.EndDay();
        break;
      case DialogueActionType.EnableFreeRoam:
        GameManager.Instance.gameState.FreeRoam = true;
        break;
      case DialogueActionType.DisableFreeRoam:
        GameManager.Instance.gameState.FreeRoam = false;
        break;
      case DialogueActionType.SendToRoom:
        GameManager.Instance.SendPlayerToRoom();
        break;
      case DialogueActionType.SendToCorridor:
        GameManager.Instance.SendPlayerToCorridor();
        break;
      default:
        break;
    }
  }
}
