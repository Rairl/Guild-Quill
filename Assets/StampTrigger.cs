using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StampTrigger : MonoBehaviour, IEndDragHandler
{
    public RectTransform targetImage;      // Image to deactivate
    public GameObject stampedImage;        // Image to activate

    public void OnEndDrag(PointerEventData eventData)
    {
        if (IsOverlapping(this.GetComponent<RectTransform>(), targetImage))
        {
            targetImage.gameObject.SetActive(false);
            stampedImage.SetActive(true);
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
