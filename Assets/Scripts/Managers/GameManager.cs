using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using System.Linq; // helps you work with lists more easily.

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
    public GameObject speedTime;
    public GameObject skipTime;

    //Main
    public GameObject levelMain;
    public GameObject playerMain;

    public GameObject drawerBlock;

    public GameObject endcreditsPanel;
    public GameObject endcreditsScroll;

    [Header("UI Elements")]
    public GameObject adventurerIDHolder;
    public GameObject questHolder;
    public TMP_Text nameText;
    public TMP_Text traitsText;
    public TMP_Text questNameText;
    public TMP_Text questDescriptionText;
    public GameObject TraitsQuestOpen;

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
    private List<QuestResult> questResultsToday = new();

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

    private Dictionary<string, float> traitModifiers = new Dictionary<string, float>()
    {
     { "reliable", 0.1f },
     { "short tempered", -0.05f },
     { "charismatic", 0.05f },
     { "loyal", 0.05f },
     { "strategist", 0.15f },
     { "clumsy", -0.1f },
     { "resilient", 0.1f },
     { "distracted", -0.1f },
     { "quick learner", 0.08f },
     { "gifted", 0.1f },
     { "cowardly", -0.15f },
     { "arrogant", -0.05f },
     { "empathic", 0.05f },
     { "resourceful", 0.1f },
     { "injury prone", -0.1f },
     { "hard working", 0.07f },
     { "impulsive", -0.07f },
     { "greedy", -0.1f },
     { "superstitious", -0.05f }
    };

    private Dictionary<string, List<string>> questTraitBonuses = new()
    {
        { "Herbalist’s Request", new List<string> { "Resourceful", "Reliable" } },
        { "Lost Heirloom", new List<string> { "Charismatic", "Strategist" } },
        { "Goblin Encroachment", new List<string> { "Resilient", "Strategist" } },
        { "Ancient Ruins Survey", new List<string> { "Quick Learner", "Gifted" } },
        { "Trade Route Patrol", new List<string> { "Reliable", "Loyal" } },
        { "Haunted Manor Investigation", new List<string> { "Empathic", "Resilient" } },
        { "Beast Hunt: Dire Boar", new List<string> { "Hardy", "Strategist" } },
        { "Diplomatic Escort", new List<string> { "Charismatic", "Loyal" } },
        { "Mine Disaster Response", new List<string> { "Resourceful", "Resilient" } },
        { "Cult Disruption", new List<string> { "Strategist", "Gifted" } }
    };

    private Dictionary<string, List<string>> questTraitPenalties = new()
    {
        { "Herbalist’s Request", new List<string> { "Clumsy", "Distracted" } },
        { "Lost Heirloom", new List<string> { "Impulsive", "Selfish" } },
        { "Goblin Encroachment", new List<string> { "Cowardly", "Arrogant" } },
        { "Ancient Ruins Survey", new List<string> { "Superstitious", "Injury Prone" } },
        { "Trade Route Patrol", new List<string> { "Unreliable", "Distracted" } },
        { "Haunted Manor Investigation", new List<string> { "Superstitious", "Cowardly" } },
        { "Beast Hunt: Dire Boar", new List<string> { "Injury Prone", "Clumsy" } },
        { "Diplomatic Escort", new List<string> { "Arrogant", "Greedy" } },
        { "Mine Disaster Response", new List<string> { "Distracted", "Clumsy" } },
        { "Cult Disruption", new List<string> { "Impulsive", "Cowardly" } }
    };

    // ---- Adventurer preset class ----
    [System.Serializable] // NEW
    public class AdventurerPreset
    {
        public string name;
        public List<Trait> traits;
    }

    [Header("Adventurer Presets")]
    public List<AdventurerPreset> adventurerPresets = new List<AdventurerPreset>();

    private List<Adventurer> allAdventurers = new(); // Mood

    public static GameManager Instance { get; private set; }
    public Adventurer CurrentAdventurer => currentAdventurerAtCounter;

    private bool hasSpawnStarted = false;

    private Coroutine spawnCoroutine;
    private int dayNumber => GameResultsManager.Instance.GetCurrentDay();

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        //StartCoroutine(SpawnAdventurers());
        OnNewDayStart();
    }

    IEnumerator SpawnAdventurers()
    {
        if (hasSpawnStarted)
        {
            Debug.LogWarning("SpawnAdventurers coroutine already running, skipping duplicate start.");
            yield break;
        }
        hasSpawnStarted = true;

        int spawnDay = dayNumber;
        Debug.Log($"SpawnAdventurers started for Day {spawnDay}");
        isDayOver = false;

        if (spawnDay == 1)
        {
            for (int i = 0; i < 10; i++)
            {
                yield return new WaitUntil(() =>
                {
                    var t = timeManager.GetCurrentTime().TimeOfDay;
                    return t >= new TimeSpan(10, 0, 0) && t <= new TimeSpan(23, 0, 0);
                });

                GameObject adventurerObj = Instantiate(adventurerPrefab, adventurerEntry.position, Quaternion.identity);
                AdventurerPreset preset = adventurerPresets[i];

                Adventurer adventurer = new Adventurer(adventurerObj, preset.name);
                adventurer.traits.AddRange(preset.traits);
                adventurer.mood = Mood.Good;

                allAdventurers.Add(adventurer); // ONLY DAY 1
                activeAdventurers.Add(adventurer);

                yield return StartCoroutine(HandleDayOneRegistration(adventurer));
                yield return WaitForSecondsScaled(6f);
            }

            yield break;
        }
        else if (spawnDay == 5)
        {
            for (int i = 0; i < 10; i++)
            {
                if (isDayOver || dayNumber != spawnDay)
                {
                    Debug.Log($"Day {spawnDay} ended early, stopping spawn.");
                    yield break;
                }

                yield return new WaitUntil(() =>
                {
                    var t = timeManager.GetCurrentTime().TimeOfDay;
                    return t >= new TimeSpan(10, 0, 0) && t <= new TimeSpan(23, 0, 0);
                });

                Adventurer adventurer = SpawnOrReuseAdventurer(adventurerPresets[i]);
                activeAdventurers.Add(adventurer);

                yield return StartCoroutine(HandleDayFiveRaid());
                yield return WaitForSecondsScaled(6f);
            }

            yield break;
        }
        else
        {
            int adventurerCount = 0;

            while (!isDayOver && adventurerCount < adventurerPresets.Count)
            {
                if (dayNumber != spawnDay)
                {
                    Debug.Log($"Day changed from {spawnDay} to {dayNumber}, stopping spawn.");
                    yield break;
                }

                drawerBlock.SetActive(false);
                speedTime.SetActive(false);
                skipTime.SetActive(true);
                TraitsQuestOpen.SetActive(true);

                yield return WaitForSecondsScaled(30f);

                DateTime currentTime = timeManager.GetCurrentTime();
                Debug.Log($"Day {dayNumber} current time: {currentTime.TimeOfDay}");

                if (currentTime.TimeOfDay >= new TimeSpan(10, 0, 0) &&
                    currentTime.TimeOfDay <= new TimeSpan(23, 0, 0))
                {
                    Adventurer adventurer = SpawnOrReuseAdventurer(adventurerPresets[adventurerCount]);
                    activeAdventurers.Add(adventurer);

                    Transform target = spawnPoints[adventurerCount % spawnPoints.Length];
                    StartCoroutine(MoveToPosition(adventurer.gameObject, target.position));
                    StartCoroutine(AttemptMoveToCounter(adventurer, 15f));

                    adventurerCount++;
                }
            }
        }
    }

    private Adventurer SpawnOrReuseAdventurer(AdventurerPreset preset)
    {
        Adventurer existing = allAdventurers.FirstOrDefault(a => a.name == preset.name);
        if (existing != null)
        {
            GameObject obj = Instantiate(adventurerPrefab, adventurerEntry.position, Quaternion.identity);
            existing.gameObject = obj;

            Debug.Log($"[SpawnOrReuse] Reusing adventurer {existing.name} with mood: {existing.mood}");
            return existing;
        }

        // Only for Day 1 creation
        GameObject newObj = Instantiate(adventurerPrefab, adventurerEntry.position, Quaternion.identity);
        Adventurer newAdventurer = new Adventurer(newObj, preset.name);
        newAdventurer.traits.AddRange(preset.traits);
        newAdventurer.mood = Mood.Good;

        allAdventurers.Add(newAdventurer);
        Debug.Log($"[SpawnOrReuse] Creating new adventurer {newAdventurer.name}");

        return newAdventurer;
    }

    IEnumerator HandleDayOneRegistration(Adventurer adventurer)
    {
        while (counterOccupied)
            yield return null;

        counterOccupied = true;
        currentAdventurerAtCounter = adventurer;

        yield return StartCoroutine(MoveToPosition(adventurer.gameObject, adventurerCounter.position));
        TimeManager.Instance.PauseTime();

        Image.SetActive(true);
        dialogueButton.SetActive(true);
        adventurerIDHolder.SetActive(true);
        questHolder.SetActive(true);

        nameText.text = adventurer.name;
        traitsText.text = GetTraitsString(adventurer);

        questNameText.text = "Registering...";
        questDescriptionText.text = "";

        yield return new WaitUntil(() => !counterOccupied);
    }

    IEnumerator HandleDayFiveRaid()
    {
        while (counterOccupied)
            yield return null;

        counterOccupied = true;

        // For example, you can pick the first adventurer as the "main" one at the counter:
        if (activeAdventurers.Count == 0) yield break;

        Adventurer mainAdventurer = activeAdventurers[0];
        currentAdventurerAtCounter = mainAdventurer;

        yield return StartCoroutine(MoveToPosition(mainAdventurer.gameObject, adventurerCounter.position));
        TimeManager.Instance.PauseTime();

        Image.SetActive(true);
        dialogueButton.SetActive(true);
        adventurerIDHolder.SetActive(true);
        questHolder.SetActive(true);

        nameText.text = "Raid Party";
        traitsText.text = string.Join(", ", activeAdventurers.Select(a => a.name).ToArray());

        float successChance = CalculateRaidSuccessChance(activeAdventurers);
        int percentChance = Mathf.RoundToInt(successChance * 100f);

        questNameText.text = "Emergency Request";
        questDescriptionText.text = "Goblins are attacking the village! The Goblin King leads them. Defend the town and stop him before it’s too late!";

        yield return new WaitUntil(() => !counterOccupied);
    }

    public float CalculateRaidSuccessChance(List<Adventurer> adventurers)
    {
        float baseRate = 0.5f;
        float moodSum = 0f;

        foreach (var adv in adventurers)
        {
            float mod = GetMoodModifier(adv.mood);
            moodSum += mod;
            Debug.Log($"[RAID] {adv.name} mood: {adv.mood}, modifier: {mod}");
        }

        float avgMoodModifier = moodSum / adventurers.Count;
        float finalChance = Mathf.Clamp01(baseRate + avgMoodModifier);

        Debug.Log($"[RAID] Average mood modifier: {avgMoodModifier}, Final raid chance: {finalChance * 100}%");
        return finalChance;
    }

    private float GetMoodModifier(Mood mood)
{
    switch (mood)
    {
        case Mood.Good:     return  0.0f;
        case Mood.Neutral:  return -0.15f;
        case Mood.Bad:      return -0.3f;
        case Mood.VeryBad:  return -0.45f;
        default:            return  0f;
    }
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
        yield return WaitForSecondsScaled(delay);
        if (adventurer == null || isDayOver) yield break;

        if (counterOccupied)
        {
            StartCoroutine(AttemptMoveToCounter(adventurer, 1f));
            yield break;
        }

        counterOccupied = true;
        currentAdventurerAtCounter = adventurer;

        yield return StartCoroutine(MoveToPosition(adventurer.gameObject, adventurerCounter.position));
        TimeManager.Instance.PauseTime();

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

    IEnumerator WaitForSecondsScaled(float seconds)
    {
        float elapsed = 0f;
        while (elapsed < seconds)
        {
            elapsed += Time.deltaTime * TimeManager.Instance.GetTimeMultiplier();
            yield return null;
        }
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

        bool ignoredRequest = currentAdventurerAtCounter.wantsToChangeQuest && questToUse == GetAdventurerProvidedQuest();
        if (ignoredRequest)
            currentAdventurerAtCounter.ChangeMood(+1);

        float baseRate = 0.5f;
        float bonus = 0f;

        Debug.Log($"[StampQuest] Evaluating success for adventurer: {currentAdventurerAtCounter.name}");
        Debug.Log($"Base success rate: {baseRate * 100}%");
        Debug.Log($"Quest: {questToUse.questName}");

        foreach (var trait in currentAdventurerAtCounter.traits)
        {
            string traitName = trait.name.Trim();
            string traitKey = traitName.ToLowerInvariant();

            // Global modifier
            if (traitModifiers.TryGetValue(traitKey, out float globalMod))
            {
                bonus += globalMod;
                Debug.Log($" - [Global Trait] {traitName}: {globalMod:+0.00;-0.00}");
            }

            // Quest-specific bonus
            if (questTraitBonuses.TryGetValue(questToUse.questName, out var bonusTraits))
            {
                if (bonusTraits.Any(bt => string.Equals(bt.Trim(), traitName, StringComparison.OrdinalIgnoreCase)))
                {
                    bonus += 0.1f;
                    Debug.Log($" - [Quest Bonus] {traitName} matches bonus list for \"{questToUse.questName}\": +0.10");
                }
            }

            // Quest-specific penalty
            if (questTraitPenalties.TryGetValue(questToUse.questName, out var penaltyTraits))
            {
                if (penaltyTraits.Any(pt => string.Equals(pt.Trim(), traitName, StringComparison.OrdinalIgnoreCase)))
                {
                    bonus -= 0.1f;
                    Debug.Log($" - [Quest Penalty] {traitName} matches penalty list for \"{questToUse.questName}\": -0.10");
                }
            }
        }

        float finalRate = Mathf.Clamp01(baseRate + bonus);
        Debug.Log($"[Final Chance] Success chance for \"{questToUse.questName}\": {finalRate * 100:F1}%");

        bool success = UnityEngine.Random.value <= finalRate;
        GameResultsManager.Instance.AddQuestResult(new QuestResult(currentAdventurerAtCounter.name,questToUse.questName,currentAdventurerAtCounter.mood,success));
        Debug.Log(success
            ? $"[RESULT] Quest succeeded! (+100 gold)"
            : $"[RESULT] Quest failed. Mood worsens.");

        if (success)
        {
            GameResultsManager.Instance.AddEarnings(100);
        }
        else
        {
            currentAdventurerAtCounter.ChangeMood(+1);
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

            TimeManager.Instance.ResumeTime();
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

        // Stop any existing spawn coroutine before starting new day
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
        }

        hasSpawnStarted = false;
        isDayOver = false;
        adventurersStampedToday = 0;

        foreach (var adv in activeAdventurers)
        {
            if (adv.daysToSkip > 0) adv.daysToSkip--;
        }

        spawnCoroutine = StartCoroutine(SpawnAdventurers());
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
        choicePanel.SetActive(false);
    }

    void UpdateMoodUI()
    {
        if (currentAdventurerAtCounter == null) return;

        Debug.Log($"[UI] Updating mood: {currentAdventurerAtCounter.mood}");
        moodUI.text = $"Mood: {currentAdventurerAtCounter.mood}";
    }

       public void ProceedToNextDay()
    {
        //GameResultsManager.Instance.IncrementDay();
        timeManager.StartNewDay();
        OnNewDayStart();
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