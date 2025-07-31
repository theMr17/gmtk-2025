using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class DialogueUi : MonoBehaviour
{
  [SerializeField] private Image _characterBustImage;
  [SerializeField] private TextMeshProUGUI _characterNameText;
  [SerializeField] private TextMeshProUGUI _dialogueText;

  [SerializeField] private Transform _choicesContainer;
  [SerializeField] private Button _choiceButtonPrefab;

  private void Start()
  {
    Hide();

    DialogueManager.Instance.OnDialogueStart += DialogueManager_OnDialogueStart;
    DialogueManager.Instance.OnDialogueChange += DialogueManager_OnDialogueChange;
    DialogueManager.Instance.OnDialogueEnd += DialogueManager_OnDialogueEnd;
    DialogueManager.Instance.OnChoicesAdded += DialogueManager_OnChoicesAdded;
  }

  private void DialogueManager_OnDialogueStart(object sender, DialogueManager.DialogueEventArgs e)
  {
    Show();
    UpdateDialogue(e.characterName, e.characterBust, e.dialogue);
  }

  private void DialogueManager_OnDialogueChange(object sender, DialogueManager.DialogueEventArgs e)
  {
    UpdateDialogue(e.characterName, e.characterBust, e.dialogue);
  }

  private void DialogueManager_OnDialogueEnd(object sender, System.EventArgs e)
  {
    Hide();
  }

  private void DialogueManager_OnChoicesAdded(object sender, DialogueManager.ChoicesEventArgs e)
  {
    // Clear existing choices
    foreach (Transform child in _choicesContainer)
    {
      Destroy(child.gameObject);
    }

    // Instantiate choice buttons based on the provided choice text
    foreach (var choice in e.choices)
    {
      Button choiceButton = Instantiate(_choiceButtonPrefab, _choicesContainer);
      choiceButton.GetComponentInChildren<TextMeshProUGUI>().text = choice;
      choiceButton.onClick.AddListener(() =>
      {
        // DialogueManager.Instance.ChooseOption(choice);
        Hide();
      });
    }
  }

  private void Show()
  {
    gameObject.SetActive(true);
  }

  private void Hide()
  {
    gameObject.SetActive(false);
  }

  private void UpdateDialogue(string characterName, Sprite characterBust, string dialogue)
  {
    _characterNameText.text = characterName;
    _characterBustImage.sprite = characterBust;
    _dialogueText.text = dialogue;
  }
}
