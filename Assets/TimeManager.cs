using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class TimeManager : MonoBehaviour
{

    [Header("UI")]
    public TMP_Text timerText;

    [Header("Timing")]
    public float incrementInterval = 1f; // Time in seconds between minute increments

    [Header("References")]
    public GameObject staticPlayer;
    public GameObject movingPlayer;
    public CounterController activeCounter; // Replace 'ActiveCounter' with your actual class type

    private DateTime currentTime;
    private DateTime endTime;
    private float timer;
    private bool hasEnded = false;

    void Start()
    {
        // Start at 9:00 AM and end at 11:00 PM
        currentTime = DateTime.Today.AddHours(9);
        endTime = DateTime.Today.AddHours(23);

        UpdateTimerDisplay();
    }

    void Update()
    {
        if (hasEnded) return;

        timer += Time.deltaTime;

        if (timer >= incrementInterval)
        {
            timer = 0f;
            currentTime = currentTime.AddMinutes(1);
            UpdateTimerDisplay();

            if (currentTime >= endTime)
            {
                EndTimerActions();
            }
        }
    }

    void UpdateTimerDisplay()
    {
        if (timerText != null)
        {
            timerText.text = currentTime.ToString("hh:mm tt");
        }
    }

    void EndTimerActions()
    {
        hasEnded = true;

        if (activeCounter != null)
        {
            activeCounter.stopWork();
        }

        if (staticPlayer != null)
        {
            staticPlayer.SetActive(false);
        }

        if (movingPlayer != null)
        {
            movingPlayer.SetActive(true);
        }
    }
}
