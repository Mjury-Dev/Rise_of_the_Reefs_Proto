using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public PlayerStats player;
    public CharacterScriptableObject playerData;
    public Slider healthSlider;
    public Slider easeHealthSlider;
    public float maxHealth;
    public float health;
    private float lerpSpeed = 0.05f;

    void Start()
    {
        if (player != null)
        {
            playerData = player.characterData;
            UpdateMaxHealth();
        }
    }

    void Update()
    {
        if (player != null)
        {
            // Always get the current max health directly from PlayerStats
            float currentMaxHealth = player.MaxHealth;

            // Update max values if they changed
            if (!Mathf.Approximately(maxHealth, currentMaxHealth))
            {
                maxHealth = currentMaxHealth;
                healthSlider.maxValue = maxHealth;
                easeHealthSlider.maxValue = maxHealth;
            }

            // Update current health display
            health = player.CurrentHealth;

            if (healthSlider.value != health)
            {
                healthSlider.value = health;
            }

            if (Mathf.Abs(easeHealthSlider.value - health) > 0.01f)
            {
                easeHealthSlider.value = Mathf.Lerp(easeHealthSlider.value, health, lerpSpeed);
            }
        }
    }
    void UpdateMaxHealth()
    {
        maxHealth = playerData.MaxHealth + PlayerUpgradeManager.Instance.GetUpgradeBonus(4);
        healthSlider.maxValue = maxHealth;
        easeHealthSlider.maxValue = maxHealth;

        // Ensure current values don't exceed new max
        if (healthSlider.value > maxHealth) healthSlider.value = maxHealth;
        if (easeHealthSlider.value > maxHealth) easeHealthSlider.value = maxHealth;
    }
}