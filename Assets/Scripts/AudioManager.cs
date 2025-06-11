using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class AudioClipData
{
    public string name;
    public AudioClip clip;
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource bgmSource; // 2D music
    [SerializeField] private AudioSource sfxSource; // 3D spatial sound effects
    [SerializeField] private AudioSource uiSfxSource; // 2D UI sound effects

    [Header("Audio Clips")]
    [SerializeField] private List<AudioClipData> bgmClips = new List<AudioClipData>();
    [SerializeField] private List<AudioClipData> sfxClips = new List<AudioClipData>();
    [SerializeField] private List<AudioClipData> uiSfxClips = new List<AudioClipData>();

    [Header("Volume Settings")]
    [Range(0f, 1f)] public float masterVolume = 1f;
    [Range(0f, 1f)] public float bgmVolume = 1f;
    [Range(0f, 1f)] public float sfxVolume = 1f;
    [Range(0f, 1f)] public float uiSfxVolume = 1f;

    private Dictionary<string, AudioClip> bgmDictionary = new Dictionary<string, AudioClip>();
    private Dictionary<string, AudioClip> sfxDictionary = new Dictionary<string, AudioClip>();
    private Dictionary<string, AudioClip> uiSfxDictionary = new Dictionary<string, AudioClip>();

    public AudioSource BGMSource
    {
        get { return bgmSource; }
    }

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Initialize audio sources
        if (bgmSource != null)
        {
            bgmSource.loop = true;
            bgmSource.spatialBlend = 0f; // 2D audio
        }

        if (sfxSource != null)
        {
            sfxSource.spatialBlend = 1f; // 3D audio
        }

        if (uiSfxSource != null)
        {
            uiSfxSource.spatialBlend = 0f; // 2D audio
        }

        // Build audio clip dictionaries
        BuildAudioDictionaries();

        UpdateVolumes();
    }

    private void BuildAudioDictionaries()
    {
        bgmDictionary.Clear();
        foreach (var clipData in bgmClips)
        {
            if (!bgmDictionary.ContainsKey(clipData.name))
            {
                bgmDictionary.Add(clipData.name, clipData.clip);
            }
        }

        sfxDictionary.Clear();
        foreach (var clipData in sfxClips)
        {
            if (!sfxDictionary.ContainsKey(clipData.name))
            {
                sfxDictionary.Add(clipData.name, clipData.clip);
            }
        }

        uiSfxDictionary.Clear();
        foreach (var clipData in uiSfxClips)
        {
            if (!uiSfxDictionary.ContainsKey(clipData.name))
            {
                uiSfxDictionary.Add(clipData.name, clipData.clip);
            }
        }
    }

    private void UpdateVolumes()
    {
        if (bgmSource != null) bgmSource.volume = bgmVolume * masterVolume;
        if (sfxSource != null) sfxSource.volume = sfxVolume * masterVolume;
        if (uiSfxSource != null) uiSfxSource.volume = uiSfxVolume * masterVolume;
    }

    // BGM Methods
    public void PlayBGM(string clipName)
    {
        if (bgmSource == null || !bgmDictionary.ContainsKey(clipName)) return;

        bgmSource.clip = bgmDictionary[clipName];
        bgmSource.Play();
    }

    public void StopBGM()
    {
        if (bgmSource == null) return;
        bgmSource.Stop();
    }

    public void PauseBGM()
    {
        if (bgmSource == null) return;
        bgmSource.Pause();
    }

    public void ResumeBGM()
    {
        if (bgmSource == null) return;
        bgmSource.UnPause();
    }

    // 3D SFX Methods
    public void PlaySFX(string clipName, Vector3 position)
    {
        if (sfxSource == null || !sfxDictionary.ContainsKey(clipName)) return;

        sfxSource.transform.position = position;
        sfxSource.PlayOneShot(sfxDictionary[clipName]);
    }

    // 2D UI SFX Methods
    public void PlayUISFX(string clipName)
    {
        if (uiSfxSource == null || !uiSfxDictionary.ContainsKey(clipName)) return;

        uiSfxSource.PlayOneShot(uiSfxDictionary[clipName]);
    }

    // Volume Control Methods
    public void SetMasterVolume(float volume)
    {
        masterVolume = Mathf.Clamp01(volume);
        UpdateVolumes();
    }

    public void SetBGMVolume(float volume)
    {
        bgmVolume = Mathf.Clamp01(volume);
        UpdateVolumes();
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        UpdateVolumes();
    }

    public void SetUISFXVolume(float volume)
    {
        uiSfxVolume = Mathf.Clamp01(volume);
        UpdateVolumes();
    }

    // Editor-only method to refresh dictionaries
#if UNITY_EDITOR
    public void RefreshDictionaries()
    {
        BuildAudioDictionaries();
    }
#endif
}