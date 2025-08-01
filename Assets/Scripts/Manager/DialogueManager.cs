using System;
using UnityEngine;
using System.Collections.Generic;

public class DialogueManager : MonoBehaviour
{
  public static DialogueManager Instance { get; private set; }

  public event EventHandler<DialogueEventArgs> OnDialogueStart;
  public event EventHandler<DialogueEventArgs> OnDialogueChange;
  public event EventHandler OnDialogueEnd;

  public class DialogueEventArgs : EventArgs
  {
    public string characterName;
    public Sprite characterBust;
    public string dialogue;
  }

  public event EventHandler<OptionsEventArgs> OnOptionsAdded;
  public class OptionsEventArgs : EventArgs
  {
    public string[] options;
  }

  private DialogueNodeSo _currentNode;

  private Queue<DialogueLine> _dialogueQueue;
  private List<DialogueOption> _dialogueOptions = new();
  private bool _isDialogueActive = false;
  private bool _isShowingOptions = false;

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
    GameManager.Instance.OnDialogueTriggered += GameManager_OnDialogueTriggered;
  }

  private void Update()
  {
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
    _currentNode = dialogueNode;

    if (_currentNode == null || !_currentNode.showNodeCondition.IsMet(GameManager.Instance.GetComponent<GameState>()))
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
      if (line.showLineCondition.IsMet(GameManager.Instance.GetComponent<GameState>()))
      {
        _dialogueQueue.Enqueue(line);
      }
    }

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

    if (currentLine.hideCharacterIfConditionMet && currentLine.hideCharacterCondition.IsMet(GameManager.Instance.GetComponent<GameState>()))
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
      dialogue = dialogue
    };

    if (isDialogueStart)
    {
      OnDialogueStart?.Invoke(this, eventArgs);
    }
    else
    {
      OnDialogueChange?.Invoke(this, eventArgs);
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
      if (_currentNode.options[i].showOptionCondition.IsMet(GameManager.Instance.GetComponent<GameState>()))
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

    DialogueNodeSo nextNode = _dialogueOptions[selectedOptionIndex].nextNode;

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

    _isDialogueActive = false;
    _isShowingOptions = false;
    _currentNode = null;
    _dialogueQueue.Clear();
    _dialogueOptions.Clear();

    StartDialogueNode(nextNode);
  }

  private void EndDialogueNode()
  {
    if (_currentNode != null && _currentNode.nextNode != null)
    {
      StartNextDialogueNode(_currentNode.nextNode);
      return;
    }

    _isDialogueActive = false;
    _isShowingOptions = false;
    _currentNode = null;
    _dialogueQueue.Clear();
    _dialogueOptions.Clear();

    OnDialogueEnd?.Invoke(this, EventArgs.Empty);
  }
}
