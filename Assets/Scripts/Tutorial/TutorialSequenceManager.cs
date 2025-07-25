using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


[System.Serializable]
public class TutorialSteps
{
    [TextArea(3, 10)] public string text;
    public Sprite image;
}
public class TutorialSequenceManager : MonoBehaviour
{
    [Header("Tutorial Steps")]
    public TutorialSteps[] steps;

    [Header("UI References")]
    public GameObject introPanel;
    public Image imageDisplay;
    public TextMeshProUGUI textDisplay;
    public Button nextButton;
    public Button backButton;

    [Header("Settings")]
    public float delayBeforeStart = 3f;

    private int currentIndex = 0;

    void Start()
    {
        // Hide the intro panel and clear any placeholder text/image
        introPanel.SetActive(false);
        textDisplay.text = "";
        imageDisplay.sprite = null;

        // Begin coroutine to delay and then show tutorial
        StartCoroutine(WaitAndStartTutorial());
    }

    IEnumerator WaitAndStartTutorial()
    {
        yield return new WaitForSecondsRealtime(delayBeforeStart);

        Time.timeScale = 0f;
        introPanel.SetActive(true);
        ShowStep(0);

        nextButton.onClick.AddListener(Next);
        backButton.onClick.AddListener(Back);
    }

    void ShowStep(int index)
    {
        textDisplay.text = steps[index].text;
        imageDisplay.sprite = steps[index].image;

        // Show/hide Back button depending on position
        backButton.gameObject.SetActive(index > 0);

        // Change Next button text to "Start" if it's the last step
        nextButton.GetComponentInChildren<TextMeshProUGUI>().text =
            (index == steps.Length - 1) ? "Close" : "Next";
    }

    void Next()
    {
        if (currentIndex < steps.Length - 1)
        {
            currentIndex++;
            ShowStep(currentIndex);
        }
        else
        {
            // End of tutorial — resume game
            introPanel.SetActive(false);
            nextButton.gameObject.SetActive(false);
            backButton.gameObject.SetActive(false);
            Time.timeScale = 1f;
        }
    }

    void Back()
    {
        if (currentIndex > 0)
        {
            currentIndex--;
            ShowStep(currentIndex);
        }
    }

    // Public method to show tutorial again from any button
    public void ShowTutorial()
    {
        Time.timeScale = 0f;       // Pause game
        introPanel.SetActive(true);
        currentIndex = 0;          // Reset tutorial to first step
        ShowStep(currentIndex);

        nextButton.gameObject.SetActive(true);
        backButton.gameObject.SetActive(false);
    }
}
