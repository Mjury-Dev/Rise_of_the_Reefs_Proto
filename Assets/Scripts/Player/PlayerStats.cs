using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEditor.U2D.Animation;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public CharacterScriptableObject characterData;

    public float currentHealth;
    public float currentRecovery;
    public float currentMoveSpeed;
    public float currentStrength;
    public float currentProjectileSpeed;
    public float currentMagnet;

    #region Current Stats Properties
    public float CurrentHealth
    {
        get { return currentHealth; }
        set
        {
            if (currentHealth != value)
            {
                currentHealth = value;
                if (GameManager.instance != null)
                {
                    GameManager.instance.currentHealthDisplay.text = "Health: " + currentHealth;
                }
            }
        }
    }

    public float CurrentRecovery
    {
        get { return currentRecovery; }
        set
        {
            if (currentRecovery != value)
            {
                currentRecovery = value;
                if (GameManager.instance != null)
                {
                    GameManager.instance.currentRecoveryDisplay.text = "Recovery: " + currentRecovery + " hp/sec";
                }
            }
        }
    }

    public float CurrentMoveSpeed
    {
        get { return currentMoveSpeed; }
        set
        {
            if (currentMoveSpeed != value)
            {
                currentMoveSpeed = value;
                if (GameManager.instance != null)
                {
                    GameManager.instance.currentMoveSpeedDisplay.text = "Mov Spd: +" + currentMoveSpeed * 10 + "%";
                }
            }
        }
    }

    public float CurrentStrength
    {
        get { return currentStrength; }
        set
        {
            if (currentStrength != value)
            {
                currentStrength = value;
                if (GameManager.instance != null)
                {
                    GameManager.instance.currentStrengthDisplay.text = "Strength: +" + currentStrength * 100 + "%";
                }
            }
        }
    }

    public float CurrentProjectileSpeed
    {
        get { return currentProjectileSpeed; }
        set
        {
            if (currentProjectileSpeed != value)
            {
                currentProjectileSpeed = value;
                if (GameManager.instance != null)
                {
                    GameManager.instance.currentProjectileSpeedDisplay.text = "Proj Spd: +" + CurrentProjectileSpeed + "%";
                }
            }
        }
    }

    public float CurrentMagnet
    {
        get { return currentMagnet; }
        set
        {
            if (currentMagnet != value)
            {
                currentMagnet = value;
                if (GameManager.instance != null)
                {
                    GameManager.instance.currentMagnetDisplay.text = "Magnet: +" + CurrentMagnet * 10 + "%";
                }
            }
        }
    }
    #endregion

    [Header("Experience/Level")]
    public int experience = 0;
    public int level = 1;
    public int experienceCap;

    public GameObject firstWeaponTest;
    public GameObject secondWeaponTest;
    public GameObject firstPassiveItemTest;
    public GameObject secondPassiveItemTest;

    private void Awake()
    {

        inventory  = GetComponent<InventoryManager>();

        CurrentHealth = characterData.MaxHealth;
        CurrentRecovery = characterData.Recovery;
        CurrentMoveSpeed = characterData.MoveSpeed;
        CurrentStrength = characterData.Strength;
        CurrentProjectileSpeed = characterData.ProjectileSpeed;
        CurrentMagnet = characterData.Magnet;

        SpawnWeapon(firstWeaponTest);
        SpawnWeapon(secondWeaponTest);
        
        SpawnPassiveItem(firstPassiveItemTest);
        SpawnPassiveItem(secondPassiveItemTest);
    }

    [System.Serializable]
    public class LevelRange
    {
        public int startLevel;
        public int endLevel;
        public int experienceCapIncrease;
    }

    [Header("I-Frames")]
    public float invincibilityDuration;
    float invincibilityTimer;
    bool isInvincible;

    public List<LevelRange> levelRanges;

    InventoryManager inventory;
    public int weaponIndex;
    public int passiveItemIndex;

    private void Start()
    {
        experienceCap = levelRanges[0].experienceCapIncrease;

        GameManager.instance.currentHealthDisplay.text = "Health: +" + CurrentHealth + "%";
        GameManager.instance.currentRecoveryDisplay.text = "Recovery: " + CurrentRecovery + " hp/sec";
        GameManager.instance.currentMoveSpeedDisplay.text = "Mov Spd: +" + CurrentMoveSpeed + "%";
        GameManager.instance.currentStrengthDisplay.text = "Strength: +" +  CurrentStrength + "%";
        GameManager.instance.currentProjectileSpeedDisplay.text = "Proj Spd: +" + CurrentProjectileSpeed + "%";
        GameManager.instance.currentMagnetDisplay.text = "Magnet: +" + CurrentMagnet * 10 + "%";

    }

    private void Update()
    {
        if (invincibilityTimer > 0)
        {
            invincibilityTimer -= Time.deltaTime;
        } else if (isInvincible)
        {
            isInvincible = false;
        }

        Recover();
    }

    public void IncreaseExperience(int amount) 
    {
        experience += amount;

        LevelUpChecker();
    }

    void LevelUpChecker()
    {
        if(experience >= experienceCap)
        {
            level++;
            experience -= experienceCap;

            int experienceCapIncrease = 0;
            foreach (LevelRange range in levelRanges) {
                if (level >= range.startLevel && level <= range.endLevel)
                {
                    experienceCapIncrease = range.experienceCapIncrease;
                    break;
                }
            }
            experienceCap += experienceCapIncrease;
        }
    }

    public void TakeDamage(float dmg)
    {
        if (!isInvincible)
        {
            if (!isInvincible)
            {
                CurrentHealth -= dmg;

                invincibilityTimer = invincibilityDuration;
                isInvincible = true;

                if (CurrentHealth <= 0)
                {
                    Kill();
                }
            }
        }
        
    }

    public void Kill()
    {
        if (!GameManager.instance.isGameOver)
        {
            GameManager.instance.GameOver();
        }
    }

    public void RestoreHealth(float amount)
    {
        if (CurrentHealth < characterData.MaxHealth)
        {
            CurrentHealth += amount;
            if (CurrentHealth > characterData.MaxHealth)
            {
                CurrentHealth = characterData.MaxHealth;
            }
        }
    }

    void Recover()
    {
        if(CurrentHealth < characterData.MaxHealth)
        {
            CurrentHealth += CurrentRecovery * Time.deltaTime;

            if (CurrentHealth >= characterData.MaxHealth)
            {
                CurrentHealth = characterData.MaxHealth;
            }
        }
    }

    public void SpawnWeapon(GameObject weapon)
    {
        Debug.Log("Trying to spawn weapon: " + weapon);

        if (weaponIndex >= inventory.weaponSlots.Count)
        {
            Debug.LogError("Weapon inventory full");
            return;
        }

        GameObject spawnedWeapon = Instantiate(weapon, transform.position, Quaternion.identity);
        Debug.Log("Spawned weapon: " + spawnedWeapon.name);

        spawnedWeapon.transform.SetParent(transform);
        inventory.AddWeapon(weaponIndex, spawnedWeapon.GetComponent<WeaponController>());

        weaponIndex++;
    }


    public void SpawnPassiveItem(GameObject passiveItem)
    {

        if (passiveItemIndex >= inventory.passiveItemSlots.Count)
        {
            Debug.LogError("Passive item inventory full");
            return;
        }

        GameObject spawnedPassiveItem = Instantiate(passiveItem, transform.position, Quaternion.identity);
        spawnedPassiveItem.transform.SetParent(transform);
        inventory.AddPassiveItem(passiveItemIndex, spawnedPassiveItem.GetComponent<PassiveItem>());

        passiveItemIndex++;
    }

}
