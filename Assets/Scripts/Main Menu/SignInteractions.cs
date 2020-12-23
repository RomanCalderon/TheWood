using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ChrisTutorials.Persistent;

public class SignInteractions : MonoBehaviour
{
    public delegate void ButtonPressHandler();
    public static event ButtonPressHandler OnNewGamePressed;
    public static event ButtonPressHandler OnContinueGamePressed;
    public static event ButtonPressHandler OnOptionsPressed;
    public static event ButtonPressHandler OnQuitPressed;

    [SerializeField] Camera mainCamera;
    [SerializeField] LayerMask signLayerMask;
    bool hovered = false;
    [SerializeField] float hoverForce = 250;
    [SerializeField] float clickForce = 400;
    [SerializeField] AudioClip hoverSound;
    [SerializeField] AudioClip clickSound;


    // Update is called once per frame
    void Update()
    {
        if (!MainMenuManager.MenuOpen && !SceneLoader.IsLoadingScene)
        {
            Hover();
            Click();
        }
    }

    void Hover()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, 25, signLayerMask))
        {
            if (hovered)
                return;

            // Apply a small force
            hit.transform.GetComponent<Rigidbody>().AddForce(Vector3.forward * hoverForce);

            // Play sfx
            AudioManager.Instance.Play(hoverSound, transform).spatialBlend = 0;

            hovered = true;
        }
        else
        {
            hovered = false;
        }
    }

    void Click()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        if (Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(ray, out RaycastHit hit, 25, signLayerMask))
            {
                // Apply a small force
                hit.transform.GetComponent<Rigidbody>().AddForce(Vector3.forward * clickForce);

                // Play click sfx
                AudioManager.Instance.Play(clickSound, transform).spatialBlend = 0;

                if (hit.transform.tag == "NewGameSign")
                    OnNewGamePressed?.Invoke();
                else if (hit.transform.tag == "ContinueSign")
                    OnContinueGamePressed?.Invoke();
                else if (hit.transform.tag == "OptionsSign")
                    OnOptionsPressed?.Invoke();
                else if (hit.transform.tag == "QuitSign")
                    OnQuitPressed?.Invoke();
            }
        }
    }
}
