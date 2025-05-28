using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public List<WeaponController> weaponSlots = new List<WeaponController>(2);
    public int[] weaponLevels = new int[6];
    public List<Image> weaponUISlots = new List<Image>(2);
    public List<PassiveItem> passiveItemSlots = new List<PassiveItem>(4);
    public int[] passiveItemLevels = new int[6];
    public List<Image> passiveItemUISlots = new List<Image>(4);

    public PlayerStats player;

    [System.Serializable]
    public class WeaponUpgrade
    {
        public int weaponUpgradeIndex;
        public GameObject initialWeapon;
        public WeaponScriptableObject weaponData;
    }

    [System.Serializable]

    public class PassiveItemUpgrade
    {
        public int passiveItemUpgradeIndex;
        public GameObject initialPassiveItem;
        public PassiveItemScriptableObject passiveItemData;
    }

    [System.Serializable]
    public class UpgradeUI
    {
        public TextMeshProUGUI upgradeNameDisplay;
        public TextMeshProUGUI upgradeDescription;
        public Image upgradeIcon;
        public Button upgradeButton;
    }

    public List<WeaponUpgrade> weaponUpgradeOptions = new List<WeaponUpgrade>();
    public List<PassiveItemUpgrade> passiveItemUpgradeOptions = new List<PassiveItemUpgrade>();
    public List<UpgradeUI> upgradeUIOptions = new List<UpgradeUI>();
    private void Awake()
    {
      //nothing yet
    }

    private void Start()
    {
        player = GetComponent<PlayerStats>();
        for (int i = 0; i < passiveItemUpgradeOptions.Count; i++)
        {
            passiveItemUpgradeOptions[i].passiveItemUpgradeIndex = i;
        }

    }

    public void AddWeapon(int slotIndex, WeaponController weapon)
    {
        weaponSlots[slotIndex] = weapon;
        weaponLevels[slotIndex] = weapon.weaponData.Level;
        weaponUISlots[slotIndex].enabled = true;
        weaponUISlots[slotIndex].sprite = weapon.weaponData.Icon;

        if (GameManager.instance != null && GameManager.instance.isUpgrading)
        {
            GameManager.instance.EndLevelUp();
        }
    }

    public void AddPassiveItem(int slotIndex,  PassiveItem passiveItem)
    {
        passiveItemSlots[slotIndex] = passiveItem;
        passiveItemLevels[slotIndex] = passiveItem.passiveItemData.Level;
        passiveItemUISlots[slotIndex].enabled = true;
        passiveItemUISlots[slotIndex].sprite = passiveItem.passiveItemData.Icon;

        if (GameManager.instance != null && GameManager.instance.isUpgrading)
        {
            GameManager.instance.EndLevelUp();
        }
    }
    
    public void LevelUpWeapon(int slotIndex, int upgradeIndex)
    {
        if (upgradeIndex < 0 || upgradeIndex >= weaponUpgradeOptions.Count)
        {
            Debug.LogWarning("Invalid weapon upgrade index: " + upgradeIndex);
            return;
        }

        if (weaponSlots.Count > slotIndex)
        {
            WeaponController weapon = weaponSlots[slotIndex];
            if (!weapon.weaponData.NextLevelPrefab)
            {
                Debug.LogError("No next level for " + weapon.name);
                return;
            }
            GameObject upgradeWeapon = Instantiate(weapon.weaponData.NextLevelPrefab, transform.position, Quaternion.identity);
            upgradeWeapon.transform.SetParent(transform);
            AddWeapon(slotIndex, upgradeWeapon.GetComponent<WeaponController>());
            Destroy(weapon.gameObject);
            weaponLevels[slotIndex] = upgradeWeapon.GetComponent<WeaponController>().weaponData.Level;

            weaponUpgradeOptions[upgradeIndex].weaponData = upgradeWeapon.GetComponent<WeaponController>().weaponData;

            if (GameManager.instance != null && GameManager.instance.isUpgrading)
            {
                GameManager.instance.EndLevelUp();
            }
        }
    }

    public void LevelUpPassiveItem(int slotIndex,int upgradeIndex)
    {
        if (upgradeIndex < 0 || upgradeIndex >= passiveItemUpgradeOptions.Count)
        {
            Debug.LogWarning("Invalid passive upgrade index: " + upgradeIndex);
            return;
        }

        if (passiveItemSlots.Count > slotIndex)
        {
            PassiveItem passiveItem = passiveItemSlots[slotIndex];
            if (!passiveItem.passiveItemData.NextLevelPrefab)
            {
                Debug.LogError("No next level for " + passiveItem.name);
                return;
            }
            GameObject upgradePassiveItem = Instantiate(passiveItem.passiveItemData.NextLevelPrefab, transform.position, Quaternion.identity);
            upgradePassiveItem.transform.SetParent(transform);
            AddPassiveItem(slotIndex, upgradePassiveItem.GetComponent<PassiveItem>());
            Destroy(passiveItem.gameObject);
            passiveItemLevels[slotIndex] = upgradePassiveItem.GetComponent<PassiveItem>().passiveItemData.Level;

            passiveItemUpgradeOptions[upgradeIndex].passiveItemData = upgradePassiveItem.GetComponent<PassiveItem>().passiveItemData;

            if (GameManager.instance != null && GameManager.instance.isUpgrading)
            {
                GameManager.instance.EndLevelUp();
            }
        }
    }

    void ApplyUpgradeOptions()
    {
        List<WeaponUpgrade> availableWeaponUpgrades = new List<WeaponUpgrade>(weaponUpgradeOptions);
        List<PassiveItemUpgrade> availablePassiveItemUpgrades = new List<PassiveItemUpgrade>(passiveItemUpgradeOptions);

        foreach (var upgradeOption in upgradeUIOptions)
        {
            if (availableWeaponUpgrades.Count == 0 && availablePassiveItemUpgrades.Count == 0)
            {
                return;
            }

            int upgradeType;

            if (availableWeaponUpgrades.Count == 0)
            {
                upgradeType = 2;
            }
            else if (availablePassiveItemUpgrades.Count == 0)
            {
                upgradeType = 1;
            }
            else
            {
                upgradeType = Random.Range(1, 3); // 1 = weapon, 2 = passive
            }

            if (upgradeType == 1)
            {
                WeaponUpgrade chosen = availableWeaponUpgrades[Random.Range(0, availableWeaponUpgrades.Count)];
                availableWeaponUpgrades.Remove(chosen);

                if (chosen != null)
                {
                    EnableUpgradeUI(upgradeOption);
                    bool alreadyOwned = false;
                    int index = -1;

                    for (int i = 0; i < weaponSlots.Count; i++)
                    {
                        if (weaponSlots[i] != null && weaponSlots[i].weaponData == chosen.weaponData)
                        {
                            alreadyOwned = true;
                            index = i;
                            break;
                        }
                    }

                    if (alreadyOwned)
                    {
                        if (chosen.weaponData.NextLevelPrefab == null)
                        {
                            DisableUpgradeUI(upgradeOption);
                            continue;
                        }

                        upgradeOption.upgradeButton.onClick.AddListener(() => LevelUpWeapon(index, chosen.weaponUpgradeIndex));
                        upgradeOption.upgradeDescription.text = chosen.weaponData.NextLevelPrefab.GetComponent<WeaponController>().weaponData.Description;
                        upgradeOption.upgradeNameDisplay.text = chosen.weaponData.NextLevelPrefab.GetComponent<WeaponController>().weaponData.WeaponName;
                    }
                    else
                    {
                        upgradeOption.upgradeButton.onClick.AddListener(() => player.SpawnWeapon(chosen.initialWeapon));
                        upgradeOption.upgradeDescription.text = chosen.weaponData.Description;
                        upgradeOption.upgradeNameDisplay.text = chosen.weaponData.WeaponName;
                    }

                    upgradeOption.upgradeIcon.sprite = chosen.weaponData.Icon;
                }
            }
            else // upgradeType == 2
            {
                PassiveItemUpgrade chosen = availablePassiveItemUpgrades[Random.Range(0, availablePassiveItemUpgrades.Count)];
                availablePassiveItemUpgrades.Remove(chosen);

                if (chosen != null)
                {
                    EnableUpgradeUI(upgradeOption);
                    bool alreadyOwned = false;
                    int index = -1;

                    for (int i = 0; i < passiveItemSlots.Count; i++)
                    {
                        if (passiveItemSlots[i] != null && passiveItemSlots[i].passiveItemData == chosen.passiveItemData)
                        {
                            alreadyOwned = true;
                            index = i;
                            break;
                        }
                    }

                    if (alreadyOwned)
                    {
                        if (chosen.passiveItemData.NextLevelPrefab == null)
                        {
                            DisableUpgradeUI(upgradeOption);
                            continue;
                        }

                        upgradeOption.upgradeButton.onClick.AddListener(() => LevelUpPassiveItem(index, chosen.passiveItemUpgradeIndex));
                        upgradeOption.upgradeDescription.text = chosen.passiveItemData.NextLevelPrefab.GetComponent<PassiveItem>().passiveItemData.Description;
                        upgradeOption.upgradeNameDisplay.text = chosen.passiveItemData.NextLevelPrefab.GetComponent<PassiveItem>().passiveItemData.PassiveItemName;
                    }
                    else
                    {
                        upgradeOption.upgradeButton.onClick.AddListener(() => player.SpawnPassiveItem(chosen.initialPassiveItem));
                        upgradeOption.upgradeDescription.text = chosen.passiveItemData.Description;
                        upgradeOption.upgradeNameDisplay.text = chosen.passiveItemData.PassiveItemName;
                    }

                    upgradeOption.upgradeIcon.sprite = chosen.passiveItemData.Icon;
                }
            }
        }
    }


    void RemoveUpgradeOptions()
    {
        foreach (var upgradeOption in upgradeUIOptions)
        {
            upgradeOption.upgradeButton.onClick.RemoveAllListeners();
            DisableUpgradeUI(upgradeOption);
        }
    }

    public void RemoveAndApplyUpgrades()
    {
        RemoveUpgradeOptions();
        ApplyUpgradeOptions();  
    }

    void DisableUpgradeUI(UpgradeUI ui)
    {
        ui.upgradeNameDisplay.transform.parent.gameObject.SetActive(false);
    }

    void EnableUpgradeUI(UpgradeUI ui)
    {
        ui.upgradeNameDisplay.transform.parent.gameObject.SetActive(true);
    }
}
