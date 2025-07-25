using UnityEngine;
using TMPro;
using System;

public class TimeManager : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text timerText;
    public TMP_Text cycleSpeedButtonText;      // Text on the button

    [Header("Timing")]
    public float incrementInterval = 1f;

    [Header("References")]
    public GameObject staticPlayer;
    public GameObject movingPlayer;
    public CounterController activeCounter;

    private DateTime currentTime;
    private DateTime endTime;
    private float timer;
    private bool hasEnded = false;

    private float[] speedLevels = { 1f, 4f, 6f };
    private string[] speedLabels = { "x1", "x2", "x3" }; // Text shown on button
    private int currentSpeedIndex = 0;
    private float timeMultiplier = 1f;

    public float GetTimeMultiplier() => timeMultiplier;

    private bool isPausedByCounter = false;

    public static TimeManager Instance;

    private bool hasStartedDay = false;

    void Start()
    {
        Instance = this;

        currentTime = DateTime.Today.AddHours(9);
        endTime = DateTime.Today.AddHours(23);

        SetTimeSpeed(speedLevels[currentSpeedIndex]); // Initialize with x1
        UpdateTimerDisplay();
    }

    void Update()
    {
        if (hasEnded || isPausedByCounter) return;

        timer += Time.deltaTime * timeMultiplier;

    if (timer >= incrementInterval)
    {
        timer = 0f;
        currentTime = currentTime.AddMinutes(1);
        UpdateTimerDisplay();

        // START DAY AT 10:00 AM
        if (!hasStartedDay && currentTime >= DateTime.Today.AddHours(10))
        {
            hasStartedDay = true;

                // Set active static player
                movingPlayer.SetActive(false);
                staticPlayer.SetActive(true);

                if (activeCounter != null)
                activeCounter.StartDay();
        }

        // END DAY AT END TIME
        if (currentTime >= endTime)
        {
            EndTimerActions();
        }
    }
    }

    void UpdateTimerDisplay()
    {
        if (timerText != null)
            timerText.text = currentTime.ToString("hh:mm tt");
    }

    void EndTimerActions()
    {
        hasEnded = true;

        if (activeCounter != null)
            activeCounter.stopWork();

        if (staticPlayer != null)
            staticPlayer.SetActive(false);

        if (movingPlayer != null)
            movingPlayer.SetActive(true);

        GameResultsManager.Instance.ShowEndResults();
        FindObjectOfType<GameManager>().SetDayOver(true);
    }

    public void StartNewDay()
    {
        currentTime = DateTime.Today.AddHours(9);
        endTime = DateTime.Today.AddHours(23);
        timer = 0f;
        hasStartedDay = false;
        SetTimeSpeed(1f); // Reset to 1x
        UpdateTimerDisplay();
    }

    public void CycleTimeSpeed()
    {
        currentSpeedIndex = (currentSpeedIndex + 1) % speedLevels.Length;
        SetTimeSpeed(speedLevels[currentSpeedIndex]);
    }

    private void SetTimeSpeed(float speed)
    {
        timeMultiplier = speed;

        if (cycleSpeedButtonText != null)
            cycleSpeedButtonText.text = speedLabels[currentSpeedIndex];
    }


    public DateTime GetCurrentTime()
    {
        return currentTime;
    }

    public void PauseTime()
    {
        isPausedByCounter = true;
    }

    public void ResumeTime()
    {
        isPausedByCounter = false;
    }

    public void SkipTo10AM()
    {
        if (currentTime < DateTime.Today.AddHours(10))
        {
            currentTime = DateTime.Today.AddHours(10);
            UpdateTimerDisplay();
        }
    }
}
