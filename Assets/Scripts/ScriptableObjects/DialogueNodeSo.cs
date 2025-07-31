using UnityEngine;

[CreateAssetMenu(fileName = "New Dialogue Node", menuName = "Dialogue/Node")]
public class DialogueNodeSo : ScriptableObject
{
  public DialogueLine[] dialogueLines;
  public DialogueOption[] options;
  public DialogueNodeSo nextNode;
  public Condition condition;
}

[System.Serializable]
public class DialogueLine
{
  [TextArea] public string text;
  public CharacterSo character;
  public bool hideCharacterIfConditionMet;
  public Condition nameCondition;
  public Condition showLineCondition;
}

[System.Serializable]
public class DialogueOption
{
  public string option;
  public DialogueNodeSo nextNode;
  public Condition condition;
}


[System.Serializable]
public class Condition
{
  public int minDay = -1;
  public int maxDay = -1;
  public bool requiresFreeRoam = false;
  public bool negateFreeRoam = false;
  public int requiredCalViewed = -1;

  public bool IsMet(GameState state)
  {
    if (minDay >= 0 && state.Day < minDay) return false;
    if (maxDay >= 0 && state.Day > maxDay) return false;
    if (requiredCalViewed >= 0 && state.CalViewed != requiredCalViewed) return false;
    if (requiresFreeRoam && !state.FreeRoam) return false;
    if (negateFreeRoam && state.FreeRoam) return false;
    return true;
  }
}

