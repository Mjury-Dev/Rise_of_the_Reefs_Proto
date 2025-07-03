using UnityEngine;
using System.Collections.Generic;

// This component requires a SpriteRenderer to function.
[RequireComponent(typeof(SpriteRenderer))]
public class PropSpriteRandomizer : MonoBehaviour
{
    // A list to hold all the possible sprites for this object.
    // You can drag and drop your sprites into this list in the Unity Inspector.
    public List<Sprite> spriteOptions;

    // Awake is called when the script instance is being loaded.
    // It's perfect for initialization tasks.
    void Awake()
    {
        // Check if the spriteOptions list has any sprites in it.
        if (spriteOptions != null && spriteOptions.Count > 0)
        {
            // Get the SpriteRenderer component attached to this GameObject.
            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();

            // Pick a random index from the list.
            int randomIndex = Random.Range(0, spriteOptions.Count);

            // Assign the randomly selected sprite to the SpriteRenderer.
            spriteRenderer.sprite = spriteOptions[randomIndex];
        }
        else
        {
            // Log a warning if no sprites are assigned to the list.
            Debug.LogWarning("No sprites assigned in the SpriteRandomizer list.", this);
        }
    }
}