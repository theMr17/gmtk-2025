using UnityEngine;

public class GameState : MonoBehaviour
{
  public int Day = 1;
  public bool FreeRoam = false;
  public int CalViewed = 0;

  public void EndDay()
  {
    Day++;
    if (CalViewed == 1 || CalViewed == 3)
      CalViewed++;
  }
}