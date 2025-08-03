using UnityEngine;

public class GameEndSceneManager : MonoBehaviour
{
    public static GameEndSceneManager Instance { get; private set; }

    [SerializeField] private DialogueNodeSo gameEndDialogueNode;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        SoundManager.PlaySound(SoundType.BathedLight);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GameManager.Instance.TriggerDialogue(gameEndDialogueNode);
        }
    }
}