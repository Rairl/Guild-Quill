using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("Adventurer Settings")]
    public GameObject adventurerPrefab;
    public Transform adventurerEntry;
    public Transform adventurerCounter;
    public Transform[] spawnPoints; // Assign 4 spawn points in the Inspector

    [Header("Time Management")]
    public TimeManager timeManager; // Reference to your TimeManager script

    [Header("Spawn Timing")]
    public float minSpawnInterval = 5f;
    public float maxSpawnInterval = 15f;

    [Header("UI Elements")]
    public GameObject adventurerIDHolder;
    public TMP_Text rankText;
    public TMP_Text nameText;

    [Header("Rank Buttons")]
    public Button sRankButton;
    public Button aRankButton;
    public Button bRankButton;
    public Button cRankButton;
    public Button dRankButton;

    private List<Adventurer> activeAdventurers = new List<Adventurer>();
    private bool counterOccupied = false;
    private Adventurer currentAdventurerAtCounter = null;

    private string[] possibleNames = { "Kazuma", "Aqua", "Megumin", "Darkness", "Yunyun", "Chris", "Wiz", "Vanir", "Dust", "Kyouya" };
    private string[] possibleRanks = { "S", "A", "B", "C", "D" };

    void Start()
    {
        StartCoroutine(SpawnAdventurers());

        // Assign button click listeners
        sRankButton.onClick.AddListener(() => OnRankButtonClicked("S"));
        aRankButton.onClick.AddListener(() => OnRankButtonClicked("A"));
        bRankButton.onClick.AddListener(() => OnRankButtonClicked("B"));
        cRankButton.onClick.AddListener(() => OnRankButtonClicked("C"));
        dRankButton.onClick.AddListener(() => OnRankButtonClicked("D"));
    }

    IEnumerator SpawnAdventurers()
    {
        while (true)
        {
            // Wait for a random interval between spawns
            float waitTime = UnityEngine.Random.Range(minSpawnInterval, maxSpawnInterval);
            yield return new WaitForSeconds(waitTime);

            // Check if current time is between 10:00 AM and 10:30 PM
            DateTime currentTime = timeManager.GetCurrentTime();
            if (currentTime.TimeOfDay >= new TimeSpan(10, 0, 0) && currentTime.TimeOfDay <= new TimeSpan(22, 30, 0))
            {
                // Spawn Adventurer at the entry point
                GameObject adventurerObj = Instantiate(adventurerPrefab, adventurerEntry.position, Quaternion.identity);
                string randomName = possibleNames[UnityEngine.Random.Range(0, possibleNames.Length)];
                string randomRank = possibleRanks[UnityEngine.Random.Range(0, possibleRanks.Length)];
                Adventurer adventurer = new Adventurer(adventurerObj, randomName, randomRank);
                activeAdventurers.Add(adventurer);

                // Move to a random spawn point
                Transform targetSpawnPoint = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)];
                StartCoroutine(MoveToPosition(adventurer.gameObject, targetSpawnPoint.position));

                // After 10 - 20 seconds, attempt to move to the counter
                float delay = UnityEngine.Random.Range(10f, 20f);
                StartCoroutine(AttemptMoveToCounter(adventurer, delay));
            }
        }
    }

    IEnumerator MoveToPosition(GameObject obj, Vector3 targetPosition)
    {
        float speed = 2f;
        while (Vector3.Distance(obj.transform.position, targetPosition) > 0.1f)
        {
            obj.transform.position = Vector3.MoveTowards(obj.transform.position, targetPosition, speed * Time.deltaTime);
            yield return null;
        }
    }

    IEnumerator AttemptMoveToCounter(Adventurer adventurer, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (!counterOccupied)
        {
            counterOccupied = true;
            currentAdventurerAtCounter = adventurer;
            yield return StartCoroutine(MoveToPosition(adventurer.gameObject, adventurerCounter.position));
            // Display Adventurer ID
            adventurerIDHolder.SetActive(true);
            rankText.text = adventurer.rank;
            nameText.text = adventurer.name;
        }
        else
        {
            // Wait and retry after a short delay
            yield return new WaitForSeconds(10f);
            StartCoroutine(AttemptMoveToCounter(adventurer, 0f));
        }
    }

    void OnRankButtonClicked(string selectedRank)
    {
        if (currentAdventurerAtCounter != null)
        {
            if (currentAdventurerAtCounter.rank == selectedRank)
            {
                // Correct rank selected
                StartCoroutine(MoveAdventurerBack(currentAdventurerAtCounter));
                adventurerIDHolder.SetActive(false);
                rankText.text = "";
                nameText.text = "";
                counterOccupied = false;
                currentAdventurerAtCounter = null;
            }
            else
            {
                // Incorrect rank selected
                Debug.Log("Incorrect rank selected.");
            }
        }
    }

    IEnumerator MoveAdventurerBack(Adventurer adventurer)
    {
        yield return StartCoroutine(MoveToPosition(adventurer.gameObject, adventurerEntry.position));
        activeAdventurers.Remove(adventurer);
        Destroy(adventurer.gameObject);
    }
}

public class Adventurer
{
    public GameObject gameObject;
    public string name;
    public string rank;

    public Adventurer(GameObject obj, string name, string rank)
    {
        this.gameObject = obj;
        this.name = name;
        this.rank = rank;
    }
}

