using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Procedural audio generator for MVP - creates basic sounds at runtime
/// Use this until real audio assets are imported
/// </summary>
public class ProceduralAudioGenerator : MonoBehaviour
{
    private static ProceduralAudioGenerator instance;
    public static ProceduralAudioGenerator Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject go = new GameObject("ProceduralAudioGenerator");
                instance = go.AddComponent<ProceduralAudioGenerator>();
                DontDestroyOnLoad(go);
            }
            return instance;
        }
    }

    // Cache for generated audio clips
    private Dictionary<string, AudioClip> generatedClips = new Dictionary<string, AudioClip>();

    /// <summary>
    /// Generate a simple audio clip procedurally
    /// </summary>
    public AudioClip GenerateSound(string soundName, float duration, float frequency, AudioWaveform waveform = AudioWaveform.Sine)
    {
        // Check cache first
        if (generatedClips.ContainsKey(soundName))
        {
            return generatedClips[soundName];
        }

        int sampleRate = 44100;
        int sampleCount = Mathf.RoundToInt(duration * sampleRate);

        AudioClip clip = AudioClip.Create(soundName, sampleCount, 1, sampleRate, false);
        float[] samples = new float[sampleCount];

        for (int i = 0; i < sampleCount; i++)
        {
            float t = (float)i / sampleRate;
            samples[i] = GenerateWaveform(waveform, frequency, t) * GetEnvelope(t, duration);
        }

        clip.SetData(samples, 0);
        generatedClips[soundName] = clip;

        return clip;
    }

    /// <summary>
    /// Generate weapon shot sound
    /// </summary>
    public AudioClip GenerateWeaponShot()
    {
        return GenerateSound("WeaponShot", 0.3f, 150f, AudioWaveform.Noise);
    }

    /// <summary>
    /// Generate rat squeak sound
    /// </summary>
    public AudioClip GenerateRatSqueak()
    {
        return GenerateSound("RatSqueak", 0.15f, 2000f, AudioWaveform.Square);
    }

    /// <summary>
    /// Generate impact sound
    /// </summary>
    public AudioClip GenerateImpact(ImpactType type)
    {
        switch (type)
        {
            case ImpactType.Flesh:
                return GenerateSound("ImpactFlesh", 0.1f, 100f, AudioWaveform.Noise);
            case ImpactType.Wood:
                return GenerateSound("ImpactWood", 0.15f, 300f, AudioWaveform.Noise);
            case ImpactType.Metal:
                return GenerateSound("ImpactMetal", 0.2f, 500f, AudioWaveform.Triangle);
            case ImpactType.Concrete:
                return GenerateSound("ImpactConcrete", 0.12f, 200f, AudioWaveform.Noise);
            default:
                return GenerateSound("ImpactDefault", 0.1f, 150f, AudioWaveform.Noise);
        }
    }

    /// <summary>
    /// Generate UI click sound
    /// </summary>
    public AudioClip GenerateUIClick()
    {
        return GenerateSound("UIClick", 0.05f, 800f, AudioWaveform.Sine);
    }

    /// <summary>
    /// Generate ambient wind sound
    /// </summary>
    public AudioClip GenerateAmbientWind()
    {
        return GenerateSound("AmbientWind", 5f, 100f, AudioWaveform.Noise);
    }

    private float GenerateWaveform(AudioWaveform waveform, float frequency, float time)
    {
        float phase = 2f * Mathf.PI * frequency * time;

        switch (waveform)
        {
            case AudioWaveform.Sine:
                return Mathf.Sin(phase);

            case AudioWaveform.Square:
                return Mathf.Sign(Mathf.Sin(phase));

            case AudioWaveform.Triangle:
                return 2f * Mathf.Abs(2f * (phase / (2f * Mathf.PI) - Mathf.Floor(phase / (2f * Mathf.PI) + 0.5f))) - 1f;

            case AudioWaveform.Sawtooth:
                return 2f * (phase / (2f * Mathf.PI) - Mathf.Floor(phase / (2f * Mathf.PI) + 0.5f));

            case AudioWaveform.Noise:
                // White noise
                return Random.Range(-1f, 1f);

            default:
                return 0f;
        }
    }

    private float GetEnvelope(float time, float duration)
    {
        // Simple ADSR envelope
        float attack = 0.01f;
        float decay = 0.1f;
        float sustain = 0.7f;
        float release = duration * 0.3f;

        if (time < attack)
        {
            // Attack
            return time / attack;
        }
        else if (time < attack + decay)
        {
            // Decay
            float decayProgress = (time - attack) / decay;
            return 1f - (1f - sustain) * decayProgress;
        }
        else if (time < duration - release)
        {
            // Sustain
            return sustain;
        }
        else
        {
            // Release
            float releaseProgress = (time - (duration - release)) / release;
            return sustain * (1f - releaseProgress);
        }
    }

    public enum AudioWaveform
    {
        Sine,
        Square,
        Triangle,
        Sawtooth,
        Noise
    }

    public enum ImpactType
    {
        Flesh,
        Wood,
        Metal,
        Concrete
    }
}
