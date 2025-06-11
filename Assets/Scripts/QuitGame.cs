using UnityEngine;

public class QuitGame : MonoBehaviour
{
    public void ExitApplication()
    {
#if UNITY_EDITOR
        // If running in the Unity Editor, stop playing
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // If running in a build, quit the application
        Application.Quit();
#endif
    }
}