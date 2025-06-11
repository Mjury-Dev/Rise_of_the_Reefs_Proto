using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    Transform player;
    EnemyStats enemy;
    Rigidbody2D rb;
    SpriteRenderer spriteRenderer;

    [Header("Sprite Settings")]
    [Tooltip("Set to true if sprite faces right by default, false if it faces left")]
    public bool spriteFacesRight = true; // Toggle in Inspector

    Vector2 knockbackVelocity;
    float knockbackDuration;

    void Start()
    {
        enemy = GetComponent<EnemyStats>();
        player = FindObjectOfType<PlayerMovement>().transform;
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void FixedUpdate()
    {
        if (knockbackDuration > 0)
        {
            rb.velocity = knockbackVelocity;
            knockbackDuration -= Time.fixedDeltaTime;
        }
        else
        {
            Vector2 direction = (player.position - transform.position).normalized;
            rb.velocity = direction * enemy.currentMoveSpeed;

            // Flip sprite based on movement direction
            if (rb.velocity.x > 0.1f) // Moving right
            {
                spriteRenderer.flipX = !spriteFacesRight;
            }
            else if (rb.velocity.x < -0.1f) // Moving left
            {
                spriteRenderer.flipX = spriteFacesRight;
            }
        }
    }

    public void Knockback(Vector2 velocity, float duration)
    {
        if (knockbackDuration > 0) return;

        knockbackVelocity = velocity;
        knockbackDuration = duration;
    }
}