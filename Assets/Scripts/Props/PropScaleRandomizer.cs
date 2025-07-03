using UnityEngine;

/// <summary>
/// Randomizes the scale of this GameObject upon instantiation.
/// The scale will be a uniform value between the specified min and max.
/// </summary>
public class PropScaleRandomizer : MonoBehaviour
{
    [Header("Scale Settings")]
    [Tooltip("The minimum possible scale for the object.")]
    public float minScale = 0.8f;

    [Tooltip("The maximum possible scale for the object.")]
    public float maxScale = 1.2f;

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// It's ideal for setting up the initial state of an object.
    /// </summary>
    void Awake()
    {
        // Generate a single random scale value within the defined range.
        float randomScaleValue = Random.Range(minScale, maxScale);

        // Create a new Vector3 with the same random value for x, y, and z for uniform scaling.
        Vector3 newScale = new Vector3(randomScaleValue, randomScaleValue, randomScaleValue);

        // Apply the new random scale to the GameObject's transform.
        transform.localScale = newScale;
    }
}
