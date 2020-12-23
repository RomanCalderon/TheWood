using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Funshroom : MonoBehaviour, IConsumable
{
    public void Consume()
    {
        Debug.Log("You consumed a funshroom. Wooo!");
        Destroy(gameObject, 3f);
    }

    public void Consume(CharacterStats stats)
    {
        Consume();

        // Apply stats
    }
}
