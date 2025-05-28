using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.RestService;
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
            maxHealth = playerData.MaxHealth;
            healthSlider.maxValue = maxHealth;
            easeHealthSlider.maxValue = maxHealth;
        }
    }

    void Update()
    {
        if (player != null)
        {
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
}
