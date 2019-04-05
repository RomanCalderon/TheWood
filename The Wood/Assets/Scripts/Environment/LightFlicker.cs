using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Light))]
public class LightFlicker : MonoBehaviour
{
    Light lightSource;
    [SerializeField]
    Vector2 flickerRange = new Vector2(0.8f, 1.2f);
    [SerializeField]
    float changeSpeed = 1f;
    [SerializeField]
    float rateDampening = 0.1f;
    float targetIntensity;

    void Start()
    {
        lightSource = GetComponent<Light>();
        
        StartCoroutine(DoFlicker());
    }

    private void Update()
    {
        lightSource.intensity = Mathf.Lerp(lightSource.intensity, targetIntensity, changeSpeed * Time.deltaTime);
    }

    private IEnumerator DoFlicker()
    {
        while (true)
        {
            targetIntensity = Random.Range(flickerRange.x, flickerRange.y);
            yield return new WaitForSeconds(rateDampening);
        }
    }
}
