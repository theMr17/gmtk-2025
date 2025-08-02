using System;
using TMPro;
using UnityEngine;

public class DayTimeManager : MonoBehaviour
{
    public static DayTimeManager Instance { get; private set; }

    private const float MinutesPerDay = 1440f;

    [Header("Clock Settings")]
    public float startTimeMinutes = 6 * 60;
    public float realSecondsPerGameMinute = 1f;

    private float _gameMinutes;
    private bool _isPaused;

    private int _lastHour;
    private int _lastMinute;

    // EVENTS
    public class TimeChangedEventArgs : EventArgs
    {
        public int Hour;
        public int Minute;
    }

    public class HourChangedEventArgs : EventArgs
    {
        public int Hour;
    }

    public event EventHandler<TimeChangedEventArgs> OnMinuteChanged;
    public event EventHandler<HourChangedEventArgs> OnHourChanged;
    public event EventHandler OnDayPassed;

    public int Hour => Mathf.FloorToInt(_gameMinutes / 60f);
    public int Minute => Mathf.FloorToInt(_gameMinutes % 60f);

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this; DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    private void Start()
    {
        _gameMinutes = Mathf.Clamp(startTimeMinutes, 0, MinutesPerDay);
        _lastHour = Hour;
        _lastMinute = Minute;

        // Fire initial UI update
        OnMinuteChanged?.Invoke(this, new TimeChangedEventArgs { Hour = _lastHour, Minute = _lastMinute });

        DialogueManager.Instance.OnDialogueStart += DialogManager_OnDialogueStart;
        DialogueManager.Instance.OnDialogueEnd += DialogManager_OnDialogueEnd;
    }

    private void DialogManager_OnDialogueStart(object sender, EventArgs e)
    {
        // Pause time during dialogue
        PauseTime();
    }

    private void DialogManager_OnDialogueEnd(object sender, EventArgs e)
    {
        // Resume time after dialogue ends
        ResumeTime();
    }

    private void Update()
    {
        if (_isPaused) return;

        _gameMinutes += Time.deltaTime / realSecondsPerGameMinute;

        if (_gameMinutes >= MinutesPerDay)
        {
            _gameMinutes -= MinutesPerDay;
            OnDayPassed?.Invoke(this, EventArgs.Empty);
        }

        int currentHour = Hour;
        int currentMinute = Minute;

        if (currentMinute != _lastMinute)
        {
            _lastMinute = currentMinute;
            OnMinuteChanged?.Invoke(this, new TimeChangedEventArgs { Hour = currentHour, Minute = currentMinute });
        }

        if (currentHour != _lastHour)
        {
            _lastHour = currentHour;
            OnHourChanged?.Invoke(this, new HourChangedEventArgs { Hour = currentHour });
        }
    }

    public void PauseTime() => _isPaused = true;
    public void ResumeTime(int? hours = null, int? mins = null)
    {
        if (hours.HasValue && mins.HasValue)
        {
            _gameMinutes = Mathf.Clamp(hours.Value * 60 + mins.Value, 0, MinutesPerDay);
            _lastHour = Hour;
            _lastMinute = Minute;
            OnMinuteChanged?.Invoke(this, new TimeChangedEventArgs { Hour = _lastHour, Minute = _lastMinute });
        }
        _isPaused = false;
    }

    public void EndDay()
    {
        GameStateSo gameState = GameManager.Instance.gameState;

        // Advance the day
        gameState.Day++;

        // Update progression flags
        if (gameState.CalViewed is 1 or 3 or 5)
            gameState.CalViewed++;

        if (gameState.TvViewed is 1 or 3)
            gameState.TvViewed++;

        // Fire OnDayPassed event for any listeners (UI, quest systems, etc.)
        OnDayPassed?.Invoke(this, EventArgs.Empty);

        // Reset in-game time to start of new day
        _gameMinutes = Mathf.Clamp(startTimeMinutes, 0, 1440);

        // Update internal trackers to force refresh on next Update() tick
        _lastHour = Hour;
        _lastMinute = Minute;
        OnMinuteChanged?.Invoke(this, new TimeChangedEventArgs { Hour = _lastHour, Minute = _lastMinute });
    }

    void OnDestroy()
    {
        DialogueManager.Instance.OnDialogueStart -= DialogManager_OnDialogueStart;
        DialogueManager.Instance.OnDialogueEnd -= DialogManager_OnDialogueEnd;
    }
}
