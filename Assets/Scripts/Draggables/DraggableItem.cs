using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;
    private Canvas canvas;

    public RectTransform dragArea; // assign brown area in Inspector

    private bool isDraggingAllowed = false;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // Only allow drag if started inside the drag area
        isDraggingAllowed = RectTransformUtility.RectangleContainsScreenPoint(dragArea, eventData.position, eventData.pressEventCamera);
        if (isDraggingAllowed)
        {
            canvasGroup.blocksRaycasts = false;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDraggingAllowed) return;

        Vector2 localMousePos;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform, eventData.position, eventData.pressEventCamera, out localMousePos))
        {
            Vector2 newPos = ClampToDragArea(localMousePos);
            rectTransform.anchoredPosition = newPos;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!isDraggingAllowed) return;

        canvasGroup.blocksRaycasts = true;
        isDraggingAllowed = false;
    }

    private Vector2 ClampToDragArea(Vector2 desiredPosition)
    {
        Vector2 clampedPosition = desiredPosition;

        // Convert corners of dragArea to local canvas space
        Vector3[] areaCorners = new Vector3[4];
        dragArea.GetWorldCorners(areaCorners);

        Vector2 minCorner = RectTransformUtility.WorldToScreenPoint(null, areaCorners[0]);
        Vector2 maxCorner = RectTransformUtility.WorldToScreenPoint(null, areaCorners[2]);

        RectTransform canvasRect = canvas.transform as RectTransform;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, minCorner, null, out minCorner);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, maxCorner, null, out maxCorner);

        Vector2 itemSize = rectTransform.sizeDelta;
        Vector2 pivotOffset = new Vector2(itemSize.x * rectTransform.pivot.x, itemSize.y * rectTransform.pivot.y);

        float minX = minCorner.x + pivotOffset.x;
        float maxX = maxCorner.x - (itemSize.x - pivotOffset.x);
        float minY = minCorner.y + pivotOffset.y;
        float maxY = maxCorner.y - (itemSize.y - pivotOffset.y);

        clampedPosition.x = Mathf.Clamp(clampedPosition.x, minX, maxX);
        clampedPosition.y = Mathf.Clamp(clampedPosition.y, minY, maxY);

        return clampedPosition;
    }
}