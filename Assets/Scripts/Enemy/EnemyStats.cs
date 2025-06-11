using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(SpriteRenderer))]
public class EnemyStats : MonoBehaviour
{
    public EnemyScriptableObject enemyData;

    [HideInInspector]
    public float currentMoveSpeed;
    [HideInInspector]
    public float currentHealth;
    [HideInInspector]
    public float currentDamage;

    public float despawnDistance = 20f;
    Transform player;

    [Header("Damage Feedback")]
    public Color damageColor = new Color(1f, 1f, 1f, 1f);
    public float damageFlashDuration = 0.2f;
    public float deathFadeTime = 0.6f;
    public Material flashMaterial;
    private Material originalMaterial;
    private SpriteRenderer sr;
    Color originalcolor;
    EnemyMovement movement;
    public GameObject slashEffectPrefab;

    private void Start()
    {
        player = FindObjectOfType<PlayerStats>().transform;
        sr = GetComponent<SpriteRenderer>();
        originalMaterial = sr.material;
        originalcolor = sr.color;

        movement = GetComponent<EnemyMovement>();
    }

    private void Update()
    {
        if (Vector2.Distance(transform.position, player.position) >= despawnDistance)
        {
            ReturnEnemy();
        }
    }
    void Awake()
    {
        currentMoveSpeed = enemyData.MoveSpeed;
        currentHealth = enemyData.MaxHealth;
        currentDamage = enemyData.Damage;
    }

    public void Hurt(float dmg, Vector2 sourcePosition, float knockbackForce = 2f, float knockbackDuration = 0.1f)
    {
        AudioManager.Instance.PlaySFX("EnemyHit", transform.position);
        currentHealth -= dmg;
        SpawnSlashEffect();
        StartCoroutine(DamageFlash());
        if (dmg > 0)
        {
            GameManager.GenerateFloatingText(Mathf.FloorToInt(dmg).ToString(), transform);
        }
        if (knockbackForce > 0)
        {
            Vector2 dir = (Vector2)transform.position - sourcePosition;
            movement.Knockback(dir.normalized * knockbackForce, knockbackDuration);
        }
        if(currentHealth <= 0)
        {
            StartCoroutine(KillFade());
        }
    }
    void SpawnSlashEffect()
    {
        if (slashEffectPrefab != null)
        {
            GameObject effect = Instantiate(slashEffectPrefab, transform.position, Quaternion.identity);
            effect.transform.SetParent(transform); // Optional
        }
    }

    IEnumerator DamageFlash()
    {
        int flashes = 5;               // hardcoded number of flashes
        float flashDuration = 0.05f;    // hardcoded duration for each flash on/off

        Vector3 originalScale = transform.localScale;

        for (int i = 0; i < flashes; i++)
        {
            // Flash ON: white material + scale up
            sr.material = flashMaterial;
            sr.color = Color.white;
            transform.localScale = originalScale * 1.10f;

            yield return new WaitForSeconds(flashDuration);

            // Flash OFF: original material + scale reset
            sr.material = originalMaterial;
            sr.color = originalcolor;
            transform.localScale = originalScale;

            yield return new WaitForSeconds(flashDuration);
        }

        // Make sure scale is reset at the end (in case of early exit)
        transform.localScale = originalScale;
    }



    public void Kill()
    {
        Destroy(gameObject);
        GameManager.instance.EnemyKilled();
        GameManager.instance.UpdateKillCounter();
    }
    IEnumerator KillFade()
    {
        AudioManager.Instance.PlaySFX("EnemyDeath", transform.position);
        WaitForEndOfFrame w = new WaitForEndOfFrame();
        float t = 0, origAlpha = sr.color.a;

        while (t < deathFadeTime)
        {
            yield return w;
            t += Time.deltaTime;

            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, (1 - t / deathFadeTime) * origAlpha);
        }

        Kill();
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            PlayerStats player = col.GetComponent<PlayerStats>();
            if (player != null)
            {
                // Now passes the enemy gameObject as the damage source
                player.TakeDamageFromSource(currentDamage, gameObject);
            }
        }
    }


    private void OnDestroy()
    {
        EnemySpawner es = FindObjectOfType<EnemySpawner>();
        if (es != null)
        {
            es.OnEnemyKilled();
        }
        else
        {
            Debug.LogWarning("EnemySpawner not found!");
        }
    }

    void ReturnEnemy()
    {
        EnemySpawner es = FindObjectOfType<EnemySpawner>();
        transform.position = player.position + es.relativeSpawnPoints[Random.Range(0, es.relativeSpawnPoints.Count)].position;
    }
}
