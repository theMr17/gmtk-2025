using UnityEngine;

public class GameEndSceneManager : MonoBehaviour
{
    public static GameEndSceneManager Instance { get; private set; }

    [SerializeField] private DialogueNodeSo gameEndDialogueNode;
    private bool isDialogueTriggered = false;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        SoundManager.StopSound();
        SoundManager.PlaySound(SoundType.BathedLight);
    }

    private void Update()
    {
        if (!isDialogueTriggered)
        {
            GameManager.Instance.TriggerDialogue(gameEndDialogueNode);
            isDialogueTriggered = true;
        }
    }
}