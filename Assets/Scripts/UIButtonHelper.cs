using UnityEngine;
using UnityEngine.UI;

public class UIButtonHelper : MonoBehaviour
{
    [SerializeField] private string soundName = "ButtonClick"; // Default sound
    [SerializeField] private bool debugLogs = true; // Toggle debug messages

    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();

        if (debugLogs)
        {
            Debug.Log($"UIButtonHelper: Initializing for button '{gameObject.name}'");
        }

        // Add sound to button click
        button.onClick.AddListener(PlayButtonSound);

        if (debugLogs && button == null)
        {
            Debug.LogError($"UIButtonHelper: No Button component found on {gameObject.name}!");
        }
    }

    private void PlayButtonSound()
    {
        if (debugLogs)
        {
            Debug.Log($"UIButtonHelper: Button '{gameObject.name}' clicked, attempting to play sound '{soundName}'");
        }

        if (AudioManager.Instance != null)
        {
            if (debugLogs)
            {
                Debug.Log($"UIButtonHelper: AudioManager instance found, playing sound '{soundName}'");
            }

            AudioManager.Instance.PlayUISFX(soundName);

            if (debugLogs)
            {
                Debug.Log($"UIButtonHelper: Sound '{soundName}' should now be playing");
            }
        }
        else
        {
            Debug.LogWarning($"UIButtonHelper: AudioManager instance not found on button '{gameObject.name}'!");
        }
    }

    private void OnDestroy()
    {
        if (debugLogs)
        {
            Debug.Log($"UIButtonHelper: Cleaning up button listener for '{gameObject.name}'");
        }

        // Always remember to remove listeners when destroyed
        if (button != null)
        {
            button.onClick.RemoveListener(PlayButtonSound);
        }
    }
}