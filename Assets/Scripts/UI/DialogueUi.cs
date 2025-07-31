using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueUi : MonoBehaviour
{
  [SerializeField] private Image _characterBustImage;
  [SerializeField] private TextMeshProUGUI _characterNameText;
  [SerializeField] private TextMeshProUGUI _dialogueText;

  [SerializeField] private Transform _choicesContainer;
  [SerializeField] private Button _choiceButtonPrefab;

  private void Start()
  {
    DialogueManager.Instance.OnDialogueStart += DialogueManager_OnDialogueStart;
    DialogueManager.Instance.OnDialogueChange += DialogueManager_OnDialogueChange;
    DialogueManager.Instance.OnDialogueEnd += DialogueManager_OnDialogueEnd;
    DialogueManager.Instance.OnChoicesAdded += DialogueManager_OnChoicesAdded;

    Hide();
  }

  private void DialogueManager_OnDialogueStart(object sender, DialogueManager.DialogueEventArgs e)
  {
    Show();
    UpdateDialogue(e.characterName, e.characterBust, e.dialogue);
    HideChoices();
  }

  private void DialogueManager_OnDialogueChange(object sender, DialogueManager.DialogueEventArgs e)
  {
    UpdateDialogue(e.characterName, e.characterBust, e.dialogue);
    HideChoices();
  }

  private void DialogueManager_OnDialogueEnd(object sender, System.EventArgs e)
  {
    Hide();
  }

  private void DialogueManager_OnChoicesAdded(object sender, DialogueManager.ChoicesEventArgs e)
  {
    ShowChoices(e.choices);
  }

  private void ShowChoices(string[] choices)
  {
    // Clear existing choices
    foreach (Transform child in _choicesContainer)
    {
      Destroy(child.gameObject);
    }

    // Show choices container
    _choicesContainer.gameObject.SetActive(true);

    // Instantiate choice buttons with proper index tracking 
    for (int i = 0; i < choices.Length; i++)
    {
      string choiceText = choices[i];
      int choiceIndex = i;
      Button choiceButton = Instantiate(_choiceButtonPrefab, _choicesContainer);
      choiceButton.GetComponentInChildren<TextMeshProUGUI>().text = choiceText;

      // Choice selection
      choiceButton.onClick.AddListener(() =>
      {
        DialogueManager.Instance.ChooseOption(choiceIndex);
      });
    }
  }

  private void HideChoices()
  {
    // Clear Existing choices
    foreach (Transform child in _choicesContainer)
    {
      Destroy(child.gameObject);
    }

    _choicesContainer.gameObject.SetActive(false);
  }

  private void Show()
  {
    gameObject.SetActive(true);
  }

  private void Hide()
  {
    gameObject.SetActive(false);
    HideChoices();
  }

  private void UpdateDialogue(string characterName, Sprite characterBust, string dialogue)
  {
    _characterNameText.text = characterName;
    _characterBustImage.sprite = characterBust;
    _dialogueText.text = dialogue;
  }

  private void OnDestroy()
  {
    DialogueManager.Instance.OnDialogueStart -= DialogueManager_OnDialogueStart;
    DialogueManager.Instance.OnDialogueChange -= DialogueManager_OnDialogueChange;
    DialogueManager.Instance.OnDialogueEnd -= DialogueManager_OnDialogueEnd;
    DialogueManager.Instance.OnChoicesAdded -= DialogueManager_OnChoicesAdded;
  }
}
