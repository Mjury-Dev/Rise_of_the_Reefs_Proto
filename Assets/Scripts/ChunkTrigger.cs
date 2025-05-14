using UnityEngine;

public class ChunkTrigger : MonoBehaviour
{
    private MapController mc;

    [Tooltip("Target chunk this trigger belongs to.")]
    public GameObject targetMap;

    void Start()
    {
        mc = FindObjectOfType<MapController>();

        if (mc == null)
        {
            Debug.LogError("MapController not found in the scene!");
        }

        if (targetMap == null)
        {
            Debug.LogWarning($"TargetMap not assigned on {gameObject.name}!");
        }
    }

    private void OnTriggerStay2D(Collider2D col)
    {
        if (col.CompareTag("Player") && mc != null)
        {
            mc.currentChunk = targetMap;
        }
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.CompareTag("Player") && mc != null && mc.currentChunk == targetMap)
        {
            mc.currentChunk = null;
        }
    }
}
