using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class BGMSceneRouter : MonoBehaviour
{
    [System.Serializable]
    public class SceneBGM
    {
        public string sceneName;
        public string bgmClipName;
    }

    [SerializeField]
    private List<SceneBGM> sceneBGMList = new List<SceneBGM>();

    private Dictionary<string, string> sceneToBGM = new Dictionary<string, string>();
    private string currentBGMClip = "";

    private void Awake()
    {
        foreach (var item in sceneBGMList)
        {
            if (!sceneToBGM.ContainsKey(item.sceneName))
                sceneToBGM.Add(item.sceneName, item.bgmClipName);
        }

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (sceneToBGM.TryGetValue(scene.name, out string newBGMName))
        {
            if (currentBGMClip != newBGMName)
            {
                AudioManager.Instance.PlayBGM(newBGMName);
                AudioManager.Instance.SetBGMVolume(0.4f); // Ensure volume is set
                currentBGMClip = newBGMName;
            }
        }
        else
        {
            AudioManager.Instance.StopBGM();
            currentBGMClip = "";
        }
    }
}
