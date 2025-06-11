using System.Collections.Generic;
using UnityEngine;

public class ShieldBehavior : ShieldWeaponBehavior
{
    private Vector3 playerPosition;
    List<GameObject> markedEnemies;

    private float hitSoundCooldown = 0.1f; // Cooldown for hit sound effect
    private float hitSoundTimer = 0f; // Timer to track cooldown

    protected override void Start()
    {
        base.Start();
        markedEnemies = new List<GameObject>();

        Vector3 playerPos = Camera.main.transform.position;

        float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
        float radius = 2.2f;
        Vector3 offset = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f) * radius;
        transform.position = playerPos + offset + new Vector3(0f, 0f, 5f);
    }

    protected void Update()
    {
        if (hitSoundTimer > 0f)
        {
            hitSoundTimer -= Time.deltaTime;
        }

        Vector3 playerPosition = Camera.main.transform.position;
        transform.RotateAround(
            playerPosition,
            Vector3.forward,
            weaponData.OrbitSpeed * Time.deltaTime);
        transform.Rotate(0f, 0f, weaponData.SpinSpeed * Time.deltaTime);
    }

    protected override void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Enemy") && !markedEnemies.Contains(col.gameObject))
        {
            // Play sound ONLY when hitting an enemy (and not on cooldown)
            if (hitSoundTimer <= 0f)
            {
                AudioManager.Instance.PlaySFX("ShieldHit", transform.position);
                hitSoundTimer = hitSoundCooldown;
            }

            EnemyStats enemy = col.GetComponent<EnemyStats>();
            enemy.Hurt(GetCurrentDamage(), transform.position);
            markedEnemies.Add(col.gameObject);
        }
        else if (col.CompareTag("Prop"))
        {
            if (col.gameObject.TryGetComponent(out BreakableProps breakable) && !markedEnemies.Contains(col.gameObject))
            {
                 if (hitSoundTimer <= 0f)
                {
                    AudioManager.Instance.PlaySFX("ShieldHit", transform.position);
                    hitSoundTimer = hitSoundCooldown;
                }

                breakable.TakeDamage(GetCurrentDamage());
                markedEnemies.Add(col.gameObject);
            }
        }
    }
}