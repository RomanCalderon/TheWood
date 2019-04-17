using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DropZone : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public delegate void DropZoneHandler(DropZone dropZone);
    public static event DropZoneHandler OnDropZoneEnter;
    public static event DropZoneHandler OnDropZoneExit;

    public Transform contentHolder;

    public void OnPointerEnter(PointerEventData eventData)
    {
        OnDropZoneEnter?.Invoke(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        OnDropZoneExit?.Invoke(this);
    }
}
