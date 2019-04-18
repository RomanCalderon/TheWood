using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

[Serializable]
public class DropZoneEvent : UnityEvent<GameObject, bool> { }

public class DropZone : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public DropZoneEvent OnElementAdded;

    public delegate void DropZoneHandler(DropZone dropZone);
    public static event DropZoneHandler OnDropZoneEnter;
    public static event DropZoneHandler OnDropZoneExit;

    public bool storage;
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
        OnDropZoneEnter?.Invoke(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        OnDropZoneExit?.Invoke(this);
    }

    public void AddElement(GameObject obj)
    {
        OnElementAdded?.Invoke(obj, storage);
    }
}
