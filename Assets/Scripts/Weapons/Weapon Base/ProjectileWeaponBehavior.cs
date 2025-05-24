using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileWeaponBehavior : MonoBehaviour
{
    public WeaponScriptableObject weaponData;

    protected float currentDamage;
    protected float currentSpeed;
    protected float currentCooldownDuration;
    protected int currentPierce;
    protected int currentBounce;

    private List<Transform> hitEnemies = new List<Transform>();

    private void Awake()
    {
        currentDamage = weaponData.Damage;
        currentSpeed = weaponData.Speed;
        currentCooldownDuration = weaponData.CooldownDuration;
        currentPierce = weaponData.Pierce;
        currentBounce = weaponData.Bounce;
    }

    public float GetCurrentDamage()
    {
        return currentDamage *= FindObjectOfType<PlayerStats>().CurrentStrength;
    }

    protected Vector3 direction;
    public float destroyAfterSeconds;
    
    // Start is called before the first frame update
    protected virtual void Start()
    {
        Destroy(gameObject, destroyAfterSeconds);

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = direction * currentSpeed;
        }
    }

    public void DirectionChecker(Vector3 dir)
    {
        direction = dir;

        float dirx = direction.x;
        float diry = direction.y;
        float dirz = direction.z;

        Vector3 baseScale = new Vector3(1, 1, 1);
        Vector3 rotation = Vector3.zero;

        // moving left
        if (dirx < 0 && diry == 0)
        {
            baseScale.x = -1;
            rotation.z = 45f;
        }

        // moving right
        if (dirx > 0 && diry == 0)
        {
            rotation.z = -45f;
        }

        // moving up
        if (diry > 0 && dirx == 0)
        {
            rotation.z = 45f;
        }

        // moving down
        if (diry < 0 && dirx == 0)
        {
            rotation.z = 225f;
        }

        // up-left
        if (dirx < 0 && diry > 0)
        {
            baseScale.x = -1;
            rotation.z = 0f;
        }

        // up-right
        if (dirx > 0 && diry > 0)
        {
            rotation.z = 0f;
        }

        // down-right
        if (dirx > 0 && diry < 0)
        {
            rotation.z = -90f;
        }

        // down-left
        if (dirx < 0 && diry < 0)
        {
            baseScale.x = -1;
            rotation.z = 90f;
        }

        // Apply final values
        transform.localScale = baseScale;
        transform.rotation = Quaternion.Euler(rotation);

    }

    protected void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Enemy"))
        {
            Transform hitTransform = col.transform;

            // Prevent hitting the same enemy multiple times
            if (hitEnemies.Contains(hitTransform)) return;

            hitEnemies.Add(hitTransform);

            EnemyStats enemy = col.GetComponent<EnemyStats>();
            enemy.Hurt(GetCurrentDamage());

            if (currentBounce > 0)
            {
                GameObject nextTarget = FindNearestEnemy(hitTransform);

                if (nextTarget != null && !hitEnemies.Contains(nextTarget.transform))
                {
                    Vector3 newDirection = (nextTarget.transform.position - transform.position).normalized;
                    direction = newDirection;
                    DirectionChecker(direction); // Update orientation

                    Rigidbody2D rb = GetComponent<Rigidbody2D>();
                    if (rb != null)
                    {
                        rb.velocity = direction * currentSpeed;
                    }
                    ReducePierce();
                    ReduceBounce();
                    return;
                }
            }
        }
        else if (col.CompareTag("Prop"))
        {
            if(col.gameObject.TryGetComponent(out BreakableProps breakable))
            {
                breakable.TakeDamage(GetCurrentDamage());
                ReducePierce();
            }
        }
    }


    void ReducePierce()
    {
        currentPierce--;
        if (currentPierce <= 0)
        {
            Destroy(gameObject);
        }
    }

    void ReduceBounce()
    {
        currentBounce--;
    }

    GameObject FindNearestEnemy(Transform exclude)
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject nearest = null;
        float minDistance = Mathf.Infinity;

        foreach (GameObject enemy in enemies)
        {
            if (hitEnemies.Contains(enemy.transform)) continue; // Skip already hit enemies

            float dist = Vector3.Distance(transform.position, enemy.transform.position);
            if (dist < minDistance)
            {
                minDistance = dist;
                nearest = enemy;
            }
        }

        return nearest;
    }
}
