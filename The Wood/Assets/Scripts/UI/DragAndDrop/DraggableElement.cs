using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableElement : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    public delegate void DraggableElementHandler(Transform transform);
    public static event DraggableElementHandler OnDraggableElementEnter;
    public static event DraggableElementHandler OnDraggableElementExit;

    GameObject placeholder;
    DropZone dropZone = null;
    bool isDragging = false;
    CanvasGroup canvasGroup;
    Canvas rootCanvas;

    void Awake()
    {
        DropZone.OnDropZoneEnter += DropZone_OnDropZoneEnter;
        DropZone.OnDropZoneExit += DropZone_OnDropZoneExit;
        OnDraggableElementEnter += DraggableElement_OnDraggableElementEnter;
        OnDraggableElementExit += DraggableElement_OnDraggableElementExit;
    }

    void Start()
    {
        rootCanvas = GetRootCanvas();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // Create a placeholder in original position
        placeholder = new GameObject(gameObject.name + " (Placeholder)");
        placeholder.AddComponent<RectTransform>();
        placeholder.transform.SetParent(transform.parent, false);
        placeholder.transform.SetSiblingIndex(transform.GetSiblingIndex());
        
        transform.SetParent(rootCanvas.transform);
        transform.position = eventData.position;

        // Add a canvas group so this element won't block pointer raycasts
        canvasGroup = gameObject.AddComponent<CanvasGroup>();
        canvasGroup.blocksRaycasts = false;

        isDragging = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (dropZone == null)
        {
            // Put this element in the same location as the placeholder's
            transform.SetParent(placeholder.transform.parent);
            transform.SetSiblingIndex(placeholder.transform.GetSiblingIndex());
        }
        // Otherwise, put this element in the DropZone's content holder
        else
        {
            transform.SetParent(dropZone.contentHolder, false);
            transform.SetSiblingIndex(placeholder.transform.GetSiblingIndex());
        }
        
        // Destroy the placeholder in original position
        Destroy(placeholder.gameObject);
        placeholder = null;

        // Reomve the canvas group
        Destroy(canvasGroup);
        canvasGroup = null;

        isDragging = false;
    }

    // Event Listeners
    private void DropZone_OnDropZoneEnter(DropZone dropZone)
    {
        if (isDragging)
        {
            // Set the new drop zone
            this.dropZone = dropZone;

            // Move the placeholder to new drop zone
            placeholder.transform.SetParent(dropZone.contentHolder, false);
        }
    }

    private void DropZone_OnDropZoneExit(DropZone dropZone)
    {
        if (isDragging)
            this.dropZone = null;
    }

    private void DraggableElement_OnDraggableElementEnter(Transform transform)
    {
        if (isDragging)
            placeholder.transform.SetSiblingIndex(transform.GetSiblingIndex());
    }

    private void DraggableElement_OnDraggableElementExit(Transform transform)
    {

    }

    // Events
    public void OnPointerEnter(PointerEventData eventData)
    {
        OnDraggableElementEnter?.Invoke(transform);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        OnDraggableElementExit?.Invoke(transform);
    }


    private Canvas GetRootCanvas()
    {
        Canvas[] parentCanvases = GetComponentsInParent<Canvas>();
        if (parentCanvases != null && parentCanvases.Length > 0)
            return parentCanvases[parentCanvases.Length - 1];

        return null;
    }

    void OnDisable()
    {
        DropZone.OnDropZoneExit -= DropZone_OnDropZoneEnter;
    }
}
