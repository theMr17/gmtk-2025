using UnityEngine;

[CreateAssetMenu(fileName = "NewProjectLog", menuName = "ComputerSystem/ProjectLog")]
public class ProjectLogSo : ScriptableObject
{
    [Tooltip("Entry number of the log.")]
    public string logEntry;

    [Tooltip("Who conducted the experiment.")]
    public string author;

    [Tooltip("Facility where experiment was held.")]
    public string facility;

    [Tooltip("Subject of the experiment.")]
    public string experimentSubject;

    [Tooltip("The content text of the log.")]
    [TextArea(15, 20)]
    public string content;
}