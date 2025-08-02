using TMPro;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UI;

public class EmailUi : MonoBehaviour
{
    [SerializeField] private EmailSo[] emails;
    [SerializeField] private TextMeshProUGUI emailDate;
    [SerializeField] private TextMeshProUGUI emailContent;
    [SerializeField] private Button nextButton;
    [SerializeField] private Button previousButton;

    private int currentEmailIndex = 0;

    private void Awake()
    {
        if (nextButton != null) nextButton.onClick.AddListener(ShowNextEmail);

        if (previousButton != null) previousButton.onClick.AddListener(ShowPreviousEmail);
    }

    private void Start()
    {
        if (emails.Length > 0)
        {
            ShowEmail(currentEmailIndex);
        }
        else
        {
            emailDate.text = "No Emails Available.";
            emailContent.text = "";
        }
    }

    private void ShowEmail(int index)
    {
        if (index < 0 || index >= emails.Length) return;

        emailDate.text = emails[index].date;

        emailContent.text = $"From: {emails[index].sender}"
        + $"To: {emails[index].recipient}\n\n"
        + $"RE: {emails[index].subject}\n\n"
        + $"{emails[index].content}";
    }

    private void ShowNextEmail()
    {
        if (emails.Length == 0) return;

        currentEmailIndex = (currentEmailIndex + 1) % emails.Length;
    }

    private void ShowPreviousEmail()
    {
        if (emails.Length == 0) return;

        currentEmailIndex = (currentEmailIndex - 1 + emails.Length) % emails.Length;
        ShowEmail(currentEmailIndex);
    }
}