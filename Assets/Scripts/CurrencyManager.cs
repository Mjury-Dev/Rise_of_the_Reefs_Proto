using UnityEngine;

public class CurrencyManager : MonoBehaviour
{
    // Singleton instance for global access
    public static CurrencyManager instance;

    // Player's current currency (serialized for Inspector visibility)
    [SerializeField]
    private int currency;

    // Public property to get the current currency, but only allow private setting
    public int Currency { get => currency; private set => currency = value; }

    // Ensure only one instance exists and load saved currency on startup
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Persist across scenes
            LoadCurrency(); // Load saved currency from PlayerPrefs
        }
        else
        {
            Destroy(gameObject); // Destroy duplicate instances
        }
    }

    /// <summary>
    /// Adds the specified amount to the player's currency.
    /// </summary>
    public void AddCurrency(int amount)
    {
        Currency += amount;
    }

    /// <summary>
    /// Attempts to spend the specified amount of currency.
    /// Returns true if successful, false if not enough currency.
    /// </summary>
    public bool SpendCurrency(int amount)
    {
        if (Currency >= amount)
        {
            Currency -= amount;
            return true;
        }
        return false;
    }

    /// <summary>
    /// Saves the current currency value to PlayerPrefs.
    /// </summary>
    public void SaveCurrency()
    {
        PlayerPrefs.SetInt("Currency", Currency);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Loads the currency value from PlayerPrefs.
    /// </summary>
    public void LoadCurrency()
    {
        Currency = PlayerPrefs.GetInt("Currency", 0);
    }

    // Automatically save currency when the application quits
    private void OnApplicationQuit()
    {
        SaveCurrency();
    }

    // Automatically save currency when the application is paused (e.g., minimized)
    private void OnApplicationPause(bool isPaused)
    {
        if (isPaused)
            SaveCurrency();
    }

    private void Update()
    {
        // Debug keybind: Press 'G' to add 1000 gold
        if (Input.GetKeyDown(KeyCode.G))
        {
            AddCurrency(1000);
            Debug.Log("[CurrencyManager] Debug key 'G' pressed: +1000 gold added. Current gold: " + Currency);
            if (ShopManager.Instance != null && ShopManager.Instance.goldCount != null)
            {
                ShopManager.Instance.UpdateDisplay();
            }
        }
    }
}
