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

  public event EventHandler<ChoicesEventArgs> OnChoicesAdded;
  public class ChoicesEventArgs : EventArgs
  {
    public string[] choices;
  }

  private Queue<DialogueLine> dialogueQueue;
  private DialogueLine currentLine;
  private bool isDialogueActive = false;

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
    StartDialogue(e.dialogue);
  }

  public void StartDialogue(DialogueSo dialogue)
  {
    if (dialogue == null || dialogue.lines == null || dialogue.lines.Count == 0)
    {
      return;
    }

    isDialogueActive = true;
    dialogueQueue.Clear();

    foreach (DialogueLine line in dialogue.lines) dialogueQueue.Enqueue(line);

    ShowNextLine(isDialogueStart: true);
  }

  public void ShowNextLine(bool isDialogueStart = false)
  {
    if (dialogueQueue.Count == 0)
    {
      EndDialogue();
      return;
    }

    currentLine = dialogueQueue.Dequeue();

    string characterName = currentLine.character != null ? currentLine.character.characterName : "Unknown";
    Sprite characterBust = currentLine.character != null ? currentLine.character.characterBust : null;

    var eventArgs = new DialogueEventArgs
    {
      characterName = characterName,
      characterBust = characterBust,
      dialogue = currentLine.message
    };

    // Trigger event accordingly 
    if (isDialogueStart)
    {
      OnDialogueStart?.Invoke(this, eventArgs);
    }
    else
    {
      OnDialogueChange?.Invoke(this, eventArgs);
    }

    if (currentLine.choices != null && currentLine.choices.Count > 0)
    {
      string[] choiceTexts = new string[currentLine.choices.Count];
      for (int i = 0; i < currentLine.choices.Count; i++)
      {
        choiceTexts[i] = currentLine.choices[i].choiceText;
      }
      OnChoicesAdded?.Invoke(this, new ChoicesEventArgs { choices = choiceTexts });
    }
  }

  public void ChooseOption(int choiceIndex)
  {
    if (currentLine != null && currentLine.choices != null && choiceIndex >= 0 && choiceIndex < currentLine.choices.Count)
    {
      // --------------------------------
      // TODO: Handle choice consequences
      // --------------------------------

      ShowNextLine();
    }
  }

  private void EndDialogue()
  {
    isDialogueActive = false;
    currentLine = null;
    OnDialogueEnd?.Invoke(this, EventArgs.Empty);
  }

  public bool IsDialogueActive()
  {
    return isDialogueActive;
  }

  public bool HasChoices()
  {
    return currentLine != null && currentLine.choices != null && currentLine.choices.Count > 0;
  }

  private void Update()
  {
    // Allow advance dialogue only if no choices 
    if (Input.GetMouseButtonDown(0) && isDialogueActive && !HasChoices())
    {
      ShowNextLine();
    }
  }
}
