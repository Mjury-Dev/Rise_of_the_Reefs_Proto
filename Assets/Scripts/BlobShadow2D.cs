using UnityEngine;

/// <summary>
/// Creates a simple 2D blob shadow that follows its parent object.
/// This script creates a child GameObject with a SpriteRenderer to act as the shadow.
/// It is positioned at a fixed offset from the parent.
/// </summary>
public class BlobShadow2D : MonoBehaviour
{
    [Header("Shadow Settings")]
    [Tooltip("The sprite to use for the shadow. A simple, soft oval works best.")]
    public Sprite shadowSprite;

    [Tooltip("The color tint and transparency of the shadow.")]
    public Color shadowColor = new Color(0f, 0f, 0f, 0.5f); // Default: 50% transparent black

    [Header("Positioning & Scale")]
    [Tooltip("The local vertical offset of the shadow from the object's pivot.")]
    public float yOffset = -0.5f;

    [Tooltip("The local Z-position of the shadow to help with sorting.")]
    public float zOffset = 0.1f;

    [Tooltip("The uniform scale of the shadow sprite.")]
    public float shadowSize = 1f;

    [Header("Sorting")]
    [Tooltip("The name of the Sorting Layer to put the shadow on.")]
    public string sortingLayerName = "Default";

    [Tooltip("The order within the sorting layer. Lower numbers are rendered first (further back).")]
    public int orderInLayer = -1;

    // Private reference to the shadow's components
    private GameObject shadowObject;
    private SpriteRenderer shadowSpriteRenderer;

    /// <summary>
    /// Called when the script instance is being loaded.
    /// </summary>
    void Start()
    {
        // --- Create the Shadow GameObject ---
        shadowObject = new GameObject("BlobShadow");
        shadowObject.transform.SetParent(this.transform); // Parent to this object

        // --- Position and Scale the Shadow ---
        shadowObject.transform.localPosition = new Vector3(0, yOffset, zOffset);
        shadowObject.transform.localRotation = Quaternion.identity;
        shadowObject.transform.localScale = new Vector3(shadowSize, shadowSize, shadowSize);

        // --- Add and configure the SpriteRenderer ---
        shadowSpriteRenderer = shadowObject.AddComponent<SpriteRenderer>();
        shadowSpriteRenderer.sprite = shadowSprite;
        shadowSpriteRenderer.color = shadowColor;

        // --- Set Sorting Order ---
        // Assign the sorting layer and order directly from the public variables.
        shadowSpriteRenderer.sortingLayerName = sortingLayerName;
        shadowSpriteRenderer.sortingOrder = orderInLayer;
    }

    /// <summary>
    /// This is called in the editor when a value is changed.
    /// It allows you to see inspector changes in real-time without running the game.
    /// </summary>
    void OnValidate()
    {
        // Ensure the script can update values in the editor for quick iteration.
        if (shadowObject != null)
        {
            shadowObject.transform.localPosition = new Vector3(0, yOffset, zOffset);
            shadowObject.transform.localScale = new Vector3(shadowSize, shadowSize, shadowSize);

            if (shadowSpriteRenderer != null)
            {
                shadowSpriteRenderer.color = shadowColor;
                shadowSpriteRenderer.sortingLayerName = sortingLayerName;
                shadowSpriteRenderer.sortingOrder = orderInLayer;
            }
        }
    }
}
