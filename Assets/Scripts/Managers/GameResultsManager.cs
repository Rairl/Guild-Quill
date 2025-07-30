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

    [Header("Day Start UI")]
    public GameObject dayStartPanel;
    public TMP_Text dayStartText;

    private int dailyEarnings = 0;
    private int currentDay = 1;

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    void Start()
    {
        ShowDayStartPanel(); // Show Day 1 panel on game start
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

        if (currentDay > 5)
        {
            GameManager.Instance.endcreditsPanel.SetActive(true);
            GameManager.Instance.endcreditsScroll.SetActive(true);
            return; // Don't start a new day
        }

        TimeManager.Instance.StartNewDay();
        GameManager.Instance.SetDayOver(false);

        ShowDayStartPanel(); // Show new day panel
        GameManager.Instance.OnNewDayStart();
    }

    public int GetCurrentDay() => currentDay;

    public void ShowDayStartPanel()
    {
        if (dayStartPanel == null || dayStartText == null) return;

        dayStartText.text = $"Day {currentDay}";
        dayStartPanel.SetActive(true);
        StartCoroutine(HideDayStartPanelAfterDelay());
    }

    IEnumerator HideDayStartPanelAfterDelay()
    {
        yield return new WaitForSeconds(2f);
        dayStartPanel.SetActive(false);
    }

    /*public void IncrementDay()
    {
        currentDay += 1;
    }*/
}
