using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

[Serializable]
public class DropRequestEvent : UnityEvent<GameObject, DropZone.DropZoneIDs, Action> { }

[Serializable]
public class DropZoneEvent : UnityEvent<GameObject, DropZone.DropZoneIDs> { }

public class DropZone : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public DropRequestEvent OnElementDropRequested;
    private Action<DropZone> dropZoneRequest;

    public DropZoneEvent OnElementAdded;

    public delegate void DropZoneHandler(DropZone dropZone);
    public static event DropZoneHandler OnPointerEnterDropZone;
    public static event DropZoneHandler OnPointerExitDropZone;

    public enum DropZoneIDs
    {
        DROPZONE1,
        DROPZONE2,
        DROPZONE3,
        DROPZONE4,
        DROPZONE5,
        DROPZONE6,
        DROPZONE7,
        DROPZONE8,
        DROPZONE9
    }

    public DropZoneIDs DropZoneID;
    public Transform contentHolder;

    private void Awake ()
    {
        foreach (Transform t in contentHolder)
        {
            DraggableElement draggableElement = t.GetComponent<DraggableElement>();

            if (draggableElement != null)
                draggableElement.dropZone = this;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        OnPointerEnterDropZone?.Invoke(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        OnPointerExitDropZone?.Invoke(this);
    }

    public void RequestElementDrop(GameObject obj, Action<DropZone> callback)
    {
        if (callback == null)
            return;

        dropZoneRequest = callback;
        OnElementDropRequested?.Invoke(obj, DropZoneID, ValidDropZone);
    }

    private void ValidDropZone()
    {
        dropZoneRequest(this);
    }

    public void AddElement(GameObject obj)
    {
        OnElementAdded?.Invoke(obj, DropZoneID);
    }
}
