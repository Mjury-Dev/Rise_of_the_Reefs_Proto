using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropRateManager : MonoBehaviour
{
    [System.Serializable]
    public class Drops
    {
        public string name;
        public GameObject itemPrefab;
        public float dropRate;
    }

    public List<Drops> drops;

    void OnDestroy()
    {
        if (!gameObject.scene.isLoaded)
            return;

        float totalWeight = 0f;
        foreach (Drops drop in drops)
        {
            totalWeight += drop.dropRate;
        }

        float randomValue = Random.Range(0f, totalWeight);
        float cumulative = 0f;

        foreach (Drops drop in drops)
        {
            cumulative += drop.dropRate;
            if (randomValue <= cumulative)
            {
                Instantiate(drop.itemPrefab, transform.position, Quaternion.identity);
                break;
            }
        }
    }

}
