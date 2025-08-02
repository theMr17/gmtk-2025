using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProjectLogUi : MonoBehaviour
{
    [SerializeField] private ProjectLogSo[] logs;
    [SerializeField] private TextMeshProUGUI logContent;
    [SerializeField] private Button nextButton;
    [SerializeField] private Button previousButton;

    private int currentLogIndex = 0;

    private void Awake()
    {
        if (nextButton != null) nextButton.onClick.AddListener(ShowNextLog);

        if (previousButton != null) previousButton.onClick.AddListener(ShowPreviousLog);
    }

    private void Start()
    {
        if (logs.Length > 0)
        {
            ShowLog(currentLogIndex);
        }
        else
        {
            logContent.text = "No logs available.";
        }
    }

    private void ShowLog(int index)
    {
        if (index < 0 || index >= logs.Length) return;


        logContent.text = $"Log Entry: {logs[index].logEntry}\n"
        + $"Author: {logs[index].author}\n"
        + $"Facility: {logs[index].facility}\n"
        + $"Subject: {logs[index].experimentSubject}\n"
        + $"{logs[index].content}";
    }

    private void ShowNextLog()
    {
        if (logs.Length == 0) return;

        currentLogIndex = (currentLogIndex + 1) % logs.Length;
        ShowLog(currentLogIndex);
    }

    private void ShowPreviousLog()
    {
        if (logs.Length == 0) return;

        currentLogIndex = (currentLogIndex - 1 + logs.Length) % logs.Length;
        ShowLog(currentLogIndex);
    }
}