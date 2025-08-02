[System.Serializable]
public struct TimeRange
{
  public int StartHour;
  public int StartMinute;
  public int EndHour;
  public int EndMinute;

  public bool Includes(int hour, int minute)
  {
    int start = StartHour * 60 + StartMinute;
    int end = EndHour * 60 + EndMinute;
    int current = hour * 60 + minute;
    return current >= start && current < end;
  }
}
