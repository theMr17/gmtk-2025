using UnityEngine;

[CreateAssetMenu()]
public class GameStateSo : ScriptableObject
{
  [Tooltip("Current day in the game.")]
  public int Day = 1;

  [Tooltip("Number of times the calendar has been viewed.")]
  public int CalViewed = 0;

  [Tooltip("Number of times the TV has been viewed.")]
  public int TvViewed = 0;

  [Tooltip("Is the game currently in Free Roam mode?")]
  public bool FreeRoam = false;

  [Tooltip("Number of times the character has been viewed in the corner.")]
  public int CorViewed = 0;

  [Tooltip("Number of times the accident has occurred in the lab.")]
  public int LabAccident = 0;
}
