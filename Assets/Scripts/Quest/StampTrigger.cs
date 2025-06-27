using UnityEngine;
using UnityEngine.EventSystems;

public class StampTrigger : MonoBehaviour, IEndDragHandler
{
    public RectTransform targetImage;
    public GameObject stampedImage;
    public Quest questScript;

    public void OnEndDrag(PointerEventData eventData)
    {
        if (IsOverlapping(GetComponent<RectTransform>(), targetImage))
        {
            targetImage.gameObject.SetActive(false);
            stampedImage.SetActive(true);
            questScript?.OnStamped();
        }
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
