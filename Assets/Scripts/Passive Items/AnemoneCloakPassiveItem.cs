using UnityEngine;

public class AnemoneCloakPassiveItem : PassiveItem
{
    private PlayerStats playerStats;
    private float baseDamageReflectPercent; // Base percentage of damage to reflect (e.g., 0.3 for 30%)

    protected override void ApplyModifier()
    {
        // Get references
        playerStats = GetComponentInParent<PlayerStats>();
        baseDamageReflectPercent = passiveItemData.Multiplier / 100f;

        if (playerStats != null)
        {
            // Subscribe to the player's OnDamageTaken event
            playerStats.OnDamageTaken += ReflectDamageToAttacker;
        }
    }

    public void ReflectDamageToAttacker(float damage, GameObject damageSource)
    {
        // Only reflect damage if it came from an enemy
        if (damageSource != null && damageSource.CompareTag("Enemy"))
        {
            EnemyStats enemyStats = damageSource.GetComponent<EnemyStats>();
            if (enemyStats != null)
            {
                // Calculate reflected damage with strength scaling
                // Strength is already a multiplier (e.g., 0.5 = +50% damage)
                float strengthMultiplier = 1f + playerStats.CurrentStrength;
                float scaledReflectPercent = baseDamageReflectPercent * strengthMultiplier;
                float reflectedDamage = damage * scaledReflectPercent;

                // Damage the enemy (using Vector2.zero if no knockback needed)
                enemyStats.Hurt(reflectedDamage, Vector2.zero, 0f, 0f);

                // Optional: Visual feedback
                Debug.Log($"Reflected {reflectedDamage} damage (base: {damage * baseDamageReflectPercent}) " +
                         $"to {damageSource.name} with strength multiplier {strengthMultiplier}x");
            }
        }
    }

    private void OnDestroy()
    {
        // Unsubscribe to prevent memory leaks
        if (playerStats != null)
        {
            playerStats.OnDamageTaken -= ReflectDamageToAttacker;
        }
    }
}