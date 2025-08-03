using UnityEngine;

public class EntranceHallSceneManager : MonoBehaviour
{
    public static EntranceHallSceneManager Instance { get; private set; }

    [SerializeField] private InteractableObject ventObject;
    [SerializeField] private InteractableObject exitAreaObject;
    [SerializeField] private InteractableObject guardObject;

    [SerializeField] private DialogueNodeSo entranceHallIntroDialogueNode;
    [SerializeField] private DialogueNodeSo wardenInteractionDialogueNode;

    private bool wardenInteractionTriggered = false;

    private void Start()
    {
        ventObject.button.onClick.AddListener(() => HandleVentInteraction());
        exitAreaObject.button.onClick.AddListener(() => HandleExitAreaInteraction());
        guardObject.button.onClick.AddListener(() => HandleGuardInteraction());

        DayTimeManager.Instance.OnHourChanged += DayTimeManager_OnHourChanged;

        if (DayTimeManager.Instance.Hour >= 16 && !wardenInteractionTriggered)
        {
            HandleWardenInteraction();
        }
    }

    private void DayTimeManager_OnHourChanged(object sender, DayTimeManager.HourChangedEventArgs e)
    {
        if (e.Hour >= 16 && !wardenInteractionTriggered)
        {
            HandleWardenInteraction();
        }
    }

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (GameManager.Instance.gameState.HallViewed < 1)
        {
            GameManager.Instance.gameState.HallViewed++;
            GameManager.Instance.TriggerDialogue(entranceHallIntroDialogueNode);
        }
    }

    private void HandleVentInteraction()
    {
        SoundManager.PlaySound(SoundType.Vent);
        GameManager.Instance.TriggerDialogue(ventObject.dialogueNode);
    }

    private void HandleExitAreaInteraction()
    {
        SoundManager.PlaySound(SoundType.Door);
        SceneLoader.Instance.LoadScene(SceneLoader.Scene.CorridorScene);
    }

    private void HandleGuardInteraction()
    {
        GameManager.Instance.TriggerDialogue(guardObject.dialogueNode);
    }

    private void HandleWardenInteraction()
    {
        wardenInteractionTriggered = true;
        GameManager.Instance.TriggerDialogue(wardenInteractionDialogueNode);
    }

    private void OnDestroy()
    {
        if (DayTimeManager.Instance != null)
        {
            DayTimeManager.Instance.OnHourChanged -= DayTimeManager_OnHourChanged;
        }
    }
}