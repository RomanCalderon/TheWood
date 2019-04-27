using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class UIFadeIn : MonoBehaviour
{
    CanvasGroup canvasGroup;

    [SerializeField] float fadeDelay;
    [SerializeField] float fadeSpeed = 1;

    // Start is called before the first frame update
    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0;

        StartCoroutine(FadeInDelay());
    }

    IEnumerator FadeInDelay()
    {
        yield return new WaitForSeconds(fadeDelay);

        while(canvasGroup.alpha < 1f)
        {
            canvasGroup.alpha += Time.deltaTime * fadeSpeed;
            yield return null;
        }
    }
}
