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
    [HideInInspector] public DropZone dropZone = null;
    bool isDragging = false;
    CanvasGroup canvasGroup;
    Canvas rootCanvas;

    void Awake()
    {
        DropZone.OnPointerEnterDropZone += DropZone_OnPointerEnter;
        DropZone.OnPointerExitDropZone += DropZone_OnPointereExit;
        OnDraggableElementEnter += DraggableElementEnter;
        OnDraggableElementExit += DraggableElementExit;
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
        // Failed drop
        if (dropZone == null)
        {
            // Put this element in the same location as the placeholder's
            transform.SetParent(placeholder.transform.parent);
            transform.SetSiblingIndex(placeholder.transform.GetSiblingIndex());
        }
        // Successful drop
        else
        {
            transform.SetParent(dropZone.contentHolder, false);
            transform.SetSiblingIndex(placeholder.transform.GetSiblingIndex());

            dropZone.AddElement(gameObject);
        }
        
        // Destroy the placeholder in original position
        Destroy(placeholder.gameObject);
        placeholder = null;

        // Remove the canvas group
        Destroy(canvasGroup);
        canvasGroup = null;

        isDragging = false;
    }

    // Event Listeners
    /// <summary>
    /// Called when the pointer enters a DropZone.
    /// </summary>
    /// <param name="dropZone">DropZone the pointer entered.</param>
    private void DropZone_OnPointerEnter(DropZone dropZone)
    {
        if (isDragging)
        {
            // Check if dropZone is valid
            dropZone.RequestElementDrop(gameObject, ValidDropZone);

            // Set the new drop zone
            //this.dropZone = dropZone;

            // Move the placeholder to new drop zone
            //placeholder.transform.SetParent(dropZone.contentHolder, false);
        }
    }

    private void DropZone_OnPointereExit(DropZone dropZone)
    {
        if (isDragging)
            this.dropZone = null;
    }

    private void ValidDropZone(DropZone newDropZone)
    {
        // Set the new drop zone
        dropZone = newDropZone;

        // Move the placeholder to new drop zone
        placeholder.transform.SetParent(dropZone.contentHolder, false);
    }

    /// <summary>
    /// Moves the placeholder relative to this when being dragged.
    /// </summary>
    /// <param name="transform"></param>
    private void DraggableElementEnter(Transform transform)
    {
        if (isDragging)
            placeholder.transform.SetSiblingIndex(transform.GetSiblingIndex());
    }

    private void DraggableElementExit(Transform transform)
    {

    }

    // Events
    public void OnPointerEnter(PointerEventData eventData)
    {
        OnDraggableElementEnter(transform);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        OnDraggableElementExit(transform);
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
        DropZone.OnPointerExitDropZone -= DropZone_OnPointerEnter;
    }
}
