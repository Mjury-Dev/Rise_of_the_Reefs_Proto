using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwarmEnemySpawner : SpawnerProp
{
    public float maximumPollutionDecrease = 2.0f;
    public float minimumPollutionDecrease = 0.6f;
    private void OnDestroy()
    {
        // Generate a random pollution decrease amount between 0.2 and 0.6
        float pollutionDecreaseAmount = Random.Range(maximumPollutionDecrease, minimumPollutionDecrease);

        // Decrease pollution by the random amount
        PollutionManager.instance.DecreasePollution(pollutionDecreaseAmount);

        // Update the pollution level in the GameManager
        GameManager.instance.UpdatePollutionLevel(PollutionManager.instance.PollutionLevel);
    }
}