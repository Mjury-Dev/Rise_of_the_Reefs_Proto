using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class BreakableProps : MonoBehaviour
{
    public float health;

    [Header("Damage Feedback")]
    public Color damageColor = new Color(1f, 1f, 1f, 1f);
    public float damageFlashDuration = 0.2f;
    public float deathFadeTime = 0.6f;
    public Material flashMaterial;
    private Material originalMaterial;
    private SpriteRenderer sr;
    Color originalColor;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        originalMaterial = sr.material;
        originalColor = sr.color;
    }

    public void TakeDamage(float dmg)
    {
        health -= dmg;
        StartCoroutine(DamageFlash());

        if (health <= 0)
        {
            StartCoroutine(KillFade());
        }
    }

    IEnumerator DamageFlash()
    {
        int flashes = 5;               // Number of flashes
        float flashDuration = 0.05f;   // Duration for each flash on/off

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
            sr.color = originalColor;
            transform.localScale = originalScale;

            yield return new WaitForSeconds(flashDuration);
        }

        // Make sure scale is reset at the end
        transform.localScale = originalScale;
    }

    IEnumerator KillFade()
    {
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

    public void Kill()
    {
        Destroy(gameObject);
    }
}