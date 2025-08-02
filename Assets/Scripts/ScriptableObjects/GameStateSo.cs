using UnityEngine;

[CreateAssetMenu()]
public class GameStateSo : ScriptableObject
{
  [Tooltip("Current day in the game.")]
  public int Day = 1;

  [Tooltip("Number of times the calendar has been viewed.")]
  public int CalViewed = 0;

  [Tooltip("Number of times the hallway has been viewed.")]
  public int HallViewed = 0;

  [Tooltip("Number of times the TV has been viewed.")]
  public int TvViewed = 0;

  [Tooltip("Is the game currently in Free Roam mode?")]
  public bool FreeRoam = false;

  [Tooltip("Number of times the character has been viewed in the corridor.")]
  public int CorViewed = 0;

  [Tooltip("Number of times the accident has occurred in the lab.")]
  public int LabAccident = 0;

  [Tooltip("Number of times the storage room has been viewed.")]
  public int StorViewed = 0;

  [Tooltip("Number of times the office has been viewed.")]
  public int OfficeViewed = 0;

  [Tooltip("Number of times the lab has been viewed.")]
  public int LabViewed = 0;

  [Tooltip("Number of times the vent has been viewed.")]
  public int VentViewed = 0;

  [Tooltip("Whether the ladder vent has been accessed.")]
  public int LadderVent = 0;

  [Tooltip("Whether the player has learned about the meeting.")]
  public int LearnMeeting = 0;

  [Tooltip("Whether the player selected the organization.")]
  public int SelOrg = 0;

  [Tooltip("Whether the player selected the movement.")]
  public int SelMove = 0;

  [Tooltip("Whether the player has entered the box.")]
  public int EnteredBox = 0;

  [Tooltip("Whether the player has stored the box.")]
  public int StoredBox = 0;

  [Tooltip("Whether the player has witnessed the meeting.")]
  public int MeetingWitnessed = 0;

  [Tooltip("Whether the player ordered the cargo.")]
  public int OrderCargo = 0;
}
