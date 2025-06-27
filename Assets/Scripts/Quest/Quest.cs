using UnityEngine;
using TMPro;

public class Quest : MonoBehaviour
{
    [Header("UI Elements")]
    public TMP_Text questNameText;
    public TMP_Text questDescriptionText;

    [HideInInspector] public Adventurer currentAdventurer;

    private bool isStamped = false;

    public void AssignQuest(string name, string description)
    {
        isStamped = false;

        if (questNameText != null)
            questNameText.text = name;

        if (questDescriptionText != null)
            questDescriptionText.text = description;
    }

    public void ResetQuest()
    {
        isStamped = false;
        if (questNameText != null) questNameText.text = "";
        if (questDescriptionText != null) questDescriptionText.text = "";
    }

    public void OnStamped()
    {
        if (isStamped) return;
        isStamped = true;

        float baseSuccessRate = 0.8f;
        float traitBonus = 0f;

        if (currentAdventurer != null)
        {
            foreach (var trait in currentAdventurer.traits)
            {
                if (trait.name == "Reliable") traitBonus += 0.05f;
            }
        }

        float successRate = Mathf.Clamp01(baseSuccessRate + traitBonus);
        bool success = Random.value <= successRate;

        if (success)
        {
            GameResultsManager.Instance.AddEarnings(100);
        }

        FindObjectOfType<GameManager>().CompleteQuestAndSendAdventurer();
    }
}
