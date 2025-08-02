using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NotesUi : MonoBehaviour
{
  [SerializeField] private NoteSo[] notes;
  [SerializeField] private TextMeshProUGUI noteTitleText;
  [SerializeField] private TextMeshProUGUI noteContentText;
  [SerializeField] private Button nextButton;
  [SerializeField] private Button previousButton;

  private int currentNoteIndex = 0;

  private void Awake()
  {
    if (nextButton != null)
      nextButton.onClick.AddListener(ShowNextNote);

    if (previousButton != null)
      previousButton.onClick.AddListener(ShowPreviousNote);
  }

  private void Start()
  {
    if (notes.Length > 0)
    {
      ShowNote(currentNoteIndex);
    }
    else
    {
      noteTitleText.text = "No Notes Available";
      noteContentText.text = "";
    }
  }

  private void ShowNote(int index)
  {
    if (index < 0 || index >= notes.Length) return;

    noteTitleText.text = notes[index].title;
    noteContentText.text = $"Date: {notes[index].date}\n\n" +
                           $"{notes[index].content}";
  }

  private void ShowNextNote()
  {
    if (notes.Length == 0) return;

    currentNoteIndex = (currentNoteIndex + 1) % notes.Length;
    ShowNote(currentNoteIndex);
  }

  private void ShowPreviousNote()
  {
    if (notes.Length == 0) return;

    currentNoteIndex = (currentNoteIndex - 1 + notes.Length) % notes.Length;
    ShowNote(currentNoteIndex);
  }
}
