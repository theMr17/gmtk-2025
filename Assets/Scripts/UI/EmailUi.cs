using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EmailUi : MonoBehaviour
{
    [Header("Email Data")]
    [SerializeField] private EmailSo[] emails;

    [Header("Email Display UI")]
    [SerializeField] private TextMeshProUGUI emailDate;
    [SerializeField] private TextMeshProUGUI emailContent;
    [SerializeField] private Button nextButton;
    [SerializeField] private Button previousButton;

    [Header("Password Panel UI")]
    [SerializeField] private GameObject passwordPanel;
    [SerializeField] private TMP_InputField passwordInputField;
    [SerializeField] private Button submitPasswordButton;
    [SerializeField] private TextMeshProUGUI passwordFeedbackText;

    private int currentEmailIndex = 0;

    private void Awake()
    {
        if (nextButton != null) nextButton.onClick.AddListener(ShowNextEmail);
        if (previousButton != null) previousButton.onClick.AddListener(ShowPreviousEmail);
        if (submitPasswordButton != null) submitPasswordButton.onClick.AddListener(CheckPassword);
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

        currentEmailIndex = index;

        if (emails[index].isPasswordProtected)
        {
            passwordPanel.SetActive(true);
            passwordInputField.text = "";
            passwordFeedbackText.text = "";
            emailDate.text = emails[index].sender;
            emailContent.text = "";
        }
        else
        {
            passwordPanel.SetActive(false);
            DisplayEmailContent(index);
        }
    }

    private void DisplayEmailContent(int index)
    {
        emailDate.text = emails[index].sender;
        emailContent.text = $"From: {emails[index].sender}\n\n" +
                            $"{emails[index].subject}\n\n" +
                            $"{emails[index].content}";
    }

    private void CheckPassword()
    {
        var currentEmail = emails[currentEmailIndex];
        if (passwordInputField.text == currentEmail.password)
        {
            passwordPanel.SetActive(false);
            DisplayEmailContent(currentEmailIndex);
        }
        else
        {
            passwordFeedbackText.text = "Incorrect password. Try again.";
        }
    }

    private void ShowNextEmail()
    {
        if (emails.Length == 0) return;
        currentEmailIndex = (currentEmailIndex + 1) % emails.Length;
        ShowEmail(currentEmailIndex);
    }

    private void ShowPreviousEmail()
    {
        if (emails.Length == 0) return;
        currentEmailIndex = (currentEmailIndex - 1 + emails.Length) % emails.Length;
        ShowEmail(currentEmailIndex);
    }
}
