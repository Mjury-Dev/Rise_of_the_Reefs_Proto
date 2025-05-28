using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseGame : MonoBehaviour
{
    public GameObject pauseMenuUI;
    public Button resumeButton;
    public Button restartButton;
    public Button quitButton;

    private Canvas[] allCanvases;

    private bool isPaused = false;

    void Start()
    {
        pauseMenuUI.SetActive(false);

        allCanvases = FindObjectsOfType<Canvas>(true);

        resumeButton.onClick.AddListener(ResumeGame);
        restartButton.onClick.AddListener(RestartGame);
        quitButton.onClick.AddListener(QuitGame);
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                ResumeGame();
            else
                Pause();
        }
    }

    void Pause()
    {
        foreach (Canvas canvas in allCanvases)
        {
            if (!pauseMenuUI.transform.IsChildOf(canvas.transform) && canvas.gameObject != pauseMenuUI)
            {
                canvas.gameObject.SetActive(false);
            }
        }

        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }



    void ResumeGame()
    {
        foreach (Canvas canvas in allCanvases)
        {
            canvas.gameObject.SetActive(true);
        }

        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }


    void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void QuitGame()
    {
        Time.timeScale = 1f;
        Application.Quit();
        // Uncomment this if testing in Unity Editor
        // UnityEditor.EditorApplication.isPlaying = false;
    }
}
