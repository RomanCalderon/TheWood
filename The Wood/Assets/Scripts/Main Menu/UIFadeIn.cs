using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class UIFadeIn : MonoBehaviour
{
    CanvasGroup canvasGroup;

    [SerializeField] float fadeDelay;
    [SerializeField] float fadeSpeed = 1;

    Coroutine fadeCoroutine;
    
    void OnEnable()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0;

        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(FadeInDelay());
    }

    IEnumerator FadeInDelay()
    {
        yield return new WaitForSeconds(fadeDelay);

        while(canvasGroup.alpha < 1f)
        {
            canvasGroup.alpha += Time.deltaTime * fadeSpeed;
            yield return null;
        }

        fadeCoroutine = null;
    }

    private void OnDisable()
    {
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = null;
    }
}
