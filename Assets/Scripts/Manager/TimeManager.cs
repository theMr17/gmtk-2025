using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance { get; private set; }

    [Header("Clock Settings")]
    [Tooltip("Start time in minutes after midnight (6:00am = 6*60)")]
    public float startTimeMinutes = 6 * 60;
    [Tooltip("Real seconds per in-game minute (1 real second = 1 game minute)")]
    public float realSecondsPerGameMinute = 1f;

    [Header("UI")]
    public TextMeshProUGUI clockText;

    // current time in in-game minutes since midnight (0–1440)
    private float _gameMinutes;
    private bool _isPaused = false;

    // Events you can subscribe to
    public event Action<int, int> OnMinuteChanged;   // (hour, minute)
    public event Action<int> OnHourChanged;          // (hour)
    public event Action OnDayPassed;                 // at midnight wrap

    private int _lastHour, _lastMinute;

    private void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
        else Destroy(gameObject);
    }

    private void Start()
    {
        _gameMinutes = Mathf.Clamp(startTimeMinutes, 0, 1440);
        UpdateClockUI();
        _lastHour = Hour;
        _lastMinute = Minute;
    }

    private void Update()
    {
        if (_isPaused) return;

        // advance game minutes
        _gameMinutes += Time.deltaTime / realSecondsPerGameMinute;
        if (_gameMinutes >= 1440f)
        {
            _gameMinutes -= 1440f;
            OnDayPassed?.Invoke();
        }

        // detect minute tick
        int newMinute = Minute;
        if (newMinute != _lastMinute)
        {
            OnMinuteChanged?.Invoke(Hour, newMinute);
            _lastMinute = newMinute;
            UpdateClockUI();
        }

        // detect hour tick
        int newHour = Hour;
        if (newHour != _lastHour)
        {
            OnHourChanged?.Invoke(newHour);
            _lastHour = newHour;
        }
    }

    /// <summary>Hour 0–23</summary>
    public int Hour => Mathf.FloorToInt(_gameMinutes / 60f);
    /// <summary>Minute 0–59</summary>
    public int Minute => Mathf.FloorToInt(_gameMinutes % 60f);

    public void PauseTime() => _isPaused = true;
    public void ResumeTime() => _isPaused = false;

    private void UpdateClockUI()
    {
        if (clockText != null)
        {
            clockText.text = $"{Hour:00}:{Minute:00}";
        }
    }
}
