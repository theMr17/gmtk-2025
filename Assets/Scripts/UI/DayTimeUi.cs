using TMPro;
using UnityEngine;

public class DayTimeUi : MonoBehaviour
{
  [SerializeField] private TextMeshProUGUI _clockText;
  [SerializeField] private TextMeshProUGUI _dayText;

  private void Start()
  {
    DayTimeManager.Instance.OnMinuteChanged += HandleMinuteChanged;
    DayTimeManager.Instance.OnDayPassed += HandleDayPassed;

    // Initial setup
    UpdateClockText(DayTimeManager.Instance.Hour, DayTimeManager.Instance.Minute);
    UpdateDayText();
  }

  private void HandleMinuteChanged(object sender, DayTimeManager.TimeChangedEventArgs e)
  {
    UpdateClockText(e.Hour, e.Minute);
  }

  private void HandleDayPassed(object sender, System.EventArgs e)
  {
    UpdateDayText();
  }

  private void UpdateClockText(int hour, int minute)
  {
    if (_clockText != null)
      _clockText.text = $"{hour:00}:{minute:00}";
  }

  private void UpdateDayText()
  {
    if (_dayText != null)
    {
      int currentDay = GameManager.Instance.gameState.Day;
      _dayText.text = $"Day {currentDay}";
    }
  }

  private void OnDestroy()
  {
    if (DayTimeManager.Instance != null)
    {
      DayTimeManager.Instance.OnMinuteChanged -= HandleMinuteChanged;
      DayTimeManager.Instance.OnDayPassed -= HandleDayPassed;
    }
  }
}
