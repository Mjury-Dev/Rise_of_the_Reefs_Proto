using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Manages the combat AI for a pet. The pet roams near the player,
/// seeks out the nearest enemy, hops to attack it on contact, then returns and recharges.
/// </summary>
[RequireComponent(typeof(Rigidbody2D), typeof(CircleCollider2D))]
public class PetHoppingMovement : MonoBehaviour
{
    // Enum to manage the pet's complex states.
    private enum PetState
    {
        Roaming,        // Sliding around the player while recharging.
        SeekingTarget,  // Looking for a new enemy.
        MovingToTarget, // Hopping towards a selected enemy.
        Returning,      // Hopping back to the player.
        Recharging      // Waiting for the attack cooldown.
    }

    [Header("Player Following")]
    [Tooltip("The player's transform that the pet will follow.")]
    public Transform playerTransform;
    [Tooltip("The radius around the player where the pet will roam and return to.")]
    public float followRadius = 3f;

    [Header("Combat Settings")]
    [Tooltip("The tag used to identify enemies.")]
    public string enemyTag = "Enemy";
    [Tooltip("How far the pet can 'see' to find a target.")]
    public float detectionRadius = 15f;
    [Tooltip("The amount of damage the pet deals on contact.")]
    public float petAttackDamage = 10f;
    [Tooltip("The cooldown time in seconds between attack runs.")]
    public float rechargeTime = 3f;

    [Header("Movement Style")]
    [Tooltip("How fast the pet moves from one point to another.")]
    public float moveSpeed = 6f;
    [Tooltip("How high the pet 'jumps' during a hop. This is purely visual.")]
    public float hopHeight = 1f;
    [Tooltip("How long it takes to accelerate/decelerate. Smaller values are faster.")]
    public float movementSmoothing = 0.3f;

    // Private state and component references
    private PetState currentState = PetState.Recharging;
    private Rigidbody2D rb;
    private Transform spriteTransform;
    private Transform currentTarget;
    private float rechargeTimer;
    private Coroutine movementCoroutine; // A reference to the current movement coroutine
    private List<Collider2D> alreadyHitEnemies; // Tracks enemies hit in the current attack run
    private Vector3 velocity = Vector3.zero; // Used by SmoothDamp for acceleration
    private Animator animator; // Reference to the Animator component

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.isKinematic = true;

        // Ensure the collider is a trigger.
        GetComponent<CircleCollider2D>().isTrigger = true;
        alreadyHitEnemies = new List<Collider2D>();

        // Get the Animator component from the child sprite object.
        animator = GetComponentInChildren<Animator>();

        if (playerTransform == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) playerTransform = player.transform;
            else { Debug.LogError("Pet AI: Player not found!", this); this.enabled = false; return; }
        }

        spriteTransform = transform.GetChild(0);
        if (spriteTransform == null)
        {
            Debug.LogError("Pet AI: Requires a child object for the sprite.", this);
            this.enabled = false;
        }

        // Start the main AI loop.
        StartCoroutine(PetAI_Routine());
    }

    /// <summary>
    /// The main AI loop that controls the pet's state transitions.
    /// </summary>
    private IEnumerator PetAI_Routine()
    {
        while (true)
        {
            switch (currentState)
            {
                case PetState.Recharging:
                    rechargeTimer -= Time.deltaTime;
                    if (rechargeTimer <= 0)
                    {
                        currentState = PetState.SeekingTarget;
                    }
                    else if (movementCoroutine == null) // Only roam if not already moving
                    {
                        movementCoroutine = StartCoroutine(MoveToPosition(GetRoamingPosition(), false));
                    }
                    break;

                case PetState.SeekingTarget:
                    alreadyHitEnemies.Clear(); // Clear the list for the new attack run
                    FindNearestEnemy();
                    if (currentTarget != null)
                    {
                        currentState = PetState.MovingToTarget;
                    }
                    else
                    {
                        rechargeTimer = 1f; // Short delay before trying again
                        currentState = PetState.Recharging;
                    }
                    break;

                case PetState.MovingToTarget:
                    if (currentTarget == null) { currentState = PetState.SeekingTarget; break; }
                    if (movementCoroutine == null)
                    {
                        movementCoroutine = StartCoroutine(MoveToPosition(currentTarget.position, true));
                    }
                    break;

                case PetState.Returning:
                    if (movementCoroutine == null)
                    {
                        movementCoroutine = StartCoroutine(MoveToPosition(GetRoamingPosition(), true));
                    }
                    break;
            }
            yield return null;
        }
    }

    /// <summary>
    /// Called by Unity's physics engine when this object's trigger collider enters another.
    /// </summary>
    void OnTriggerEnter2D(Collider2D other)
    {
        // Only deal damage if we are in an attack run and haven't hit this enemy yet.
        if (currentState == PetState.MovingToTarget && !alreadyHitEnemies.Contains(other))
        {
            // Check if the object we hit is an enemy.
            EnemyStats enemyStats = other.GetComponent<EnemyStats>();
            if (enemyStats != null)
            {
                Debug.Log($"Pet hit {other.name}!", this);
                if (animator != null)
                {
                    animator.SetTrigger("Attack");
                }

                // Deal damage
                enemyStats.Hurt(petAttackDamage, transform.position);
                AudioManager.Instance.PlaySFX("SharkBite", transform.position); // Play attack sound effect
                alreadyHitEnemies.Add(other); // Add to the list to prevent multi-hits

                // --- Immediately return to the player ---
                if (movementCoroutine != null)
                {
                    StopCoroutine(movementCoroutine);
                    movementCoroutine = null;
                }
                currentState = PetState.Returning;
            }
        }
    }

    private void FindNearestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);
        Transform nearestEnemy = null;
        float minDistance = Mathf.Infinity;

        foreach (GameObject enemy in enemies)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance < minDistance && distance <= detectionRadius)
            {
                minDistance = distance;
                nearestEnemy = enemy.transform;
            }
        }
        currentTarget = nearestEnemy;
    }

    private IEnumerator MoveToPosition(Vector3 destination, bool isHopping)
    {
        UpdateSpriteDirection(destination);

        // Store the starting position and total distance for the hop arc calculation.
        Vector3 startPos = transform.position;
        float totalDistance = Vector3.Distance(startPos, destination);

        while (Vector3.Distance(transform.position, destination) > 0.1f)
        {
            // Use SmoothDamp for acceleration/deceleration.
            // The 'velocity' variable is passed by reference and updated automatically.
            transform.position = Vector3.SmoothDamp(transform.position, destination, ref velocity, movementSmoothing, moveSpeed);

            if (isHopping && totalDistance > 0)
            {
                // Calculate current progress from 0 to 1.
                float journeyProgress = 1f - (Vector3.Distance(transform.position, destination) / totalDistance);

                // Calculate the arc using a sine wave for a smooth bounce.
                float arc = hopHeight * Mathf.Sin(journeyProgress * Mathf.PI);
                spriteTransform.localPosition = new Vector3(0, arc, 0);
            }
            yield return null;
        }

        // Finalize position and reset state.
        transform.position = destination;
        spriteTransform.localPosition = Vector3.zero;
        velocity = Vector3.zero; // Reset velocity after movement is complete.
        movementCoroutine = null; // Mark movement as complete

        // If the return trip is finished, start recharging.
        if (currentState == PetState.Returning)
        {
            rechargeTimer = rechargeTime;
            currentState = PetState.Recharging;
        }
    }

    private void UpdateSpriteDirection(Vector3 destination)
    {
        // Check if there's a meaningful horizontal direction change to avoid unnecessary flips.
        if (Mathf.Abs(destination.x - transform.position.x) > 0.1f)
        {
            if (destination.x > transform.position.x)
            {
                // Moving right, so scale is normal.
                spriteTransform.localScale = new Vector3(1f, 1f, 1f);
            }
            else
            {
                // Moving left, so flip the sprite on the X axis.
                spriteTransform.localScale = new Vector3(-1f, 1f, 1f);
            }
        }
    }

    private Vector3 GetRoamingPosition()
    {
        return playerTransform.position + (Vector3)(Random.insideUnitCircle.normalized * followRadius);
    }
}
