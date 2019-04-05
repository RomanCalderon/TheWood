using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHouseGenerator : MonoBehaviour
{
    [SerializeField] Vector2 houseSpawnBounds;
    [SerializeField] GameObject bedPrefab;
    [SerializeField] LayerMask terrainLayerMask;

    // Start is called before the first frame update
    void Start()
    {
        GenerateHouse();
    }

    void GenerateHouse()
    {

        Vector3 rayPos = transform.position + new Vector3(Random.Range(-houseSpawnBounds.x / 2, houseSpawnBounds.x / 2), 50f, Random.Range(-houseSpawnBounds.y / 2, houseSpawnBounds.y / 2));
        Ray ray = new Ray(rayPos, Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hit, 75f, terrainLayerMask))
            Instantiate(bedPrefab, hit.point, Quaternion.identity);
        else
            Debug.LogError("House spawn point is null");
        
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(transform.position, new Vector3(houseSpawnBounds.x, 0f, houseSpawnBounds.y));
    }
}
