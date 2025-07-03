using System.Collections;
using UnityEngine;

/// <summary>
/// Manages the behavior of a toxic goo puddle. It fades out over time
/// and deals continuous damage to any player inside its trigger.
/// </summary>
[RequireComponent(typeof(Collider2D), typeof(SpriteRenderer))]
public class ToxicGooBehavior : MonoBehaviour
{
    [Header("Goo Settings")]
    [Tooltip("How long, in seconds, the puddle will exist before completely fading away.")]
    public float lifetime = 5f;

    [Header("Damage Settings")]
    [Tooltip("The amount of damage to deal per tick.")]
    public int damagePerTick = 1;

    [Tooltip("How many times per second damage should be applied.")]
    public float ticksPerSecond = 2f;

    // Private components and timers
    private SpriteRenderer spriteRenderer;
    private float damageTimer;

    /// <summary>
    /// Called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        // Get the required components.
        spriteRenderer = GetComponent<SpriteRenderer>();
        GetComponent<Collider2D>().isTrigger = true; // Ensure the collider is a trigger.
    }

    /// <summary>
    /// Called on the frame when a script is enabled just before any of the Update methods are called the first time.
    /// </summary>
    void Start()
    {
        // Start the fade-out process as soon as the goo is instantiated.
        StartCoroutine(FadeOutRoutine());
    }

    /// <summary>
    /// A coroutine that gradually fades the goo's transparency over its lifetime.
    /// </summary>
    private IEnumerator FadeOutRoutine()
    {
        float elapsedTime = 0f;
        Color originalColor = spriteRenderer.color;

        while (elapsedTime < lifetime)
        {
            // Calculate the new alpha value based on the remaining lifetime.
            float newAlpha = Mathf.Lerp(originalColor.a, 0f, elapsedTime / lifetime);
            spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, newAlpha);

            // Wait for the next frame.
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // After the loop, ensure the alpha is fully zero and destroy the object.
        spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);
        Destroy(gameObject);
    }

    /// <summary>
    /// Called continuously for every frame another collider is within this trigger.
    /// </summary>
    /// <param name="other">The other collider involved in this collision.</param>
    void OnTriggerStay2D(Collider2D other)
    {
        // Try to get the PlayerStats component from the object inside the trigger.
        PlayerStats playerStats = other.GetComponent<PlayerStats>();

        // Check if the object has a PlayerStats component.
        if (playerStats != null)
        {
            // Increment the damage timer.
            damageTimer += Time.deltaTime;

            // If the timer has exceeded the interval for one damage tick...
            if (damageTimer >= 1f / ticksPerSecond)
            {
                // Reset the timer.
                damageTimer = 0f;

                // Call the player's TakeDamageFromSource function.
                // Pass this goo puddle's GameObject as the damage source.
                playerStats.TakeDamageFromSource(damagePerTick, this.gameObject);
            }
        }
    }

    /// <summary>
    /// Called when another collider leaves this trigger.
    /// </summary>
    /// <param name="other">The other collider involved in this collision.</param>
    void OnTriggerExit2D(Collider2D other)
    {
        // Check if the object that left has a PlayerStats component.
        if (other.GetComponent<PlayerStats>() != null)
        {
            // Reset the damage timer when the player leaves the puddle.
            damageTimer = 0f;
        }
    }
}
