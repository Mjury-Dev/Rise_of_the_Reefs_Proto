using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour
{
    [Header("Chunk Settings")]
    public List<GameObject> terrainChunks;
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

    void SpawnChunkAt(Vector3 position)
    {
        if (terrainChunks.Count == 0)
        {
            Debug.LogError("No terrain chunks in the list!");
            return;
        }

        int rand = Random.Range(0, terrainChunks.Count);
        GameObject newChunk = Instantiate(terrainChunks[rand], position, Quaternion.identity);
        spawnedChunks.Add(newChunk);
    }

    void ChunkOptimizer()
    {
        optimizerCooldown -= Time.deltaTime;

        if (optimizerCooldown > 0f) return;

        optimizerCooldown = optimizerCooldownDur;

        foreach (GameObject chunk in spawnedChunks)
        {
            if (chunk == null) continue;

            float dist = Vector3.Distance(player.transform.position, chunk.transform.position);
            chunk.SetActive(dist <= maxOpDist);
        }
    }
}
