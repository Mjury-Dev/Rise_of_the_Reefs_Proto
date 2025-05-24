using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    IEnumerator ShowGameOverTextAfterDelay(float delay1, float delay2)
    {
        yield return new WaitForSecondsRealtime(delay1);
        gameOverSubtext.gameObject.SetActive(true);
        yield return new WaitForSecondsRealtime(delay2);
        backToMenuButton.gameObject.SetActive(true);
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

    [Header("UI")]
    public GameObject pauseScreen;
    public GameObject gameOverScreen;

    [Header("Game Over UI")]
    public TextMeshProUGUI gameOverSubtext;
    public Button backToMenuButton;

    [Header("Current Stats")]
    public TextMeshProUGUI currentHealthDisplay;
    public TextMeshProUGUI currentRecoveryDisplay;
    public TextMeshProUGUI currentMoveSpeedDisplay;
    public TextMeshProUGUI currentStrengthDisplay;
    public TextMeshProUGUI currentProjectileSpeedDisplay;
    public TextMeshProUGUI currentMagnetDisplay;

    public bool isGameOver = false;
    public bool isPaused = false;

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
                    DisplayResults();
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
    }

    public void GameOver()
    {
        ChangeState(GameState.GameOver);
    }

    void DisplayResults()
    {
        gameOverSubtext.gameObject.SetActive(false);
        backToMenuButton.gameObject.SetActive(false);
        gameOverScreen.SetActive(true);
        StartCoroutine(ShowGameOverTextAfterDelay(3f, 2f));
    }
}
