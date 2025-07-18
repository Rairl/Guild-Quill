using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("Adventurer Settings")]
    public GameObject adventurerPrefab;
    public Transform adventurerEntry;
    public Transform adventurerCounter;
    public Transform[] spawnPoints;

    [Header("Time Management")]
    public TimeManager timeManager;
    public GameObject workUI;
    //Static
    public GameObject levelStatic;
    public GameObject playerStatic;

    //Main
    public GameObject levelMain;
    public GameObject playerMain;

    public GameObject drawerBlock;

    [Header("UI Elements")]
    public GameObject adventurerIDHolder;
    public GameObject questHolder;
    public TMP_Text nameText;
    public TMP_Text traitsText;
    public TMP_Text questNameText;
    public TMP_Text questDescriptionText;

    [Header("Dialogue and Mood UI")]
    public GameObject Image; // Adventurer image
    public TMP_Text moodUI; // Mood UI panel
    public GameObject moodImage;
    public TMP_Text dialogueText;
    public GameObject dialogueImage;
    public GameObject dialogueButton;
    public GameObject dialoguePanel;
    public GameObject choicePanel;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip adventurerQuestSfx;
    public AudioClip moodSFX;

    private List<Adventurer> activeAdventurers = new();
    private bool counterOccupied = false;
    private bool isDayOver = false;

    private Adventurer currentAdventurerAtCounter = null;
    private QuestData currentProvidedQuest = null;

    private int adventurersStampedToday = 0;

    private readonly string[] possibleNames = {
        "Cylix", "Pavel", "Terys", "Aria", "Minetta", "Dana", "Nindr", "Saevel", "Gildir",
        "Alavara", "Eilua", "Eiresti", "Bromir", "Drak", "Brund"
    };

    private List<Trait> allPositiveTraits = new()
    {
        new() { name = "Reliable", type = TraitType.Positive },
        new() { name = "Quick Learner", type = TraitType.Positive },
        new() { name = "Charismatic", type = TraitType.Positive },
        new() { name = "Resilient", type = TraitType.Positive },
        new() { name = "Resourceful", type = TraitType.Positive },
        new() { name = "Loyal", type = TraitType.Positive },
        new() { name = "Strategist", type = TraitType.Positive },
        new() { name = "Empathic", type = TraitType.Positive },
        new() { name = "Hardy", type = TraitType.Positive },
        new() { name = "Gifted", type = TraitType.Positive }
    };

    private List<Trait> allNegativeTraits = new()
    {
        new() { name = "Arrogant", type = TraitType.Negative },
        new() { name = "Unreliable", type = TraitType.Negative },
        new() { name = "Greedy", type = TraitType.Negative },
        new() { name = "Cowardly", type = TraitType.Negative },
        new() { name = "Clumsy", type = TraitType.Negative },
        new() { name = "Impulsive", type = TraitType.Negative },
        new() { name = "Vengeful", type = TraitType.Negative },
        new() { name = "Injury Prone", type = TraitType.Negative },
        new() { name = "Superstitious", type = TraitType.Negative },
        new() { name = "Distracted", type = TraitType.Negative }
    };

    private List<QuestData> questList = new()
    {
        new("Herbalist’s Request", "Gather rare herbs in the nearby meadow for the local apothecary. Low danger, good for beginners."),
        new("Lost Heirloom", "Retrieve a stolen family ring from petty bandits hiding in the old mill. Light combat may be required."),
        new("Goblin Encroachment", "Drive off goblins harassing farmsteads at dusk. Moderate risk."),
        new("Ancient Ruins Survey", "Escort scholars exploring newly uncovered ruins. Traps and minor monsters."),
        new("Trade Route Patrol", "Safeguard a caravan to the mountain pass. Higher chance of ambush."),
        new("Haunted Manor Investigation", "Uncover eerie disturbances in an abandoned estate. Supernatural threats possible."),
        new("Beast Hunt: Dire Boar", "Slay a dire boar terrorizing villages. Requires strength and coordination."),
        new("Diplomatic Escort", "Protect a treaty envoy. High political stakes; stealth recommended."),
        new("Mine Disaster Response", "Rescue trapped miners after a collapse. Complex hazards, limited time."),
        new("Cult Disruption", "Infiltrate and dismantle a cult near the cliffs. Extremely dangerous.")
    };

    // Dialogue options
    private List<string> registrationDialogues = new List<string>
    {
    "I want to register for the guild!",
    "Ready to become a proud guild member!",
    "Let’s get started with my registration.",
    "Sign me up, I’m ready!",
    "Excited to join the guild!"
    };

    private List<string> keepQuestDialogues = new List<string>
    {
    "I'm fine with the quest I have.",
    "This quest suits me just fine.",
    "I’ll stick with my current mission.",
    "No need to change, I’m happy with this.",
    "All good, this quest works for me."
    };

    private List<string> changeQuestDialogues = new List<string>
    {
    "I'd like to change my quest.",
    "This one isn’t right for me.",
    "Can I get a different mission?",
    "I’m not feeling confident about this quest.",
    "Mind if I switch to something else?"
    };

    public static GameManager Instance { get; private set; }
    public Adventurer CurrentAdventurer => currentAdventurerAtCounter;

    private int dayNumber => GameResultsManager.Instance.GetCurrentDay();

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        StartCoroutine(SpawnAdventurers());
    }

    IEnumerator SpawnAdventurers()
    {
        Debug.Log($"SpawnAdventurers started for Day {dayNumber}");
        isDayOver = false;

        if (dayNumber == 1)
        {
            for (int i = 0; i < 10; i++)
            {
                if (isDayOver)
                {
                    Debug.Log("Day 1 ended early, stopping spawning.");
                    yield break;
                }

                // Wait until between 10:00 AM and 11:00 PM
                yield return new WaitUntil(() =>
                {
                    var t = timeManager.GetCurrentTime().TimeOfDay;
                    return t >= new TimeSpan(10, 0, 0) && t <= new TimeSpan(23, 0, 0);
                });

                GameObject adventurerObj = Instantiate(adventurerPrefab, adventurerEntry.position, Quaternion.identity);
                string randomName = possibleNames[UnityEngine.Random.Range(0, possibleNames.Length)];
                Adventurer adventurer = new(adventurerObj, randomName);
                adventurer.traits.AddRange(RandomizeTraits(allPositiveTraits, 3));
                adventurer.traits.AddRange(RandomizeTraits(allNegativeTraits, 2));
                activeAdventurers.Add(adventurer);

                yield return StartCoroutine(HandleDayOneRegistration(adventurer));

                // Wait a random delay between 10 to 15 seconds before spawning next
                float waitTime = UnityEngine.Random.Range(5f, 10f);
                yield return new WaitForSeconds(waitTime);
            }
        }
        else
        {
            // Days 2-7: Adventurers spawn continuously between 10:00am and 10:30pm
            while (!isDayOver)
            {
                drawerBlock.SetActive(false);

                float waitTime = UnityEngine.Random.Range(20f, 40f);
                yield return new WaitForSeconds(waitTime);

                DateTime currentTime = timeManager.GetCurrentTime();
                Debug.Log($"Day {dayNumber} current time: {currentTime.TimeOfDay}");

                if (currentTime.TimeOfDay >= new TimeSpan(10, 0, 0) &&
                    currentTime.TimeOfDay <= new TimeSpan(23, 0, 0))
                {
                    GameObject adventurerObj = Instantiate(adventurerPrefab, adventurerEntry.position, Quaternion.identity);
                    string randomName = possibleNames[UnityEngine.Random.Range(0, possibleNames.Length)];
                    Adventurer adventurer = new(adventurerObj, randomName);
                    adventurer.traits.AddRange(RandomizeTraits(allPositiveTraits, 3));
                    adventurer.traits.AddRange(RandomizeTraits(allNegativeTraits, 2));
                    activeAdventurers.Add(adventurer);

                    // Move to a random spawn point first
                    Transform target = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)];
                    StartCoroutine(MoveToPosition(adventurer.gameObject, target.position));

                    // After a delay, attempt to move to the counter
                    StartCoroutine(AttemptMoveToCounter(adventurer, UnityEngine.Random.Range(10f, 25f)));

                }
            }
        }
        Debug.Log("SpawnAdventurers coroutine ended");
    }

    IEnumerator HandleDayOneRegistration(Adventurer adventurer)
    {
        while (counterOccupied)
            yield return null; // wait if counter busy

        counterOccupied = true;
        currentAdventurerAtCounter = adventurer;

        yield return StartCoroutine(MoveToPosition(adventurer.gameObject, adventurerCounter.position));

        // Show adventurer UI
        Image.SetActive(true);
        dialogueButton.SetActive(true);
        adventurerIDHolder.SetActive(true);
        questHolder.SetActive(true);

        nameText.text = adventurer.name;
        traitsText.text = GetTraitsString(adventurer);

        // On Day 1, no quests, only registration
        questNameText.text = "Registering...";
        questDescriptionText.text = "";

        // Wait for player to complete registration (e.g., stamping)
        yield return new WaitUntil(() => !counterOccupied);
    }

    List<Trait> RandomizeTraits(List<Trait> pool, int max)
    {
        List<Trait> selected = new();
        int count = UnityEngine.Random.Range(max, max + 1); // Always max traits

        while (selected.Count < count)
        {
            Trait t = pool[UnityEngine.Random.Range(0, pool.Count)];
            if (!selected.Exists(existing => existing.name == t.name))
                selected.Add(t);
        }
        return selected;
    }

    IEnumerator MoveToPosition(GameObject obj, Vector3 targetPosition)
    {
        const float speed = 2f;
        while (Vector3.Distance(obj.transform.position, targetPosition) > 0.1f)
        {
            obj.transform.position = Vector3.MoveTowards(obj.transform.position, targetPosition, speed * Time.deltaTime);
            yield return null;
        }
    }

    IEnumerator AttemptMoveToCounter(Adventurer adventurer, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (adventurer == null || isDayOver) yield break;

        if (counterOccupied)
        {
            StartCoroutine(AttemptMoveToCounter(adventurer, 1f));
            yield break;
        }

        counterOccupied = true;
        currentAdventurerAtCounter = adventurer;

        yield return StartCoroutine(MoveToPosition(adventurer.gameObject, adventurerCounter.position));

        Image.SetActive(true);
        dialogueButton.SetActive(true);

        adventurerIDHolder.SetActive(true);
        questHolder.SetActive(true);

        nameText.text = adventurer.name;
        traitsText.text = GetTraitsString(adventurer);

        QuestData randomQuest = questList[UnityEngine.Random.Range(0, questList.Count)];
        currentProvidedQuest = randomQuest;

        questNameText.text = randomQuest.questName;
        questDescriptionText.text = randomQuest.questDescription;
    }

    string GetTraitsString(Adventurer adventurer)
    {
        if (adventurer.traits.Count == 0) return "Traits: None";

        List<string> pos = new();
        List<string> neg = new();

        foreach (var t in adventurer.traits)
        {
            if (t.type == TraitType.Positive) pos.Add(t.name);
            else neg.Add(t.name);
        }

        string result = "Traits:\n";
        if (pos.Count > 0)
            result += string.Join(", ", pos) + "\n";
        if (neg.Count > 0)
            result += string.Join(", ", neg);
        return result;
    }

    public void StampQuest(QuestData questToUse)
    {
        if (currentAdventurerAtCounter == null || questToUse == null) return;

        // Check if player ignored adventurer's request
        bool ignoredRequest = currentAdventurerAtCounter.wantsToChangeQuest && questToUse == GetAdventurerProvidedQuest();

        if (ignoredRequest)
        {
            currentAdventurerAtCounter.ChangeMood(+1); // Mood gets worse
        }

        // Quest success logic
        float baseRate = 0.8f;
        float bonus = 0f;

        foreach (var trait in currentAdventurerAtCounter.traits)
        {
            if (trait.name == "Reliable")
                bonus += 0.05f;
        }

        float finalRate = Mathf.Clamp01(baseRate + bonus);
        bool success = UnityEngine.Random.value <= finalRate;

        if (success)
        {
            GameResultsManager.Instance.AddEarnings(100);
        }
        else
        {
            currentAdventurerAtCounter.ChangeMood(+1); // Failure = mood drops
        }

        // Check if adventurer leaves
        if (currentAdventurerAtCounter.mood == Mood.VeryBad)
        {
            StartCoroutine(HandleAdventurerQuit(currentAdventurerAtCounter));
        }

        CompleteQuestAndSendAdventurer();
    }

    public void CompleteQuestAndSendAdventurer()
    {
        if (audioSource != null && adventurerQuestSfx != null)
            audioSource.PlayOneShot(adventurerQuestSfx);

        if (currentAdventurerAtCounter != null)
            StartCoroutine(MoveAdventurerBack(currentAdventurerAtCounter));

        adventurersStampedToday++;

        // Show end panel after stamping 10 adventurers
        if (adventurersStampedToday >= 10)
        {
            SetDayOver(true); // Stop new adventurers
            //RemoveAllAdventurers();
            GameResultsManager.Instance.ShowEndResults();
            endDay();
        }
    }

    IEnumerator MoveAdventurerBack(Adventurer adventurer)
    {
        ClearUI();

        if (adventurer != null && adventurer.gameObject != null)
        {
            yield return StartCoroutine(MoveToPosition(adventurer.gameObject, adventurerEntry.position));

            activeAdventurers.Remove(adventurer);
            Destroy(adventurer.gameObject);
        }

        counterOccupied = false;
        currentAdventurerAtCounter = null;
        currentProvidedQuest = null;
    }

    void ClearUI()
    {
        adventurerIDHolder.SetActive(false);
        questHolder.SetActive(false);
        Image.SetActive(false);
        dialogueButton.SetActive(false);
        dialoguePanel.SetActive(false);

        nameText.text = "";
        traitsText.text = "";
        questNameText.text = "";
        questDescriptionText.text = "";
        dialogueText.text = "";
    }

    void endDay()
    {
        workUI.SetActive(false);
        levelStatic.SetActive(false);
        playerStatic.SetActive(false);
        levelMain.SetActive(true);
        playerMain.SetActive(true);
    }

    public void SetDayOver(bool isOver)
    {
        isDayOver = isOver;
        if (isOver) ClearUI();
    }

    public QuestData GetAdventurerProvidedQuest()
    {
        return currentProvidedQuest;
    }

    // Call this on new day start to reset and start spawning again
    public void OnNewDayStart()
    {
        Debug.Log($"Starting Day {dayNumber}");
        isDayOver = false;
        adventurersStampedToday = 0;

        foreach (var adventurer in activeAdventurers)
        {
            if (adventurer.daysToSkip > 0)
                adventurer.daysToSkip--;
        }

        StartCoroutine(SpawnAdventurers());
    }

    public void OnAskButtonPressed()
    {
        if (currentAdventurerAtCounter == null) return;

        int currentDay = GameResultsManager.Instance.GetCurrentDay();

        // DO NOT reset mood here! Just show mood as-is.

        // Show mood UI
        moodUI.text = $"Mood: {currentAdventurerAtCounter.mood}";
        moodUI.gameObject.SetActive(true);
        moodImage.gameObject.SetActive(true);

        choicePanel.SetActive(true);

        if (currentDay == 1)
        {
            // Registration dialogue only
            currentAdventurerAtCounter.wantsToChangeQuest = false; // No quest changes on day 1

            // Pick random registration dialogue
            string randomLine = registrationDialogues[UnityEngine.Random.Range(0, registrationDialogues.Count)];
            dialogueText.text = $"{currentAdventurerAtCounter.name}: {randomLine}";
        }
        else
        {
            // Randomly decide if adventurer wants to change quest
            currentAdventurerAtCounter.wantsToChangeQuest = UnityEngine.Random.value < 0.5f;

            if (currentAdventurerAtCounter.wantsToChangeQuest)
            {
                string randomLine = changeQuestDialogues[UnityEngine.Random.Range(0, changeQuestDialogues.Count)];
                dialogueText.text = $"{currentAdventurerAtCounter.name}: {randomLine}";
            }
            else
            {
                string randomLine = keepQuestDialogues[UnityEngine.Random.Range(0, keepQuestDialogues.Count)];
                dialogueText.text = $"{currentAdventurerAtCounter.name}: {randomLine}";
            }
        }

        dialogueText.gameObject.SetActive(true);
        dialogueImage.gameObject.SetActive(true);
    }

    IEnumerator HandleAdventurerQuit(Adventurer adventurer)
    {
        if (adventurer == null)
        {
            Debug.LogError("HandleAdventurerQuit called with null adventurer!");
            yield break;
        }

        // Create a temporary UI text message
        TMP_Text quittingText = Instantiate(new GameObject("QuitText"), transform).AddComponent<TMP_Text>();
        quittingText.text = $"{adventurer.name} has left the guild.";
        quittingText.fontSize = 28;
        quittingText.alignment = TextAlignmentOptions.Center;
        quittingText.color = Color.red;
        quittingText.rectTransform.sizeDelta = new Vector2(400, 50);
        quittingText.rectTransform.anchoredPosition = new Vector2(0, -100);

        // Wait 2 seconds so player can read it
        yield return new WaitForSeconds(2f);

        // Destroy the message GameObject
        Destroy(quittingText.gameObject);

        // Hide adventurer from the scene (optional)
        if (adventurer.gameObject != null)
            adventurer.gameObject.SetActive(false);

    }

    public void OnChoiceSelected(bool isAccepted)
    {
        if (currentAdventurerAtCounter == null) return;

        int currentDay = GameResultsManager.Instance.GetCurrentDay();

        // Block input if adventurer already quit
        if (currentAdventurerAtCounter.mood == Mood.VeryBad)
        {
            Debug.Log("Adventurer already left the guild. Input blocked.");
            return;
        }

        bool shouldLoseMood = false;
        bool wantsToChange = currentAdventurerAtCounter.wantsToChangeQuest;

        if (currentDay == 1)
        {
            // Day 1: Deny lowers mood, Accept no change
            if (!isAccepted)
                shouldLoseMood = true;
        }
        else
        {
            // Days 2-7:
            // Deny when they want to keep = lose mood
            // Accept when they want to change = lose mood
            if ((!isAccepted && !wantsToChange) || (isAccepted && wantsToChange))
                shouldLoseMood = true;
        }

        if (shouldLoseMood)
        {
            Debug.Log("Wrong choice! Mood will go down.");
            currentAdventurerAtCounter.ChangeMood(+1);

            if (audioSource && moodSFX)
                audioSource.PlayOneShot(moodSFX);
        }
        else
        {
            Debug.Log("Correct choice. Mood stays.");
        }

        UpdateMoodUI();

        if (currentAdventurerAtCounter.mood == Mood.VeryBad)
        {
            Debug.Log($"{currentAdventurerAtCounter.name} has quit the guild!");
            StartCoroutine(HandleAdventurerQuit(currentAdventurerAtCounter));
            choicePanel.SetActive(false);
            return;
        }

        choicePanel.SetActive(false);
    }

    void UpdateMoodUI()
    {
        if (currentAdventurerAtCounter == null) return;

        Debug.Log($"[UI] Updating mood: {currentAdventurerAtCounter.mood}");
        moodUI.text = $"Mood: {currentAdventurerAtCounter.mood}";
    }
}

[System.Serializable]
public class QuestData
{
    public string questName;
    public string questDescription;

    public QuestData(string name, string description)
    {
        questName = name;
        questDescription = description;
    }
}