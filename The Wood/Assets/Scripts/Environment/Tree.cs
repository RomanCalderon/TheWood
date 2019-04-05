using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree : MonoBehaviour
{
    [SerializeField] ParticleSystemRenderer leafParticleSystemRenderer;
    [SerializeField] bool randRotate;
    [SerializeField] Vector2 randScaleRange = Vector2.one;
    [Tooltip("0 - Spring\n1 - Summer\n2 - Autumn")]
    [SerializeField] Material[] seasonalLeafMats;
    [SerializeField] GameObject[] seasonalTreeModels;

    private void Awake()
    {
        DayNightManager.OnUpdateSeason += UpdateSeason;
    }

    private void Start()
    {
        foreach (GameObject tree in seasonalTreeModels)
            tree.SetActive(false);
    }

    public void UpdateSeason(Seasons season)
    {
        // Set proper leaf material based on current season
        if (season != Seasons.WINTER)
            leafParticleSystemRenderer.material = seasonalLeafMats[(int)season];
        else
            leafParticleSystemRenderer.enabled = false;

        // Set the proper tree model based on current season
        seasonalTreeModels[(int)season].SetActive(true);

        // If randRotate is true, rotate the tree
        if (randRotate)
        {
            transform.eulerAngles = new Vector3(0, Random.Range(0f, 360f), 0);
        }

        // Apply a random scale between randScaleRange.x and randScaleRange.y
        transform.localScale = Vector3.one * Random.Range(randScaleRange.x, randScaleRange.y);
    }
}
