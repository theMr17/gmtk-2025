using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class DialogueSo : ScriptableObject
{
  public List<DialogueLine> lines;
}

[System.Serializable]
public class DialogueLine
{
  public CharacterSo character;
  [TextArea(2, 5)] public string message;
  public List<DialogueChoice> choices;
}

[System.Serializable]
public class DialogueChoice
{
  public string choiceText;
}
