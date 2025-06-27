using UnityEngine;
using UnityEngine.EventSystems;

public class StampTrigger : MonoBehaviour, IEndDragHandler
{
    public RectTransform targetImage;
    public AudioSource audioSource;
    public AudioClip stampSfx;

    void Start()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Quest[] allQuests = FindObjectsOfType<Quest>();

        foreach (var quest in allQuests)
        {
            RectTransform questRect = quest.GetComponent<RectTransform>();
            if (questRect != null && IsOverlapping(GetComponent<RectTransform>(), questRect))
            {
                targetImage.gameObject.SetActive(false);

                if (audioSource != null && stampSfx != null)
                    audioSource.PlayOneShot(stampSfx);

                // Instead of quest.OnStamped(), call GameManager.StampQuest with the quest info
                string qName = quest.questNameText.text;
                string qDesc = quest.questDescriptionText.text;
                QuestData data = new QuestData(qName, qDesc);

                GameManager.Instance.StampQuest(data);

                // Optional: mark the quest as stamped visually if needed
                quest.OnStamped();

                break;
            }
        }
    }

    private void PlayStampSFX()
    {
        if (audioSource != null && stampSfx != null)
            audioSource.PlayOneShot(stampSfx);
    }

    private bool IsOverlapping(RectTransform a, RectTransform b)
    {
        Rect aRect = GetWorldRect(a);
        Rect bRect = GetWorldRect(b);
        return aRect.Overlaps(bRect);
    }

    private Rect GetWorldRect(RectTransform rt)
    {
        Vector3[] corners = new Vector3[4];
        rt.GetWorldCorners(corners);
        return new Rect(corners[0], corners[2] - corners[0]);
    }
}
