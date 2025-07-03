using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour
{
    [Header("Chunk Settings")]
    [Tooltip("The list of terrain chunks to spawn when pollution is HIGH (>= 50%).")]
    public List<GameObject> pollutedTerrainChunks;
    [Tooltip("The list of terrain chunks to spawn when pollution is LOW (< 50%).")]
    public List<GameObject> cleanTerrainChunks;

    public GameObject player;
    public float checkerRadius = 1f;
    public LayerMask terrainMask;
    public GameObject currentChunk;

    [Header("Optimization")]
    public float maxOpDist = 50f;
    public float optimizerCooldownDur = 1f;
    public List<GameObject> spawnedChunks;

    private float optimizerCooldown;
    private PlayerMovement pm;

    void Start()
    {
        pm = FindObjectOfType<PlayerMovement>();

        if (pm == null)
            Debug.LogError("PlayerMovement not found in scene!");
        if (player == null)
            Debug.LogWarning("Player GameObject is not assigned.");
    }

    void Update()
    {
        if (currentChunk != null)
        {
            ChunkChecker();
        }

        ChunkOptimizer();
    }

    void ChunkChecker()
    {
        Vector2 dir = pm.moveDir.normalized;
        string directionName = GetDirectionName(dir);

        if (string.IsNullOrEmpty(directionName)) return;

        Transform targetPoint = currentChunk.transform.Find(directionName);

        if (targetPoint == null)
        {
            Debug.LogWarning($"'{directionName}' transform not found on {currentChunk.name}!");
            return;
        }

        bool terrainExists = Physics2D.OverlapCircle(targetPoint.position, checkerRadius, terrainMask);

        if (!terrainExists)
        {
            SpawnChunkAt(targetPoint.position);
        }
    }

    string GetDirectionName(Vector2 dir)
    {
        if (dir == Vector2.right) return "Right";
        if (dir == Vector2.left) return "Left";
        if (dir == Vector2.up) return "Up";
        if (dir == Vector2.down) return "Down";
        if (dir == (Vector2.right + Vector2.up).normalized) return "Right Up";
        if (dir == (Vector2.right + Vector2.down).normalized) return "Right Down";
        if (dir == (Vector2.left + Vector2.up).normalized) return "Left Up";
        if (dir == (Vector2.left + Vector2.down).normalized) return "Left Down";

        return null;
    }

    /// <summary>
    /// Spawns a chunk at the given position, choosing the chunk type based on pollution level.
    /// </summary>
    void SpawnChunkAt(Vector3 position)
    {
        // Determine which list of chunks to use based on the current pollution level.
        List<GameObject> chunksToUse;

        if (PollutionManager.instance.PollutionLevel < 50f)
        {
            chunksToUse = cleanTerrainChunks;
            Debug.Log("Spawning a CLEAN chunk.");
        }
        else
        {
            chunksToUse = pollutedTerrainChunks;
            Debug.Log("Spawning a POLLUTED chunk.");
        }

        // Check if the selected list has any prefabs assigned to it.
        if (chunksToUse == null || chunksToUse.Count == 0)
        {
            Debug.LogError("The appropriate terrain chunk list is empty! Assign prefabs in the Inspector.");
            return;
        }

        // Pick a random chunk from the selected list and spawn it.
        int rand = Random.Range(0, chunksToUse.Count);
        GameObject newChunk = Instantiate(chunksToUse[rand], position, Quaternion.identity);
        spawnedChunks.Add(newChunk);
    }

    void ChunkOptimizer()
    {
        optimizerCooldown -= Time.deltaTime;

        if (optimizerCooldown > 0f) return;

        optimizerCooldown = optimizerCooldownDur;

        // Use a for loop to safely iterate while potentially modifying the list (though we aren't here)
        for (int i = spawnedChunks.Count - 1; i >= 0; i--)
        {
            GameObject chunk = spawnedChunks[i];

            // If a chunk was destroyed by other means, remove it from the list.
            if (chunk == null)
            {
                spawnedChunks.RemoveAt(i);
                continue;
            }

            float dist = Vector3.Distance(player.transform.position, chunk.transform.position);
            chunk.SetActive(dist <= maxOpDist);
        }
    }
}
