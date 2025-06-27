using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameResultsManager : MonoBehaviour
{
    public static GameResultsManager Instance;

    [Header("UI")]
    public GameObject endResultsPanel;
    public TMP_Text earningsText;
    public TMP_Text dayText;
    public Button nextDayButton;

    private int dailyEarnings = 0;
    private int currentDay = 1;

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    public void AddEarnings(int amount)
    {
        dailyEarnings += amount;
    }

    public void ShowEndResults()
    {
        earningsText.text = $"You earned ${dailyEarnings} today!";
        dayText.text = $"End of Day {currentDay}";
        endResultsPanel.SetActive(true);

        nextDayButton.onClick.RemoveAllListeners();
        nextDayButton.onClick.AddListener(NextDay);
    }

    void NextDay()
    {
        dailyEarnings = 0;
        currentDay++;
        endResultsPanel.SetActive(false);

        TimeManager.Instance.StartNewDay();
        FindObjectOfType<GameManager>().SetDayOver(false);
    }

    public int GetCurrentDay() => currentDay;
}
