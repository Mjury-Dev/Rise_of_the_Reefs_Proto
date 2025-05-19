using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropRandomizer : MonoBehaviour
{
    public List<GameObject> propSpawnPoints;
    public List<GameObject> propPrefabs;

    void Start()
    {
        if (propSpawnPoints == null || propPrefabs == null)
        {
            Debug.LogError("PropRandomizer: Missing spawn points or prop prefabs list.");
            return;
        }

        SpawnProps();
    }

    void SpawnProps()
    {
        foreach (GameObject sp in propSpawnPoints)
        {
            if (sp == null)
            {
                Debug.LogWarning("PropRandomizer: Found null spawn point in list.");
                continue;
            }

            if (propPrefabs.Count == 0)
            {
                Debug.LogError("PropRandomizer: No prop prefabs assigned.");
                return;
            }

            int rand = Random.Range(0, propPrefabs.Count);
            GameObject prefabToSpawn = propPrefabs[rand];

            if (prefabToSpawn == null)
            {
                Debug.LogWarning("PropRandomizer: Null prefab found in list.");
                continue;
            }

            GameObject prop = Instantiate(prefabToSpawn, sp.transform.position, Quaternion.identity);
            prop.transform.SetParent(sp.transform);  // safer than setting .parent directly
        }
    }
}

