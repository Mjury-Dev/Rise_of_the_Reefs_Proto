using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    // Coroutine for countdown timer; triggers game over when time runs out
    IEnumerator CountDownTimer()
    {
        timeLeft = countdownTime;

        while (timeLeft > 0)
        {
            minutes = Mathf.FloorToInt(timeLeft / 60f);
            seconds = Mathf.FloorToInt(timeLeft % 60f);
            timer.text = $"{minutes:00}:{seconds:00}";

            yield return new WaitForSeconds(1f);
            timeLeft -= 1f;
        }
        // Time's up - kill player and update timer UI
        playerObject.GetComponent<PlayerStats>()?.Kill();
        minutes = 0;
        seconds = 0;
        timer.text = "00:00";
    }

    // Coroutine to delay display of game over text and buttons
    IEnumerator ShowGameOverTextAfterDelay(float delay1, float delay2)
    {
        yield return new WaitForSecondsRealtime(delay1);
        gameOverSubtext.gameObject.SetActive(true);
        yield return new WaitForSecondsRealtime(delay2);
        backToMenuButton.gameObject.SetActive(true);
        showResultsButton.gameObject.SetActive(true);
    }

    public static GameManager instance;

    // Enum representing various game states
    public enum GameState
    {
        Gameplay,
        Paused,
        GameOver,
        LevelUp,
    }

    public GameState currentState;
    public GameState previousState;

    [Header("UI Screens")]
    public GameObject pauseScreen;
    public GameObject gameOverScreen;
    public GameObject resultsScreen;
    public GameObject levelUpScreen;

    [Header("Game UI")]
    public TextMeshProUGUI levelCounter;
    public TextMeshProUGUI healthCounter;
    public TextMeshProUGUI expCounter;
    public TextMeshProUGUI killCounter;
    public TextMeshProUGUI goldCounter;
    public TextMeshProUGUI pollutionLevel;
    public TextMeshProUGUI timer;
    public TextMeshProUGUI maxPetsCounter;

    [Header("Game Over UI")]
    public TextMeshProUGUI gameOverSubtext;
    public Button showResultsButton;
    public Button backToMenuButton;

    [Header("Current Stats")]
    public TextMeshProUGUI currentHealthDisplay;
    public TextMeshProUGUI currentRecoveryDisplay;
    public TextMeshProUGUI currentMoveSpeedDisplay;
    public TextMeshProUGUI currentStrengthDisplay;
    public TextMeshProUGUI currentProjectileSpeedDisplay;
    public TextMeshProUGUI currentMagnetDisplay;

    [Header("Results Screen UI")]
    public Image chosenCharacterImage;
    public TextMeshProUGUI levelReachedValue;
    public TextMeshProUGUI enemiesKilledValue;
    public TextMeshProUGUI timeSurvived;
    public TextMeshProUGUI goldEarned;
    public TextMeshProUGUI pollutionCleared;
    public List<Image> chosenWeaponsUI = new List<Image>(2);
    public List<Image> chosenPassiveItemsUI = new List<Image>(4);

    [Header("Damage Text Settings")]
    public Canvas damageTextCanvas;
    public float textFontSize = 20;
    public TMP_FontAsset textFont;
    public Camera referenceCamera;

    public static event Action OnGameOver;

    public float countdownTime = 1200f;
    public float timeLeft;

    public bool isGameOver = false;
    public bool isPaused = false;
    public bool isUpgrading = false;

    public float runStartPollutionLevel; // Pollution level at the start of the run
    public float runEndPollutionLevel; // Pollution level at the end of the run

    public int enemiesKilled;
    public int minutes;
    public int seconds;

    public GameObject playerObject;

    private void Update()
    {
        // State machine to handle game behavior based on current state
        // Debug.Log("isupgrading: " + isUpgrading);
        switch (currentState)
        {
            case GameState.Gameplay:
                CheckForPauseAndResume();
                break;
            case GameState.Paused:
                CheckForPauseAndResume();
                break;
            case GameState.GameOver:
                if (!isGameOver)
                {
                    isGameOver = true;
                    Time.timeScale = 0f;
                    Debug.Log("Game over lmao");
                    DisplayGameOver();
                    OnGameOver?.Invoke();
                    AudioManager.Instance.StopBGM();
                    AudioManager.Instance.PlayBGM("GameOverTheme");
                }
                break;
            case GameState.LevelUp:
                if (!isUpgrading)
                {
                    Debug.Log("Entered LevelUp state and triggering UI...");
                    isUpgrading = true;
                    Time.timeScale = 0f;
                    levelUpScreen.SetActive(true);
                }
                break;
            default:
                Debug.LogWarning("GAMESTATE INVALID");  
                break;
        }

        // Debug keybind: Press 'K' to kill the player instantly
        if (Input.GetKeyDown(KeyCode.K))
        {
            if (playerObject != null)
            {
                var playerStats = playerObject.GetComponent<PlayerStats>();
                if (playerStats != null)
                {
                    Debug.Log("[GameManager] Debug key 'K' pressed: Killing player.");
                    playerStats.Kill();
                }
                else
                {
                    Debug.LogWarning("[GameManager] PlayerStats component not found on playerObject.");
                }
            }
            else
            {
                Debug.LogWarning("[GameManager] playerObject reference is null.");
            }
        }
    }

    private void Awake()
    {
        // Singleton pattern to ensure only one GameManager exists
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogWarning("EXTRA" + this + " DELETED");
            Destroy(gameObject);
        }
        DisableScreens();
        isUpgrading = false;
    }

    private void Start()
    {
        StartCoroutine(CountDownTimer());
        UpdateKillCounter();
    }

    // Static method to display floating damage text
    public static void GenerateFloatingText(string text, Transform target, float duration = 1f, float speed = 1f)
    {
        if (!instance.damageTextCanvas) return;

        if (!instance.referenceCamera) instance.referenceCamera = Camera.main;

        instance.StartCoroutine(instance.GenerateFloatingTextCoroutine(text, target, duration, speed));
    }

    // Coroutine to animate floating text above a target
    IEnumerator GenerateFloatingTextCoroutine(string text, Transform target, float duration = 1f, float speed = 50f)
    {
        if (target == null || instance?.damageTextCanvas == null || referenceCamera == null)
            yield break;

        // Create and configure text object
        GameObject textObj = new GameObject("Damage Floating Text", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        textObj.transform.SetParent(instance.damageTextCanvas.transform, false);

        RectTransform rect = textObj.GetComponent<RectTransform>();
        TextMeshProUGUI tmPro = textObj.GetComponent<TextMeshProUGUI>();

        tmPro.text = text;
        tmPro.alignment = TextAlignmentOptions.Center;
        tmPro.fontSize = textFontSize;
        if (textFont != null) tmPro.font = textFont;

        Vector3 initialWorldPos = target.position;
        Vector3 screenPos = referenceCamera.WorldToScreenPoint(initialWorldPos);
        rect.position = screenPos;

        float elapsed = 0f;
        float yOffset = 0f;
        Color originalColor = tmPro.color;

        while (elapsed < duration)
        {
            yield return null;
            elapsed += Time.deltaTime;

            if (target == null)
                break;

            float alpha = Mathf.Lerp(1f, 0f, elapsed / duration);
            tmPro.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);

            yOffset += speed * Time.deltaTime;
            Vector3 worldPos = target.position + new Vector3(0, yOffset, 0);
            rect.position = referenceCamera.WorldToScreenPoint(worldPos);
        }

        Destroy(textObj);
    }


    // Change game state
    public void ChangeState(GameState newState) 
    {
        currentState = newState;
    }

    // Pause the game
    public void PauseGame()
    {
        if (currentState == GameState.GameOver)
            return;

        if (currentState != GameState.Paused)
        {
            previousState = currentState;
            ChangeState(GameState.Paused);
            Time.timeScale = 0f;
            pauseScreen.SetActive(true);
            Debug.Log("Game Paused");
        }
    }

    // Resume game from pause
    public void ResumeGame()
    {
        if (currentState == GameState.Paused)
        {
            ChangeState(previousState);
            Time.timeScale = 1f;
            pauseScreen.SetActive(false);
            gameOverScreen.SetActive(false);
            Debug.Log("Game Resumed");

        }
    }

    public void ClickResume()
    {
        if (currentState == GameState.Paused)
        {
            isPaused = false;
            ResumeGame();
        }
    }

    // Check for Escape key to toggle pause
    void CheckForPauseAndResume()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("Pressed Escape");
            if (currentState == GameState.Paused)
            {
                isPaused = false;
                ResumeGame();
            }
            else
            {
                isPaused = true;
                PauseGame();
            }
        }
    }

    // Hide all UI screens

    void DisableScreens()
    {
        pauseScreen.SetActive(false);
        gameOverScreen.SetActive(false);
        resultsScreen.SetActive(false);
        levelUpScreen.SetActive(false);
    }

    // Trigger game over logic
    public void GameOver()
    {
        ChangeState(GameState.GameOver);
        StopCoroutine(CountDownTimer());
    }

    // Show game over screen with delays
    public void ShowResultsScreen()
    {
        DisplayResults();
    }

    // Populate results screen values
    void DisplayGameOver()
    {
        gameOverSubtext.gameObject.SetActive(false);
        backToMenuButton.gameObject.SetActive(false);
        showResultsButton.gameObject.SetActive(false);
        gameOverScreen.SetActive(true);
        StartCoroutine(ShowGameOverTextAfterDelay(3f, 4.5f));
    }

    // UI assignment methods for stats
    void DisplayResults()
    {
        ShowTimeSurvived();
        AssignEnemiesKilled();
        gameOverScreen.SetActive(false);
        resultsScreen.SetActive(true);
    }

    public void AssignChosenCharacterUI(CharacterScriptableObject chosenCharacterData)
    {
        chosenCharacterImage.sprite = chosenCharacterData.Icon;
    }

    public void AssignLevelReached(int levelReachedData)
    {
        levelReachedValue.text = levelReachedData.ToString();
    }

    public void AssignGoldEarned(int goldEarnedData)
    {
        goldEarned.text = goldEarnedData.ToString();
    }

    public void AssignEnemiesKilled()
    {
        enemiesKilledValue.text = enemiesKilled.ToString();
    }

    // Update various UI counters
    public void UpdateLevelCounter(int levelData)
    {
        levelCounter.text = "LVL " + levelData.ToString();
    }

    public void UpdateHealthCounter(float healthData, int maxHealthData)
    {
        healthCounter.text = healthData.ToString("F0") + " / " + maxHealthData.ToString();
    }

    public void UpdateKillCounter()
    {
        killCounter.text = enemiesKilled.ToString();
    }
    public void EnemyKilled()
    {
        enemiesKilled++;
    }
    public void UpdateExpCounter(int exp, int expCap)
    {
        if (exp == expCap) 
        {
            exp = 0;
        }
        expCounter.text = exp.ToString() + " / " + expCap.ToString();
    }
    public void UpdatePollutionLevel(float pollutionLevelData)
    {
        // Optional: Clamp the value to ensure it's within the 0-100 range
        float clampedPollution = Mathf.Clamp(pollutionLevelData, 0f, 100f);

        // Update the text display (using F2 for more precision if you like)
        pollutionLevel.text = $"{clampedPollution:F0}%";

        // Normalize the pollution value to a 0-1 range (e.g., 50% -> 0.5)
        float normalizedPollution = clampedPollution / 100f;

        // Linearly interpolate between green and red based on the normalized value
        pollutionLevel.color = Color.Lerp(Color.green, Color.red, normalizedPollution);
    }

    public void UpdateGoldcounter(int goldAmount)
    {
        goldCounter.text = goldAmount.ToString();
    }

    public void UpdatePetCounter(int currentPets, int maxPets)
    {
        maxPetsCounter.text = $"{currentPets} / {maxPets}";
    }

    public void ShowTimeSurvived()
    {
        int minutesPassed = Mathf.FloorToInt((countdownTime - timeLeft) / 60f);
        int secondsPassed = Mathf.FloorToInt((countdownTime - timeLeft) % 60f);
        timeSurvived.text = $"{minutesPassed:00}:{secondsPassed:00}";
    }

    // Assign UI icons for chosen weapons and passive items
    public void AssignChosenWeaponAndPassiveItemsUI(List<Image> chosenWeaponsData, List<Image> chosenPassiveItemsData)
    {
        if (chosenWeaponsData.Count != chosenWeaponsUI.Count || chosenPassiveItemsData.Count != chosenPassiveItemsUI.Count)
        {
            Debug.Log("Chosen weapons and passive item datalist have different lengths");
            return;
        }
        
        for (int i = 0; i < chosenWeaponsUI.Count; i++)
        {
            if (chosenWeaponsData[i].sprite)
            {
                chosenWeaponsUI[i].enabled = true;
                chosenWeaponsUI[i].sprite = chosenWeaponsData[i].sprite;
            }
            else
            {
                chosenWeaponsData[i].enabled = false;
            }

        }
        for (int i = 0; i < chosenPassiveItemsUI.Count; i++)
        {
            if (chosenPassiveItemsData[i].sprite)
            { 
                chosenPassiveItemsUI[i].enabled = true;
                chosenPassiveItemsUI[i].sprite = chosenPassiveItemsData[i].sprite;
            }
            else
            {
                chosenPassiveItemsData[i].enabled = false;
            }
        }
    }

    // Trigger level-up UI
    public void StartLevelUp()
    {
        ChangeState(GameState.LevelUp);
        playerObject.SendMessage("RemoveAndApplyUpgrades");
    }

    // Exit level-up state and resume gameplay
    public void EndLevelUp()
    {
        isUpgrading = false;
        Time.timeScale = 1f;
        levelUpScreen.SetActive(false);
        ChangeState(GameState.Gameplay);
    }

}
