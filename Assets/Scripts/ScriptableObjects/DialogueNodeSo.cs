using UnityEngine;

[CreateAssetMenu(fileName = "New Dialogue Node", menuName = "Dialogue/Node")]
public class DialogueNodeSo : ScriptableObject
{
  [Tooltip("Lines of dialogue that will be shown in order if their 'Show Line Condition' is met.")]
  public DialogueLine[] dialogueLines;

  [Tooltip("Player choices presented after dialogue lines, if any.")]
  public DialogueOption[] options;

  [Tooltip("The next node to proceed to automatically after this node finishes, if no choices are shown.")]
  public DialogueNodeSo nextNode;

  [Tooltip("Condition required for this node to be shown. If not met, the node is skipped.")]
  public Condition showNodeCondition;
}

[System.Serializable]
public class DialogueLine
{
  [TextArea]
  [Tooltip("The text that will be displayed for this dialogue line.")]
  public string text;

  [Tooltip("Variants of the text that can be randomly selected. If empty, 'Text' is used. Note: Text is also used when choosing a random variant.")]
  public string[] textVariants;

  [Tooltip("The character who is speaking this line.")]
  public CharacterSo character;

  [Tooltip("Enable this to allow hiding the character based on a 'Hide Character Condition'.")]
  public bool hideCharacterIfConditionMet;

  [Tooltip("Condition to hide the character. Checked only if 'Hide Character If Condition Met' is enabled.")]
  public Condition hideCharacterCondition;

  [Tooltip("Condition required to show this line. If not met, this line is skipped.")]
  public Condition showLineCondition;

  [Tooltip("Action triggered when this line is shown.")]
  public DialogueActionType action = DialogueActionType.None;
}

[System.Serializable]
public class DialogueOption
{
  [Tooltip("The text shown for this player option.")]
  public string option;

  [Tooltip("The next dialogue node to transition to if this option is selected.")]
  public DialogueNodeSo nextNode;

  [Tooltip("Action triggered when this option is selected.")]
  public DialogueActionType action = DialogueActionType.None;

  [Tooltip("Condition required for this option to appear. If not met, this option is hidden.")]
  public Condition showOptionCondition;
}

[System.Serializable]
public class Condition
{
  [Tooltip("Minimum day number required. Set to -1 to ignore.")]
  public int minDay = -1;

  [Tooltip("Maximum day number allowed. Set to -1 to ignore.")]
  public int maxDay = -1;

  [Tooltip("If true, the condition requires the game to be in Free Roam mode.")]
  public bool requiresFreeRoam = false;

  [Tooltip("If true, the condition requires the game to NOT be in Free Roam mode.")]
  public bool negateFreeRoam = false;

  [Tooltip("If >= 0, the calendar must have been viewed this exact number of times.")]
  public int requiredCalViewed = -1;

  [Tooltip("If >= 0, the TV must have been viewed at least this many times.")]
  public int minTvViewed = -1;

  [Tooltip("If >= 0, the TV must have been viewed at most this many times.")]
  public int maxTvViewed = -1;

  [Tooltip("If >= 0, the number of lab accidents must be at least this many.")]
  public int minLabAccidents = -1;

  [Tooltip("If >= 0, the number of lab accidents must be at most this many.")]
  public int maxLabAccidents = -1;

  public bool IsMet(GameStateSo state)
  {
    if (minDay >= 0 && state.Day < minDay) return false;
    if (maxDay >= 0 && state.Day > maxDay) return false;
    if (requiredCalViewed >= 0 && state.CalViewed != requiredCalViewed) return false;
    if (minTvViewed >= 0 && state.TvViewed < minTvViewed) return false;
    if (maxTvViewed >= 0 && state.TvViewed > maxTvViewed) return false;
    if (minLabAccidents >= 0 && state.LabAccident < minLabAccidents) return false;
    if (maxLabAccidents >= 0 && state.LabAccident > maxLabAccidents) return false;
    if (requiresFreeRoam && !state.FreeRoam) return false;
    if (negateFreeRoam && state.FreeRoam) return false;
    return true;
  }
}

public enum DialogueActionType
{
  None,
  EndDay,
  EnableFreeRoam,
  DisableFreeRoam,
  SendToRoom,
  SendToCorridor,
}
