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
    DialogueManager.Instance.OnOptionsAdded += DialogueManager_OnOptionsAdded;

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

  private void DialogueManager_OnOptionsAdded(object sender, DialogueManager.OptionsEventArgs e)
  {
    ShowChoices(e.options);
  }

  private void ShowChoices(string[] options)
  {
    // Clear existing choices
    foreach (Transform child in _choicesContainer)
    {
      Destroy(child.gameObject);
    }

    // Show choices container
    _choicesContainer.gameObject.SetActive(true);

    // Instantiate choice buttons with proper index tracking 
    for (int i = 0; i < options.Length; i++)
    {
      string optionText = options[i];
      int optionIndex = i;
      Button optionButton = Instantiate(_choiceButtonPrefab, _choicesContainer);
      optionButton.GetComponentInChildren<TextMeshProUGUI>().text = optionText;

      // Option selection
      optionButton.onClick.AddListener(() =>
      {
        DialogueManager.Instance.ChooseOption(optionIndex);
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
    DialogueManager.Instance.OnOptionsAdded -= DialogueManager_OnOptionsAdded;
  }
}
