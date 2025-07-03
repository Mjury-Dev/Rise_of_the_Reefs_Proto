using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Spawns a given prefab in a radius around itself at a regular interval,
/// up to a maximum number of instances. Only spawns when the player is nearby.
/// This is a base class that can be inherited from.
/// </summary>
public class SpawnerProp : MonoBehaviour
{
    /// <summary>
    /// Defines the method used to determine spawn locations.
    /// </summary>
    public enum SpawnerType
    {
        Radius,
        SetSpawn
    }

    [Header("Spawning Settings")]
    [Tooltip("The method to use for spawning: in a radius or at a fixed point.")]
    public SpawnerType spawnType = SpawnerType.Radius;

    [Tooltip("The prefab to be spawned.")]
    public GameObject prefabToSpawn;

    [Tooltip("The radius around this object where prefabs can spawn. (Used for Radius type only)")]
    public float spawnRadius = 5f;

    [Tooltip("The vertical offset from the spawner's position. (Used for SetSpawn type only)")]
    public float yOffset = 1f;

    [Tooltip("The time in seconds between each spawn attempt.")]
    public float spawnInterval = 2f;

    [Tooltip("The maximum number of spawned prefabs allowed to exist at one time.")]
    public int maxSpawns = 10;

    [Header("Activation Settings")]
    [Tooltip("The spawner will only be active if the player is within this radius. Set to 0 to disable this check.")]
    public float activationRadius = 25f;

    // A list to keep track of all the active spawned objects.
    // Protected so child classes can access it.
    protected List<GameObject> activeSpawns = new List<GameObject>();
    // A reference to the player's transform.
    protected Transform playerTransform;

    /// <summary>
    /// Called when the script instance is being loaded.
    /// Marked as virtual so it can be extended by a child class.
    /// </summary>
    protected virtual void Start()
    {
        // Find the player object by its tag. Ensure your player GameObject has the "Player" tag.
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            playerTransform = playerObject.transform;
        }
        else
        {
            Debug.LogWarning("SpawnerProp: Could not find a GameObject with the 'Player' tag. Activation radius will be ignored.", this);
        }

        // Check if the prefab has been assigned to prevent errors.
        if (prefabToSpawn == null)
        {
            Debug.LogError("Prefab to Spawn has not been assigned in the spawner!", this);
            return; // Stop the script if no prefab is assigned.
        }

        // Start the continuous spawning process.
        StartCoroutine(SpawnRoutine());
    }

    /// <summary>
    /// A coroutine that handles the continuous spawning logic.
    /// </summary>
    private IEnumerator SpawnRoutine()
    {
        // This loop will run for the entire lifetime of this object.
        while (true)
        {
            // Wait for the specified interval before trying to spawn again.
            yield return new WaitForSeconds(spawnInterval);

            // Only proceed if the player is within the activation radius.
            if (IsPlayerInRange())
            {
                // Clean up any spawned objects that might have been destroyed by other means.
                activeSpawns.RemoveAll(item => item == null);

                // Only spawn a new object if we are under the maximum limit.
                if (activeSpawns.Count < maxSpawns)
                {
                    SpawnPrefab();
                }
            }
        }
    }

    /// <summary>
    /// Checks if the player is within the activation radius.
    /// </summary>
    /// <returns>True if the player is in range or if the check is disabled.</returns>
    protected virtual bool IsPlayerInRange()
    {
        // If activationRadius is 0 or less, or if the player wasn't found, the check is disabled.
        if (activationRadius <= 0f || playerTransform == null)
        {
            return true;
        }

        // Return true if the distance to the player is less than or equal to the activation radius.
        return Vector3.Distance(transform.position, playerTransform.position) <= activationRadius;
    }

    /// <summary>
    /// Handles the instantiation of a single prefab.
    /// Marked as virtual so it can be overridden by a child class.
    /// </summary>
    protected virtual void SpawnPrefab()
    {
        Vector3 spawnPosition;

        // Determine the spawn position based on the selected SpawnerType.
        if (spawnType == SpawnerType.Radius)
        {
            // Calculate a random spawn position within the radius.
            Vector2 randomPoint = Random.insideUnitCircle * spawnRadius;
            spawnPosition = transform.position + new Vector3(randomPoint.x, randomPoint.y, 0);
        }
        else // spawnType == SpawnerType.SetSpawn
        {
            // Use a fixed offset for the spawn position.
            spawnPosition = transform.position + new Vector3(0, yOffset, 0);
        }

        // Instantiate the prefab at the calculated position.
        GameObject spawnedInstance = Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);

        // Add the new object to our tracking list.
        activeSpawns.Add(spawnedInstance);
    }
}
