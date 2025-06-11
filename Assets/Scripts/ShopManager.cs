using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    public Sprite[] upgradeSprites;

    public static ShopManager Instance { get; private set; }

    [Header("Upgrade Costs")]
    //These values have to be adjusted in the inspector
    public int[] strengthUpgradeCosts = { 100, 250, 600, 1500 };
    public int[] recoveryUpgradeCosts = { 80, 220, 550, 1400 };
    public int[] speedUpgradeCosts = { 120, 300, 750, 1800 };
    public int[] magnetUpgradeCosts = { 90, 240, 620, 1600 };
    public int[] healthUpgradeCosts = { 110, 280, 700, 1700 };

    [Header("Strength")]
    public Image strengthUpgradeBar;
    public Image strengthUpgradeButton;
    public TextMeshProUGUI strengthUpgradeCost;

    [Header("Recovery")]
    public Image recoveryUpgradeBar;
    public Image recoveryUpgradeButton;
    public TextMeshProUGUI recoveryUpgradeCost;

    [Header("Speed")]
    public Image speedUpgradeBar;
    public Image speedUpgradeButton;
    public TextMeshProUGUI speedUpgradeCost;

    [Header("Magnet")]
    public Image magnetUpgradeBar;
    public Image magnetUpgradeButton;
    public TextMeshProUGUI magnetUpgradeCost;

    [Header("Health")]
    public Image healthUpgradeBar;
    public Image healthUpgradeButton;
    public TextMeshProUGUI healthUpgradeCost;

    public TextMeshProUGUI goldCount;

    [SerializeField] int strengthPoints;
    [SerializeField] int recoveryPoints;
    [SerializeField] int speedPoints;
    [SerializeField] int magnetPoints;
    [SerializeField] int healthPoints;
    [SerializeField] int currentGold;

    private void Awake()
    {

        Instance = this;

        strengthPoints = PlayerUpgradeManager.Instance.GetPoints(0);
        recoveryPoints = PlayerUpgradeManager.Instance.GetPoints(1);
        speedPoints = PlayerUpgradeManager.Instance.GetPoints(2);
        magnetPoints = PlayerUpgradeManager.Instance.GetPoints(3);
        healthPoints = PlayerUpgradeManager.Instance.GetPoints(4);
        currentGold = CurrencyManager.instance.Currency;
    }

    void Start()
    {
        UpdateDisplay();
        // Reset(); // Debug use only, remove in production
    }

    void Update()
    {
        // Debug keybind: Press 'R' to reset all upgrades and refresh the display
        if (Input.GetKeyDown(KeyCode.R))
        {
            Debug.Log("[ShopManager] Debug key 'R' pressed: Reset() called.");
            Reset();
            UpdateDisplay();
        }

        // If Escape is pressed in Shop scene, return to Menu
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (SceneManager.GetActiveScene().name == "Shop")
            {
                SceneManager.LoadScene("Menu");
            }
        }
    }

    public void Upgrade(int upgradeType)
    {
        int cost = GetUpgradeCost(upgradeType);

        if (cost > 0 && CurrencyManager.instance.Currency >= cost)
        {
            CurrencyManager.instance.SpendCurrency(cost);
            currentGold = CurrencyManager.instance.Currency;
            PlayerUpgradeManager.Instance.AddPoint(upgradeType);

            // Refresh local point values
            strengthPoints = PlayerUpgradeManager.Instance.GetPoints(0);
            recoveryPoints = PlayerUpgradeManager.Instance.GetPoints(1);
            speedPoints = PlayerUpgradeManager.Instance.GetPoints(2);
            magnetPoints = PlayerUpgradeManager.Instance.GetPoints(3);
            healthPoints = PlayerUpgradeManager.Instance.GetPoints(4);

            UpdateDisplay();
        }
        else
        {
            Debug.Log("Not enough gold or maxed out.");
        }
    }


    private int GetUpgradeCost(int upgradeType)
    {
        int points = PlayerUpgradeManager.Instance.GetPoints(upgradeType);
        int[] costs = GetUpgradeCostsArray(upgradeType);

        if (costs == null || points >= costs.Length)
            return 0; // Maxed out or invalid type

        return costs[points];
    }

    private int[] GetUpgradeCostsArray(int upgradeType)
    {
        switch (upgradeType)
        {
            case 0: return strengthUpgradeCosts;
            case 1: return recoveryUpgradeCosts;
            case 2: return speedUpgradeCosts;
            case 3: return magnetUpgradeCosts;
            case 4: return healthUpgradeCosts;
            default: return null;
        }
    }


    public void UpdateDisplay()
    {
        currentGold = CurrencyManager.instance.Currency; 

        UpdateUpgradeDisplay(0, strengthPoints, strengthUpgradeBar, strengthUpgradeButton, strengthUpgradeCost, strengthUpgradeCosts);
        UpdateUpgradeDisplay(1, recoveryPoints, recoveryUpgradeBar, recoveryUpgradeButton, recoveryUpgradeCost, recoveryUpgradeCosts);
        UpdateUpgradeDisplay(2, speedPoints, speedUpgradeBar, speedUpgradeButton, speedUpgradeCost, speedUpgradeCosts);
        UpdateUpgradeDisplay(3, magnetPoints, magnetUpgradeBar, magnetUpgradeButton, magnetUpgradeCost, magnetUpgradeCosts);
        UpdateUpgradeDisplay(4, healthPoints, healthUpgradeBar, healthUpgradeButton, healthUpgradeCost, healthUpgradeCosts);

        goldCount.text = currentGold.ToString();
    }


    void UpdateUpgradeDisplay(int type, int points, Image bar, Image button, TextMeshProUGUI costText, int[] costs)
    {
        bar.sprite = upgradeSprites[Mathf.Clamp(points, 0, upgradeSprites.Length - 1)];

        bool isMaxed = points >= costs.Length;
        costText.text = isMaxed ? "Maxed" : costs[points].ToString();

        bool canBuy = !isMaxed && CanBuy(type);
        button.gameObject.SetActive(canBuy);
        if (canBuy)
        { 
            costText.color = Color.yellow;
        }
        else
        {
            costText.color = Color.red;
        }
    }

    //Debug use only
    private void Reset()
    {
        strengthPoints = 0;
        recoveryPoints = 0;
        speedPoints = 0;
        magnetPoints = 0;
        healthPoints = 0;
        PlayerUpgradeManager.Instance.ResetUpgrades();
        UpdateDisplay();
    }

    private bool CanBuy(int upgradeType)
    {
        int index = PlayerUpgradeManager.Instance.GetPoints(upgradeType);

        switch (upgradeType)
        {
            case 0: // Strength
                return index < strengthUpgradeCosts.Length && currentGold >= strengthUpgradeCosts[index];
            case 1: // Recovery
                return index < recoveryUpgradeCosts.Length && currentGold >= recoveryUpgradeCosts[index];
            case 2: // Speed
                return index < speedUpgradeCosts.Length && currentGold >= speedUpgradeCosts[index];
            case 3: // Magnet
                return index < magnetUpgradeCosts.Length && currentGold >= magnetUpgradeCosts[index];
            case 4: // Health
                return index < healthUpgradeCosts.Length && currentGold >= healthUpgradeCosts[index];
            default:
                return false;
        }
    }
}
