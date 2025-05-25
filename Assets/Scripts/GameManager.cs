using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
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

        minutes = 0;
        seconds = 0;
        timer.text = "00:00";
    }

    IEnumerator ShowGameOverTextAfterDelay(float delay1, float delay2)
    {
        yield return new WaitForSecondsRealtime(delay1);
        gameOverSubtext.gameObject.SetActive(true);
        yield return new WaitForSecondsRealtime(delay2);
        backToMenuButton.gameObject.SetActive(true);
        showResultsButton.gameObject.SetActive(true);
    }

    public static GameManager instance;

    public enum GameState
    {
        Gameplay,
        Paused,
        GameOver
    }

    public GameState currentState;
    public GameState previousState;

    [Header("UI Screens")]
    public GameObject pauseScreen;
    public GameObject gameOverScreen;
    public GameObject resultsScreen;

    [Header("Game UI")]
    public TextMeshProUGUI levelCounter;
    public TextMeshProUGUI healthCounter;
    public TextMeshProUGUI expCounter;
    public TextMeshProUGUI killCounter;
    public TextMeshProUGUI timer;

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
    public List<Image> chosenWeaponsUI = new List<Image>(2);
    public List<Image> chosenPassiveItemsUI = new List<Image>(4);

    public float countdownTime = 1200f;
    public float timeLeft;

    public bool isGameOver = false;
    public bool isPaused = false;

    public int enemiesKilled;
    public int minutes;
    public int seconds;

    private void Update()
    {
        switch (currentState)
        {
            case GameState.Gameplay:
                CheckForPauseAndResume();
                isPaused = false;
                break;
            case GameState.Paused:
                CheckForPauseAndResume();
                isPaused = true;
                break;
            case GameState.GameOver:
                if (!isGameOver)
                {
                    isGameOver = true;
                    Time.timeScale = 0f;
                    Debug.Log("Game over lmao");
                    DisplayGameOver();
                }
                break;
            default:
                Debug.LogWarning("GAMESTATE INVALID");  
                break;
        }
    }

    private void Awake()
    {
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
    }

    private void Start()
    {
        StartCoroutine(CountDownTimer());
        UpdateKillCounter();
    }
    public void ChangeState(GameState newState) 
    {
        currentState = newState;
    }
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
    void CheckForPauseAndResume()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("Pressed Escape");
            if (currentState == GameState.Paused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    void DisableScreens()
    {
        pauseScreen.SetActive(false);
        gameOverScreen.SetActive(false);
        resultsScreen.SetActive(false);
    }

    public void GameOver()
    {
        ChangeState(GameState.GameOver);
        StopCoroutine(CountDownTimer());
    }

    public void ShowResultsScreen()
    {
        DisplayResults();
    }

    void DisplayGameOver()
    {
        gameOverSubtext.gameObject.SetActive(false);
        backToMenuButton.gameObject.SetActive(false);
        showResultsButton.gameObject.SetActive(false);
        gameOverScreen.SetActive(true);
        StartCoroutine(ShowGameOverTextAfterDelay(3f, 2f));
    }

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

    public void AssignEnemiesKilled()
    {
        enemiesKilledValue.text = enemiesKilled.ToString();
    }

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

    public void ShowTimeSurvived()
    {
        int minutesPassed = Mathf.FloorToInt((countdownTime - timeLeft) / 60f);
        int secondsPassed = Mathf.FloorToInt((countdownTime - timeLeft) % 60f);
        timeSurvived.text = $"{minutesPassed:00}:{secondsPassed:00}";
    }

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

}
