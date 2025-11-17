using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Centralized audio management system
/// Handles music, SFX, and spatial audio
/// </summary>
public class AudioManager : Singleton<AudioManager>
{
    [System.Serializable]
    public class Sound
    {
        public string name;
        public AudioClip clip;
        [Range(0f, 1f)] public float volume = 1f;
        [Range(0.1f, 3f)] public float pitch = 1f;
        public bool loop = false;
        public bool is3D = false;

        [HideInInspector] public AudioSource source;
    }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Sound Library")]
    [SerializeField] private List<Sound> sounds = new List<Sound>();

    [Header("Volume Settings")]
    [SerializeField, Range(0f, 1f)] private float masterVolume = 1f;
    [SerializeField, Range(0f, 1f)] private float musicVolume = 0.7f;
    [SerializeField, Range(0f, 1f)] private float sfxVolume = 1f;

    private Dictionary<string, Sound> soundDictionary;

    protected override void Awake()
    {
        base.Awake();
        InitializeAudioSources();
        BuildSoundDictionary();
        LoadVolumeSettings();

        // Register with ServiceLocator
        ServiceLocator.Instance.Register<AudioManager>(this);
    }

    private void InitializeAudioSources()
    {
        if (musicSource == null)
        {
            GameObject musicObj = new GameObject("MusicSource");
            musicObj.transform.SetParent(transform);
            musicSource = musicObj.AddComponent<AudioSource>();
            musicSource.loop = true;
            musicSource.playOnAwake = false;
        }

        if (sfxSource == null)
        {
            GameObject sfxObj = new GameObject("SFXSource");
            sfxObj.transform.SetParent(transform);
            sfxSource = sfxObj.AddComponent<AudioSource>();
            sfxSource.playOnAwake = false;
        }
    }

    private void BuildSoundDictionary()
    {
        soundDictionary = new Dictionary<string, Sound>();

        foreach (Sound sound in sounds)
        {
            if (sound.clip == null)
            {
                Debug.LogWarning($"[AudioManager] Sound '{sound.name}' has no clip assigned!");
                continue;
            }

            if (soundDictionary.ContainsKey(sound.name))
            {
                Debug.LogWarning($"[AudioManager] Duplicate sound name '{sound.name}'!");
                continue;
            }

            // Create audio source for this sound
            GameObject soundObj = new GameObject($"Sound_{sound.name}");
            soundObj.transform.SetParent(transform);

            sound.source = soundObj.AddComponent<AudioSource>();
            sound.source.clip = sound.clip;
            sound.source.volume = sound.volume;
            sound.source.pitch = sound.pitch;
            sound.source.loop = sound.loop;
            sound.source.playOnAwake = false;

            if (sound.is3D)
            {
                sound.source.spatialBlend = 1f;
                sound.source.rolloffMode = AudioRolloffMode.Linear;
                sound.source.maxDistance = 50f;
            }

            soundDictionary.Add(sound.name, sound);
        }

        Debug.Log($"[AudioManager] Initialized {soundDictionary.Count} sounds");
    }

    private void LoadVolumeSettings()
    {
        SaveManager saveManager = SaveManager.Instance;
        if (saveManager != null && saveManager.CurrentSave != null)
        {
            masterVolume = saveManager.CurrentSave.masterVolume;
            musicVolume = saveManager.CurrentSave.musicVolume;
            sfxVolume = saveManager.CurrentSave.sfxVolume;
        }

        UpdateVolumes();
    }

    // === Play Methods ===

    public void Play(string soundName)
    {
        if (!soundDictionary.ContainsKey(soundName))
        {
            Debug.LogWarning($"[AudioManager] Sound '{soundName}' not found!");
            return;
        }

        Sound sound = soundDictionary[soundName];
        if (sound.source != null)
        {
            sound.source.volume = sound.volume * sfxVolume * masterVolume;
            sound.source.Play();
        }
    }

    public void PlayOneShot(string soundName)
    {
        if (!soundDictionary.ContainsKey(soundName))
        {
            Debug.LogWarning($"[AudioManager] Sound '{soundName}' not found!");
            return;
        }

        Sound sound = soundDictionary[soundName];
        sfxSource.PlayOneShot(sound.clip, sound.volume * sfxVolume * masterVolume);
    }

    public void PlayAtPoint(string soundName, Vector3 position)
    {
        if (!soundDictionary.ContainsKey(soundName))
        {
            Debug.LogWarning($"[AudioManager] Sound '{soundName}' not found!");
            return;
        }

        Sound sound = soundDictionary[soundName];
        AudioSource.PlayClipAtPoint(sound.clip, position, sound.volume * sfxVolume * masterVolume);
    }

    public void Stop(string soundName)
    {
        if (!soundDictionary.ContainsKey(soundName))
        {
            Debug.LogWarning($"[AudioManager] Sound '{soundName}' not found!");
            return;
        }

        Sound sound = soundDictionary[soundName];
        if (sound.source != null && sound.source.isPlaying)
        {
            sound.source.Stop();
        }
    }

    public bool IsPlaying(string soundName)
    {
        if (!soundDictionary.ContainsKey(soundName))
        {
            return false;
        }

        Sound sound = soundDictionary[soundName];
        return sound.source != null && sound.source.isPlaying;
    }

    // === Music Methods ===

    public void PlayMusic(string soundName, bool fadeIn = true)
    {
        if (!soundDictionary.ContainsKey(soundName))
        {
            Debug.LogWarning($"[AudioManager] Music '{soundName}' not found!");
            return;
        }

        Sound music = soundDictionary[soundName];

        if (musicSource.isPlaying)
        {
            if (fadeIn)
            {
                StartCoroutine(FadeOutAndPlayMusic(music));
            }
            else
            {
                musicSource.Stop();
                PlayMusicImmediate(music);
            }
        }
        else
        {
            if (fadeIn)
            {
                StartCoroutine(FadeInMusic(music));
            }
            else
            {
                PlayMusicImmediate(music);
            }
        }
    }

    private void PlayMusicImmediate(Sound music)
    {
        musicSource.clip = music.clip;
        musicSource.volume = music.volume * musicVolume * masterVolume;
        musicSource.loop = true;
        musicSource.Play();
    }

    private System.Collections.IEnumerator FadeInMusic(Sound music)
    {
        musicSource.clip = music.clip;
        musicSource.volume = 0f;
        musicSource.loop = true;
        musicSource.Play();

        float targetVolume = music.volume * musicVolume * masterVolume;
        float elapsed = 0f;
        float duration = 1f;

        while (elapsed < duration)
        {
            musicSource.volume = Mathf.Lerp(0f, targetVolume, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        musicSource.volume = targetVolume;
    }

    private System.Collections.IEnumerator FadeOutAndPlayMusic(Sound newMusic)
    {
        float startVolume = musicSource.volume;
        float elapsed = 0f;
        float duration = 0.5f;

        while (elapsed < duration)
        {
            musicSource.volume = Mathf.Lerp(startVolume, 0f, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        musicSource.Stop();
        yield return StartCoroutine(FadeInMusic(newMusic));
    }

    public void StopMusic(bool fadeOut = true)
    {
        if (fadeOut)
        {
            StartCoroutine(FadeOutMusic());
        }
        else
        {
            musicSource.Stop();
        }
    }

    private System.Collections.IEnumerator FadeOutMusic()
    {
        float startVolume = musicSource.volume;
        float elapsed = 0f;
        float duration = 1f;

        while (elapsed < duration)
        {
            musicSource.volume = Mathf.Lerp(startVolume, 0f, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        musicSource.Stop();
        musicSource.volume = startVolume;
    }

    // === Volume Control ===

    public void SetMasterVolume(float volume)
    {
        masterVolume = Mathf.Clamp01(volume);
        UpdateVolumes();
        SaveManager.Instance?.SetVolume("master", masterVolume);
    }

    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        UpdateVolumes();
        SaveManager.Instance?.SetVolume("music", musicVolume);
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        UpdateVolumes();
        SaveManager.Instance?.SetVolume("sfx", sfxVolume);
    }

    private void UpdateVolumes()
    {
        if (musicSource != null)
        {
            musicSource.volume = musicVolume * masterVolume;
        }

        foreach (var sound in soundDictionary.Values)
        {
            if (sound.source != null)
            {
                sound.source.volume = sound.volume * sfxVolume * masterVolume;
            }
        }
    }

    public float MasterVolume => masterVolume;
    public float MusicVolume => musicVolume;
    public float SFXVolume => sfxVolume;
}
