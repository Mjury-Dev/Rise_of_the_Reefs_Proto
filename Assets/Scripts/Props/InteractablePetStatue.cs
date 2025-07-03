using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // Make sure to include this if you're using TextMeshPro

/// <summary>
/// Manages an interactable prop that summons a companion when the player is near
/// and presses the 'E' key. Checks against the player's max pet limit.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class InteractablePetStatue : MonoBehaviour
{
    [Header("Interaction Settings")]
    [Tooltip("The key the player must press to interact.")]
    public KeyCode interactionKey = KeyCode.E;

    [Tooltip("The prefab of the companion to be spawned.")]
    public GameObject companionPrefab;

    [Tooltip("The position where the companion will spawn, relative to the statue.")]
    public Vector3 spawnOffset = new Vector3(1.5f, 0, 0);

    [Header("UI Prompt")]
    [Tooltip("The parent GameObject that holds all UI elements for the prompt (e.g., the 'E' key sprite and text).")]
    public GameObject promptUI;

    [Header("Animation Settings")]
    [Tooltip("How long the fade-in and fade-out animations take.")]
    public float fadeDuration = 0.3f;
    [Tooltip("How high the prompt bobs up and down.")]
    public float bobHeight = 0.1f;
    [Tooltip("How fast the prompt bobs.")]
    public float bobSpeed = 2f;


    // Internal state tracking
    private bool playerIsInRange = false;
    private bool hasBeenUsed = false;
    private PlayerStats playerStats; // A reference to the player's stats component

    // --- Animation variables ---
    private List<SpriteRenderer> promptSpriteRenderers = new List<SpriteRenderer>();
    private List<TextMeshPro> promptTMPs = new List<TextMeshPro>();
    private Vector3 promptOriginalPosition;
    private Coroutine animationCoroutine;
    private bool isPromptVisible = false;

    void Awake()
    {
        // Ensure the collider is a trigger to detect player proximity without physical collision.
        GetComponent<Collider2D>().isTrigger = true;

        if (promptUI != null)
        {
            // Find all renderers to fade them later.
            promptUI.GetComponentsInChildren<SpriteRenderer>(promptSpriteRenderers);
            promptUI.GetComponentsInChildren<TextMeshPro>(promptTMPs);

            // Store the starting local position for the bobbing animation.
            promptOriginalPosition = promptUI.transform.localPosition;

            // Start with the prompt invisible.
            SetPromptAlpha(0);
            promptUI.SetActive(false);
        }
    }

    void Update()
    {
        if (playerIsInRange && !hasBeenUsed)
        {
            // Check if the player has room for another pet.
            bool canSummon = (playerStats != null && playerStats.CurrentPets < playerStats.MaxPets);

            // If the prompt's visibility state doesn't match if we can summon, update it.
            if (isPromptVisible != canSummon)
            {
                if (animationCoroutine != null) StopCoroutine(animationCoroutine);
                animationCoroutine = StartCoroutine(AnimatePrompt(canSummon));
                isPromptVisible = canSummon;
            }

            // Only check for input if the player can summon.
            if (canSummon && Input.GetKeyDown(interactionKey))
            {
                SummonCompanion();
            }
        }
    }

    /// <summary>
    /// Handles the logic for summoning the companion and deactivating the prop.
    /// </summary>
    private void SummonCompanion()
    {
        AudioManager.Instance.PlaySFX("SummonCompanion", transform.position); // Play a sound effect for summoning.
        Debug.Log("Companion summoned!", this);

        // Mark as used to prevent further interactions.
        hasBeenUsed = true;

        // Stop the animation and hide the prompt.
        if (animationCoroutine != null)
        {
            StopCoroutine(animationCoroutine);
        }
        promptUI.SetActive(false);


        // --- Summoning Logic ---
        if (companionPrefab != null && playerStats != null)
        {
            // Increment the player's pet count.
            playerStats.CurrentPets++;
            GameManager.instance.UpdatePetCounter(playerStats.currentPets, playerStats.MaxPets); // Update the UI to reflect the new pet count.

            // Calculate the spawn position relative to this statue's position using the offset.
            Vector3 spawnPosition = transform.position + spawnOffset;
            Instantiate(companionPrefab, spawnPosition, Quaternion.identity);
        }
    }

    /// <summary>
    /// A coroutine that handles fading and bobbing the prompt UI.
    /// </summary>
    /// <param name="fadeIn">True to fade in, false to fade out.</param>
    private IEnumerator AnimatePrompt(bool fadeIn)
    {
        // --- Fading Phase ---
        float timer = 0f;
        float startAlpha = fadeIn ? 0f : 1f;
        float endAlpha = fadeIn ? 1f : 0f;

        // Make sure the UI is active to be visible during fade-in.
        if (fadeIn)
        {
            promptUI.SetActive(true);
        }

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float progress = timer / fadeDuration;
            SetPromptAlpha(Mathf.Lerp(startAlpha, endAlpha, progress));
            yield return null;
        }
        SetPromptAlpha(endAlpha);

        // --- Bobbing Phase (only if fading in) ---
        if (fadeIn)
        {
            while (true) // This will loop until the coroutine is stopped.
            {
                float newY = promptOriginalPosition.y + Mathf.Sin(Time.time * bobSpeed) * bobHeight;
                promptUI.transform.localPosition = new Vector3(promptOriginalPosition.x, newY, promptOriginalPosition.z);
                yield return null;
            }
        }
        else
        {
            // If fading out, deactivate the object when done.
            promptUI.SetActive(false);
        }
    }

    /// <summary>
    /// Helper method to set the alpha (transparency) on all UI elements.
    /// </summary>
    private void SetPromptAlpha(float alpha)
    {
        foreach (var renderer in promptSpriteRenderers)
        {
            Color color = renderer.color;
            color.a = alpha;
            renderer.color = color;
        }
        foreach (var tmp in promptTMPs)
        {
            Color color = tmp.color;
            color.a = alpha;
            tmp.color = color;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!hasBeenUsed && other.CompareTag("Player"))
        {
            playerIsInRange = true;
            // Get the PlayerStats component to check pet limits.
            playerStats = other.GetComponent<PlayerStats>();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsInRange = false;
            // Clear the reference when the player leaves.
            playerStats = null;

            // Hide the prompt if it's visible.
            if (isPromptVisible)
            {
                if (animationCoroutine != null) StopCoroutine(animationCoroutine);
                animationCoroutine = StartCoroutine(AnimatePrompt(false)); // Fade OUT
                isPromptVisible = false;
            }
        }
    }
}
