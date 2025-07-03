using UnityEngine;
using System; // Required for using Action

public class PollutionManager : MonoBehaviour
{
    // Singleton instance for global access
    public static PollutionManager instance;

    // Event that other scripts can subscribe to, to be notified when the pollution level changes.
    // This is more efficient than constantly checking the value in an Update() loop.
    public static event Action<float> OnPollutionChanged;

    // Global pollution level (serialized for Inspector visibility)
    // We use a float for more granular control (e.g., 99.5%)
    [SerializeField]
    private float pollutionLevel;

    // Public property to get the current pollution level.
    // The setter is private and includes clamping to ensure the value stays between 0 and 100.
    public float PollutionLevel
    {
        get => pollutionLevel;
        private set
        {
            // Clamp the value to ensure it never goes outside the 0-100 range
            pollutionLevel = Mathf.Clamp(value, 0f, 100f);
            // Fire the event to notify any listeners (like a UI manager) that the value has changed.
            OnPollutionChanged?.Invoke(pollutionLevel);
        }
    }

    // Ensure only one instance exists and load saved pollution level on startup
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Persist across scenes
            LoadPollution(); // Load saved pollution level from PlayerPrefs
        }
        else
        {
            Destroy(gameObject); // Destroy duplicate instances
        }
    }

    // --- Subscribe to the GameOver event when this object is enabled ---
    private void OnEnable()
    {
        // Subscribe the SavePollution method to the OnGameOver event.
        // Now, whenever GameManager.OnGameOver is invoked, SavePollution will be called.
        GameManager.OnGameOver += SavePollution;
        Debug.Log("[PollutionManager] Subscribed to GameManager.OnGameOver event.", this);
    }

    // --- Unsubscribe from the event when this object is disabled ---
    private void OnDisable()
    {
        // It's crucial to unsubscribe when the object is disabled to prevent errors
        // and memory leaks if the GameManager is destroyed before this manager.
        GameManager.OnGameOver -= SavePollution;
        Debug.Log("[PollutionManager] Unsubscribed from GameManager.OnGameOver event.", this);
    }


    private void Start()
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.UpdatePollutionLevel(PollutionLevel); // Initialize the game manager with the current pollution level
        }
    }

    /// <summary>
    /// Decreases the global pollution level by the specified amount.
    /// </summary>
    /// <param name="amountToDecrease">The percentage amount to decrease pollution by.</param>
    public void DecreasePollution(float amountToDecrease)
    {
        // We only allow positive values to be passed in to avoid accidental increases.
        if (amountToDecrease > 0)
        {
            PollutionLevel -= amountToDecrease;
            Debug.Log($"[PollutionManager] Pollution decreased by {amountToDecrease}. Current pollution: {PollutionLevel}%.");
        }
    }

    /// <summary>
    /// Saves the current pollution level to PlayerPrefs. This is our single, reliable save point.
    /// </summary>
    public void SavePollution()
    {
        PlayerPrefs.SetFloat("PollutionLevel", PollutionLevel);
        PlayerPrefs.Save();
        Debug.Log($"[PollutionManager] SavePollution() called. Saving PollutionLevel: {PollutionLevel} to PlayerPrefs.", this);
    }

    /// <summary>
    /// Loads the pollution level from PlayerPrefs. Defaults to 100 if no saved value is found.
    /// </summary>
    public void LoadPollution()
    {
        // When loading, if no key is found, default to 100%.
        PollutionLevel = PlayerPrefs.GetFloat("PollutionLevel", 100f);
        Debug.Log($"[PollutionManager] LoadPollution() called. Loaded PollutionLevel: {PollutionLevel} from PlayerPrefs.", this);
    }

    private void ResetPollutionLevel()
    {
        // Reset pollution level to 100% (or any other default value you choose)
        PollutionLevel = 100f;
        Debug.Log("Pollution level reset to 100%.");
    }

    private void Update()
    {
        // Debug keybind: Press 'P' to decrease pollution by 5%
        if (Input.GetKeyDown(KeyCode.LeftBracket))
        {
            DecreasePollution(0.2f);
            if (GameManager.instance != null)
            {
                GameManager.instance.UpdatePollutionLevel(PollutionLevel); // Update UI if needed
            }
        }
        if (Input.GetKeyDown(KeyCode.RightBracket))
        {
            ResetPollutionLevel();
            if (GameManager.instance != null)
            {
                GameManager.instance.UpdatePollutionLevel(PollutionLevel); // Update UI if needed
            }
        }
    }
}
