using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
[ExecuteInEditMode]
public class Rope : MonoBehaviour
{
    LineRenderer lineRenderer;
    
    [SerializeField] Transform startPoint;
    [SerializeField] Transform endPoint;
    
    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2;

        UpdateAnchors();
    }


    // Update is called once per frame
    void Update()
    {
        UpdateAnchors();
        
        // Place this Rope object in the middle of the rope
        if (startPoint != null && endPoint != null)
            transform.position = (startPoint.position + endPoint.position) / 2;
    }

    /// <summary>
    /// Updates the starting and ending points of the line renderer
    /// </summary>
    void UpdateAnchors()
    {
        lineRenderer.SetPosition(0, startPoint.position);
        lineRenderer.SetPosition(1, endPoint.position);
    }
}
