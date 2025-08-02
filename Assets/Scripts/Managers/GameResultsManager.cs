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
    public Button winloseButton;

    [Header("Day Start UI")]
    public GameObject dayStartPanel;
    public TMP_Text dayStartText;

    private int dailyEarnings = 0;
    private int currentDay = 1;

    [Header("Results List UI")]
    public TMP_Text resultsListTextLeft;
    public TMP_Text resultsListTextRight;

    private List<QuestResult> questResultsToday = new();

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

    public int GetTotalEarnings()
    {
        return dailyEarnings;
    }

    public void AddQuestResult(QuestResult result)
    {
        questResultsToday.Add(result);
    }

    public void ShowEndResults()
    {
        earningsText.text = $"You earned ${dailyEarnings} today!";
        dayText.text = $"End of Day {currentDay}";

        // Split into two groups
        int splitIndex = Mathf.CeilToInt(questResultsToday.Count / 2f);
        List<QuestResult> leftResults = questResultsToday.GetRange(0, splitIndex);
        List<QuestResult> rightResults = questResultsToday.GetRange(splitIndex, questResultsToday.Count - splitIndex);

        resultsListTextLeft.text = BuildResultsText(leftResults);
        resultsListTextRight.text = BuildResultsText(rightResults);

        endResultsPanel.SetActive(true);

        nextDayButton.onClick.RemoveAllListeners();
        nextDayButton.onClick.AddListener(NextDay);
    }

    void NextDay()
    {
        dailyEarnings = 0;
        currentDay++;
        endResultsPanel.SetActive(false);

        if (currentDay > 4)
        {
            GameManager.Instance.endcreditsPanel.SetActive(true);
            GameManager.Instance.endcreditsScroll.SetActive(true);
            return; // Don't start a new day
        }

        TimeManager.Instance.StartNewDay();
        GameManager.Instance.SetDayOver(false);
        questResultsToday.Clear();

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
    string BuildResultsText(List<QuestResult> results)
    {
        string output = "";

        foreach (var result in results)
        {
            string outcome = result.wasSuccessful ? "<color=green>Success</color>" : "<color=red>Failed</color>";
            output +=
                $"<b>{result.adventurerName}</b>\n" +
                $"{result.questName}\n" +
                $"Mood: {result.moodAtEnd} | {outcome}\n\n";
        }

        return output;
    }

}
public class QuestResult
{
    public string adventurerName;
    public string questName;
    public Mood moodAtEnd;
    public bool wasSuccessful;

    public QuestResult(string name, string quest, Mood mood, bool success)
    {
        adventurerName = name;
        questName = quest;
        moodAtEnd = mood;
        wasSuccessful = success;
    }
}
