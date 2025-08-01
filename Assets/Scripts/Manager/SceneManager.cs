using UnityEngine;
using UnityEngine.UI;

public class SceneManager : MonoBehaviour
{
  public static SceneManager Instance { get; private set; }

  [System.Serializable]
  struct InteractableObject
  {
    public Button button;
    public DialogueNodeSo dialogueNode;
  }

  [SerializeField] private InteractableObject bedObject;
  [SerializeField] private InteractableObject calendarObject;
  [SerializeField] private InteractableObject doorObject;
  [SerializeField] private InteractableObject tvObject;
  [SerializeField] private InteractableObject musicPlayerObject;
  [SerializeField] private InteractableObject booksObject;

  private void Awake()
  {
    Instance = this;
  }

  private void Start()
  {
    bedObject.button.onClick.AddListener(() => GameManager.Instance.TriggerDialogue(bedObject.dialogueNode));
    calendarObject.button.onClick.AddListener(() => GameManager.Instance.TriggerDialogue(calendarObject.dialogueNode));
    doorObject.button.onClick.AddListener(() => GameManager.Instance.TriggerDialogue(doorObject.dialogueNode));
    // tvObject.button.onClick.AddListener(() => GameManager.Instance.TriggerDialogue(tvObject.dialogueNode));
    musicPlayerObject.button.onClick.AddListener(() => GameManager.Instance.TriggerDialogue(musicPlayerObject.dialogueNode));
    booksObject.button.onClick.AddListener(() => GameManager.Instance.TriggerDialogue(booksObject.dialogueNode));
  }
}
