using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConsumableController : MonoBehaviour
{
    CharacterStats stats;

    // Start is called before the first frame update
    void Start()
    {
        stats = GetComponent<Player>().CharacterStats;
    }

    public void ConsumeItem(Item item)
    {
        GameObject itemToSpawn = Instantiate(Resources.Load<GameObject>("Consumables/" + item.ItemSlug));

        if (item.ItemModifier)
            itemToSpawn.GetComponent<IConsumable>().Consume(stats);
        else
            itemToSpawn.GetComponent<IConsumable>().Consume();
    }
}
