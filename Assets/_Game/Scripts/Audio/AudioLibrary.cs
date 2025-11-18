using UnityEngine;

/// <summary>
/// Audio library manager - provides access to all game sounds
/// For MVP: Uses procedural audio, can be replaced with real assets later
/// </summary>
public class AudioLibrary : MonoBehaviour
{
    private static AudioLibrary instance;
    public static AudioLibrary Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject go = new GameObject("AudioLibrary");
                instance = go.AddComponent<AudioLibrary>();
                DontDestroyOnLoad(go);
                instance.Initialize();
            }
            return instance;
        }
    }

    [Header("Audio Clips - Assign real assets here when available")]
    // Weapon Sounds
    public AudioClip weaponShot;
    public AudioClip weaponReload;
    public AudioClip weaponDryFire;
    public AudioClip scopeZoom;

    // Rat Sounds
    public AudioClip ratSqueak;
    public AudioClip ratScurry;
    public AudioClip ratDeath;
    public AudioClip ratAlert;

    // Impact Sounds
    public AudioClip impactFlesh;
    public AudioClip impactWood;
    public AudioClip impactMetal;
    public AudioClip impactConcrete;
    public AudioClip ricochet;

    // UI Sounds
    public AudioClip uiClick;
    public AudioClip uiHover;
    public AudioClip missionComplete;
    public AudioClip missionFailed;

    // Ambient Sounds
    public AudioClip ambientNight;
    public AudioClip ambientWind;
    public AudioClip ambientBarn;

    private bool useProceduralAudio = false;

    private void Initialize()
    {
        // Check if any clips are assigned
        useProceduralAudio = (weaponShot == null);

        if (useProceduralAudio)
        {
            Debug.Log("[AudioLibrary] No audio assets found. Using procedural audio for MVP.");
            GenerateProceduralSounds();
        }
        else
        {
            Debug.Log("[AudioLibrary] Audio assets loaded successfully.");
        }
    }

    private void GenerateProceduralSounds()
    {
        var generator = ProceduralAudioGenerator.Instance;

        // Generate weapon sounds
        weaponShot = generator.GenerateWeaponShot();
        weaponDryFire = generator.GenerateSound("DryFire", 0.1f, 400f, ProceduralAudioGenerator.AudioWaveform.Square);
        weaponReload = generator.GenerateSound("Reload", 0.5f, 200f, ProceduralAudioGenerator.AudioWaveform.Noise);
        scopeZoom = generator.GenerateSound("ScopeZoom", 0.2f, 600f, ProceduralAudioGenerator.AudioWaveform.Sine);

        // Generate rat sounds
        ratSqueak = generator.GenerateRatSqueak();
        ratScurry = generator.GenerateSound("RatScurry", 0.3f, 50f, ProceduralAudioGenerator.AudioWaveform.Noise);
        ratDeath = generator.GenerateSound("RatDeath", 0.4f, 1500f, ProceduralAudioGenerator.AudioWaveform.Square);
        ratAlert = generator.GenerateSound("RatAlert", 0.2f, 2500f, ProceduralAudioGenerator.AudioWaveform.Square);

        // Generate impact sounds
        impactFlesh = generator.GenerateImpact(ProceduralAudioGenerator.ImpactType.Flesh);
        impactWood = generator.GenerateImpact(ProceduralAudioGenerator.ImpactType.Wood);
        impactMetal = generator.GenerateImpact(ProceduralAudioGenerator.ImpactType.Metal);
        impactConcrete = generator.GenerateImpact(ProceduralAudioGenerator.ImpactType.Concrete);
        ricochet = generator.GenerateSound("Ricochet", 0.3f, 1000f, ProceduralAudioGenerator.AudioWaveform.Triangle);

        // Generate UI sounds
        uiClick = generator.GenerateUIClick();
        uiHover = generator.GenerateSound("UIHover", 0.03f, 600f, ProceduralAudioGenerator.AudioWaveform.Sine);
        missionComplete = generator.GenerateSound("MissionComplete", 1f, 440f, ProceduralAudioGenerator.AudioWaveform.Sine);
        missionFailed = generator.GenerateSound("MissionFailed", 1f, 220f, ProceduralAudioGenerator.AudioWaveform.Sine);

        // Generate ambient sounds
        ambientNight = generator.GenerateAmbientWind();
        ambientWind = generator.GenerateSound("Wind", 10f, 80f, ProceduralAudioGenerator.AudioWaveform.Noise);
        ambientBarn = generator.GenerateSound("Barn", 8f, 60f, ProceduralAudioGenerator.AudioWaveform.Noise);
    }

    // Convenience methods for accessing sounds
    public AudioClip GetWeaponSound(string soundType)
    {
        switch (soundType.ToLower())
        {
            case "shot": return weaponShot;
            case "reload": return weaponReload;
            case "dryfire": return weaponDryFire;
            case "scope": return scopeZoom;
            default: return weaponShot;
        }
    }

    public AudioClip GetRatSound(string soundType)
    {
        switch (soundType.ToLower())
        {
            case "squeak": return ratSqueak;
            case "scurry": return ratScurry;
            case "death": return ratDeath;
            case "alert": return ratAlert;
            default: return ratSqueak;
        }
    }

    public AudioClip GetImpactSound(string surfaceType)
    {
        switch (surfaceType.ToLower())
        {
            case "flesh": return impactFlesh;
            case "wood": return impactWood;
            case "metal": return impactMetal;
            case "concrete": return impactConcrete;
            case "ricochet": return ricochet;
            default: return impactWood;
        }
    }

    public AudioClip GetUISound(string soundType)
    {
        switch (soundType.ToLower())
        {
            case "click": return uiClick;
            case "hover": return uiHover;
            case "complete": return missionComplete;
            case "failed": return missionFailed;
            default: return uiClick;
        }
    }

    public AudioClip GetAmbientSound(string soundType)
    {
        switch (soundType.ToLower())
        {
            case "night": return ambientNight;
            case "wind": return ambientWind;
            case "barn": return ambientBarn;
            default: return ambientNight;
        }
    }
}
