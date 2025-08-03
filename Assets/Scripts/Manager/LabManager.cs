using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LabManager : MonoBehaviour
{
    public static LabManager Instance { get; private set; }

    [SerializeField] private DialogueNodeSo labIntroDialogueNode;

    [SerializeField] private InteractableObject exitAreaObject;
    [SerializeField] private InteractableObject syringeObject;
    [SerializeField] private InteractableObject labCoatObject;
    [SerializeField] private InteractableObject whiteBoardObject;
    [SerializeField] private InteractableObject specimenFridgeObject;
    [SerializeField] private InteractableObject chemicalsObject;
    [SerializeField] private InteractableObject computerObject;

    public event EventHandler OnComputerOpened;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        exitAreaObject.button.onClick.AddListener(() => HandleExitAreaInteraction());
        syringeObject.button.onClick.AddListener(() => HandleSyringeInteraction());
        labCoatObject.button.onClick.AddListener(() => HandleLabCoatInteraction());
        whiteBoardObject.button.onClick.AddListener(() => HandleWhiteBoardInteraction());
        specimenFridgeObject.button.onClick.AddListener(() => HandleSpecimenFridgeInteraction());
        chemicalsObject.button.onClick.AddListener(() => HandleChemicalInteraction());
        computerObject.button.onClick.AddListener(() => HandleComputerInteraction());
    }

    private void Update()
    {
        if (GameManager.Instance.gameState.LabViewed < 1)
        {
            GameManager.Instance.gameState.LabViewed++;
            GameManager.Instance.TriggerDialogue(labIntroDialogueNode);
        }
    }

    private void HandleComputerInteraction()
    {
        SoundManager.PlaySound(SoundType.ComputerOn);
        OnComputerOpened?.Invoke(this, EventArgs.Empty);
    }

    private void HandleExitAreaInteraction()
    {
        SoundManager.PlaySound(SoundType.Door);
        SceneLoader.Instance.LoadScene(SceneLoader.Scene.CorridorScene);
    }

    private void HandleSyringeInteraction()
    {
        GameManager.Instance.TriggerDialogue(syringeObject.dialogueNode);
    }

    private void HandleLabCoatInteraction()
    {
        GameManager.Instance.TriggerDialogue(labCoatObject.dialogueNode);
    }

    private void HandleWhiteBoardInteraction()
    {
        GameManager.Instance.TriggerDialogue(whiteBoardObject.dialogueNode);
    }

    private void HandleChemicalInteraction()
    {
        GameManager.Instance.TriggerDialogue(chemicalsObject.dialogueNode);
    }

    private void HandleSpecimenFridgeInteraction()
    {
        SoundManager.PlaySound(SoundType.DoorLocked);
        GameManager.Instance.TriggerDialogue(specimenFridgeObject.dialogueNode);
    }
}