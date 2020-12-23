using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject monster;
    public bool respawn;
    public float spawnDelay;
    private float currentTime;
    private bool spawning;


    // Start is called before the first frame update
    void Start()
    {
        Spawn();
        currentTime = spawnDelay;
    }

    // Update is called once per frame
    void Update()
    {
        if (spawning)
        {
            currentTime -= Time.deltaTime;

            if (currentTime <= 0)
                Spawn();
        }
    }

    public void Respawn()
    {
        currentTime = spawnDelay;
        spawning = true;
    }

    void Spawn()
    {
        Hostile instance = Instantiate(monster, transform.position, Quaternion.identity).GetComponent<Hostile>();
        instance.Spawner = this;

        spawning = false;
    }
}
