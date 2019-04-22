using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SignInteractions : MonoBehaviour
{
    [SerializeField] Camera mainCamera;
    [SerializeField] LayerMask signLayerMask;
    Transform currentSign;
    [SerializeField] float hoverForce = 250;
    [SerializeField] float clickForce = 400;


    // Update is called once per frame
    void Update()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, 25, signLayerMask))
        {
            if (currentSign != hit.transform)
            {
                currentSign = hit.transform;

                // Apply a small force
                hit.transform.GetComponent<Rigidbody>().AddForce(Vector3.forward * hoverForce);
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(ray, out hit, 25, signLayerMask))
            {
                // Apply a small force
                hit.transform.GetComponent<Rigidbody>().AddForce(Vector3.forward * clickForce);

                if (hit.transform.tag == "NewGameSign")
                    print(hit.transform.tag);
                else if (hit.transform.tag == "ContinueSign")
                    print(hit.transform.tag);
                else if (hit.transform.tag == "OptionsSign")
                    print(hit.transform.tag);
                else if (hit.transform.tag == "QuitSign")
                    print(hit.transform.tag);
            }
        }
    }
}
