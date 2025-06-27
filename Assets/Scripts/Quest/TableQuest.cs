using UnityEngine;
using TMPro;

public class TableQuest : MonoBehaviour
{
    [Header("UI Elements")]
    public TMP_Text questNameText;
    public TMP_Text questDescriptionText;

    [Header("Quest Info")]
    public string questName;
    [TextArea]
    public string questDescription;

    private bool isStamped = false;

    void Start()
    {
        if (questNameText != null) questNameText.text = questName;
        if (questDescriptionText != null) questDescriptionText.text = questDescription;
    }

    public void OnStamped()
    {
        if (isStamped) return;
        isStamped = true;

        QuestData questData = new QuestData(questName, questDescription);
        GameManager.Instance.StampQuest(questData);
    }
}
