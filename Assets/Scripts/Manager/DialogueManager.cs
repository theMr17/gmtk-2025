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

  private Queue<DialogueLine> dialogueQueue;
  private List<DialogueOption> dialogueOptions = new List<DialogueOption>();
  private DialogueLine currentLine;
  private bool isDialogueActive = false;

  private DialogueNodeSo _currentNode;

  private void Awake()
  {
    if (Instance == null)
    {
      Instance = this;
      DontDestroyOnLoad(gameObject);
      dialogueQueue = new Queue<DialogueLine>();
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

  private void GameManager_OnDialogueTriggered(object sender, GameManager.DialogueTriggeredEventArgs e)
  {
    StartDialogueNode(e.dialogueNode);
  }

  public void StartDialogueNode(DialogueNodeSo dialogueNode)
  {
    _currentNode = dialogueNode;

    if (_currentNode == null || _currentNode.dialogueLines.Length == 0)
    {
      ShowOptions();
      return;
    }

    // Check conditions for the current node
    if (!_currentNode.condition.IsMet(GameManager.Instance.GetComponent<GameState>()))
    {
      EndDialogueNode();
      return;
    }

    // Clear previous dialogue
    dialogueQueue.Clear();

    // Add lines to the queue
    foreach (var line in _currentNode.dialogueLines)
    {
      if (line.showLineCondition.IsMet(GameManager.Instance.GetComponent<GameState>()))
      {
        dialogueQueue.Enqueue(line);
      }
    }

    ShowNextLine(isDialogueStart: true);
  }

  void ShowNextLine(bool isDialogueStart = false)
  {
    if (dialogueQueue.Count == 0)
    {
      ShowOptions();
      return;
    }

    isDialogueActive = true;
    currentLine = dialogueQueue.Dequeue();

    string characterName = currentLine.character != null ? currentLine.character.characterName : "Unknown";
    Sprite characterBust = currentLine.character != null ? currentLine.character.characterBust : null;

    if (currentLine.hideCharacterIfConditionMet && currentLine.nameCondition.IsMet(GameManager.Instance.GetComponent<GameState>()))
    {
      characterName = "???";
      characterBust = null;
    }

    var eventArgs = new DialogueEventArgs
    {
      characterName = characterName,
      characterBust = characterBust,
      dialogue = currentLine.text
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
    Debug.Log("Showing options for dialogue node: " + _currentNode.name);

    for (int i = 0; i < _currentNode.options.Length; i++)
    {
      if (_currentNode.options[i].condition.IsMet(GameManager.Instance.GetComponent<GameState>()))
      {
        dialogueOptions.Add(_currentNode.options[i]);
      }
    }

    Debug.Log("Available options: " + dialogueOptions.Count);

    OnOptionsAdded?.Invoke(this, new OptionsEventArgs
    {
      options = dialogueOptions.ConvertAll(dialogueOption => dialogueOption.option).ToArray()
    });
  }

  public void ChooseOption(int optionIndex)
  {
    if (_currentNode == null || _currentNode.options == null || optionIndex < 0 || optionIndex >= dialogueOptions.Count)
    {
      EndDialogueNode();
      return;
    }

    DialogueNodeSo nextNode = dialogueOptions[optionIndex].nextNode;
    EndDialogueNode();

    // Check condition for the next node
    if (nextNode != null)
    {
      StartDialogueNode(nextNode);
    }
  }

  private void EndDialogueNode()
  {
    if (_currentNode.nextNode != null)
    {
      StartDialogueNode(_currentNode.nextNode);
      return;
    }

    isDialogueActive = false;
    _currentNode = null;
    dialogueQueue.Clear();
    dialogueOptions.Clear();
    OnDialogueEnd?.Invoke(this, EventArgs.Empty);
  }

  private void Update()
  {
    // Allow advance dialogue only if no choices 
    if (Input.GetMouseButtonDown(0))
    {
      ShowNextLine();
    }
  }
}
