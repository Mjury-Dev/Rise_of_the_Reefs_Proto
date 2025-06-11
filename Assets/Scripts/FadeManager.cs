using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class FadeManager : MonoBehaviour
{
    public CanvasGroup fadeCanvasGroup;
    public float fadeDuration = 1.2f;

    [Header("Scene Names")]
    public string upgradesSceneName = "UpgradesScene";
    public string gameSceneName = "GameScene";

    void Start()
    {
        fadeCanvasGroup.alpha = 1f; // Start fully black
        StartCoroutine(LoadUpgradeThenGame());
    }

    private IEnumerator LoadUpgradeThenGame()
    {
        Debug.Log("FadeManager: Starting loading sequence");

        yield return null;

        Debug.Log("Loading UpgradesScene additively");
        AsyncOperation loadUpgrades = SceneManager.LoadSceneAsync(upgradesSceneName, LoadSceneMode.Additive);
        yield return new WaitUntil(() => loadUpgrades.isDone);

        Debug.Log("UpgradesScene loaded");

        yield return null; // Let Awake/Start run

        Debug.Log("Unloading UpgradesScene");
        AsyncOperation unloadUpgrades = SceneManager.UnloadSceneAsync(upgradesSceneName);
        yield return new WaitUntil(() => unloadUpgrades.isDone);

        Debug.Log("Loading GameScene");
        AsyncOperation loadGame = SceneManager.LoadSceneAsync(gameSceneName);
        yield return new WaitUntil(() => loadGame.isDone);

        Debug.Log("Fading out");
        yield return StartCoroutine(FadeCanvasGroup(fadeCanvasGroup, 1f, 0f, fadeDuration));

        Debug.Log("Fade complete");
    }


    private IEnumerator FadeCanvasGroup(CanvasGroup cg, float start, float end, float duration)
    {
        float elapsed = 0f;
        cg.alpha = start;

        while (elapsed < duration)
        {
            cg.alpha = Mathf.Lerp(start, end, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        cg.alpha = end;
    }

}
