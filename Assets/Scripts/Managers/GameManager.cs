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

    [Header("UI Elements")]
    public GameObject adventurerIDHolder;
    public GameObject questHolder;
    public TMP_Text nameText;
    public TMP_Text traitsText;
    public TMP_Text questNameText;
    public TMP_Text questDescriptionText;

    [Header("Ahwoooooooo")]
    public GameObject Image;
    public AudioSource audioSource;
    public AudioClip ahwooooooSfx;

    private List<Adventurer> activeAdventurers = new();
    private bool counterOccupied = false;
    private bool isDayOver = false;

    private Adventurer currentAdventurerAtCounter = null;
    private QuestData currentProvidedQuest = null;

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
        new("Herbalistís Request", "Gather rare herbs in the nearby meadow for the local apothecary. Low danger, good for beginners."),
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

    public static GameManager Instance { get; private set; }
    public Adventurer CurrentAdventurer => currentAdventurerAtCounter;

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
        while (true)
        {
            float waitTime = UnityEngine.Random.Range(20f, 40f);
            yield return new WaitForSeconds(waitTime);

            if (isDayOver) continue;

            DateTime currentTime = timeManager.GetCurrentTime();
            if (currentTime.TimeOfDay >= new TimeSpan(10, 0, 0) &&
                currentTime.TimeOfDay <= new TimeSpan(22, 30, 0))
            {
                GameObject adventurerObj = Instantiate(adventurerPrefab, adventurerEntry.position, Quaternion.identity);
                string randomName = possibleNames[UnityEngine.Random.Range(0, possibleNames.Length)];
                Adventurer adventurer = new(adventurerObj, randomName);
                adventurer.traits.AddRange(RandomizeTraits(allPositiveTraits, 3));
                adventurer.traits.AddRange(RandomizeTraits(allNegativeTraits, 3));
                activeAdventurers.Add(adventurer);

                Transform target = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)];
                StartCoroutine(MoveToPosition(adventurer.gameObject, target.position));
                StartCoroutine(AttemptMoveToCounter(adventurer, UnityEngine.Random.Range(10f, 25f)));
            }
        }
    }

    List<Trait> RandomizeTraits(List<Trait> pool, int max)
    {
        List<Trait> selected = new();
        int count = UnityEngine.Random.Range(0, max + 1);
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

        adventurerIDHolder.SetActive(true);
        questHolder.SetActive(true);

        nameText.text = adventurer.name;
        traitsText.text = GetTraitsString(adventurer);

        // Assign adventurer's quest
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
            GameResultsManager.Instance.AddEarnings(100);

        CompleteQuestAndSendAdventurer();
    }

    public void CompleteQuestAndSendAdventurer()
    {
        if (audioSource != null && ahwooooooSfx != null)
            audioSource.PlayOneShot(ahwooooooSfx);

        if (currentAdventurerAtCounter != null)
            StartCoroutine(MoveAdventurerBack(currentAdventurerAtCounter));
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
        nameText.text = "";
        traitsText.text = "";
        questNameText.text = "";
        questDescriptionText.text = "";

        Image.SetActive(false);
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
