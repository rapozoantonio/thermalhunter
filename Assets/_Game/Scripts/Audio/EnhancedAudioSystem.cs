using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Enhanced audio system specifically for Thermal Hunt gameplay
/// Manages weapon sounds, rat vocalizations, ambient sounds, and dynamic mixing
/// </summary>
public class EnhancedAudioSystem : MonoBehaviour
{
    [System.Serializable]
    public class WeaponSounds
    {
        public AudioClip[] rifleShots;
        public AudioClip[] suppressedShots;
        public AudioClip dryFire;
        public AudioClip reload;
        public AudioClip boltAction;
        public AudioClip scopeZoom;
        public AudioClip thermalActivate;
    }

    [System.Serializable]
    public class RatSounds
    {
        public AudioClip[] squeaks;
        public AudioClip[] scurrying;
        public AudioClip[] deathSounds;
        public AudioClip[] alertSounds;
        public AudioClip[] feedingSounds;
        public AudioClip nestMotherRoar;
    }

    [System.Serializable]
    public class AmbientSounds
    {
        public AudioClip[] nightAmbience;
        public AudioClip[] windSounds;
        public AudioClip[] barnCreaks;
        public AudioClip[] industrialHums;
        public AudioClip[] distantTraffic;
        public AudioClip[] waterDrips;
    }

    [System.Serializable]
    public class ImpactSounds
    {
        public AudioClip[] bulletImpactFlesh;
        public AudioClip[] bulletImpactWood;
        public AudioClip[] bulletImpactMetal;
        public AudioClip[] bulletImpactConcrete;
        public AudioClip[] ricochet;
    }

    [Header("Sound Libraries")]
    [SerializeField] private WeaponSounds weaponSounds = new WeaponSounds();
    [SerializeField] private RatSounds ratSounds = new RatSounds();
    [SerializeField] private AmbientSounds ambientSounds = new AmbientSounds();
    [SerializeField] private ImpactSounds impactSounds = new ImpactSounds();

    [Header("Audio Sources")]
    [SerializeField] private AudioSource weaponSource;
    [SerializeField] private AudioSource ambientSource;
    [SerializeField] private AudioSource environmentSource;

    [Header("3D Audio Pool")]
    [SerializeField] private int audioSourcePoolSize = 10;
    private List<AudioSource> audioSourcePool = new List<AudioSource>();
    private int currentPoolIndex = 0;

    [Header("Audio Settings")]
    [SerializeField] private float maxAudioDistance = 50f;
    [SerializeField] private float ratVolumeRange = 0.3f;
    [SerializeField] private float ratPitchVariation = 0.15f;

    [Header("Dynamic Mixing")]
    [SerializeField] private bool enableDynamicMixing = true;
    [SerializeField] private float tensionLevel = 0f; // 0-1, affects ambient volume
    [SerializeField] private float combatIntensity = 0f; // 0-1, affects music intensity

    private AudioManager audioManager;

    private void Awake()
    {
        InitializeAudioPool();
        ServiceLocator.Instance.Register<EnhancedAudioSystem>(this);
    }

    private void Start()
    {
        audioManager = AudioManager.Instance;
        StartAmbientSounds();
    }

    private void Update()
    {
        if (enableDynamicMixing)
        {
            UpdateDynamicMixing();
        }
    }

    /// <summary>
    /// Initializes pool of 3D audio sources for spatial sounds
    /// </summary>
    private void InitializeAudioPool()
    {
        for (int i = 0; i < audioSourcePoolSize; i++)
        {
            GameObject sourceObj = new GameObject($"PooledAudioSource_{i}");
            sourceObj.transform.SetParent(transform);

            AudioSource source = sourceObj.AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.spatialBlend = 1f; // Full 3D
            source.rolloffMode = AudioRolloffMode.Linear;
            source.maxDistance = maxAudioDistance;

            audioSourcePool.Add(source);
        }

        Debug.Log($"[EnhancedAudio] Initialized audio pool with {audioSourcePoolSize} sources");
    }

    /// <summary>
    /// Gets next available audio source from pool
    /// </summary>
    private AudioSource GetPooledAudioSource()
    {
        AudioSource source = audioSourcePool[currentPoolIndex];
        currentPoolIndex = (currentPoolIndex + 1) % audioSourcePool.Count;
        return source;
    }

    // === WEAPON SOUNDS ===

    public void PlayWeaponShot(bool isSuppressed = true)
    {
        if (weaponSource == null) return;

        AudioClip[] shots = isSuppressed ? weaponSounds.suppressedShots : weaponSounds.rifleShots;
        if (shots != null && shots.Length > 0)
        {
            AudioClip clip = shots[Random.Range(0, shots.Length)];
            weaponSource.pitch = Random.Range(0.95f, 1.05f);
            weaponSource.PlayOneShot(clip);
        }
    }

    public void PlayDryFire()
    {
        if (weaponSource != null && weaponSounds.dryFire != null)
        {
            weaponSource.PlayOneShot(weaponSounds.dryFire, 0.5f);
        }
    }

    public void PlayReload()
    {
        if (weaponSource != null && weaponSounds.reload != null)
        {
            weaponSource.PlayOneShot(weaponSounds.reload, 0.7f);
        }
    }

    public void PlayBoltAction()
    {
        if (weaponSource != null && weaponSounds.boltAction != null)
        {
            weaponSource.PlayOneShot(weaponSounds.boltAction, 0.6f);
        }
    }

    public void PlayScopeZoom()
    {
        if (weaponSource != null && weaponSounds.scopeZoom != null)
        {
            weaponSource.PlayOneShot(weaponSounds.scopeZoom, 0.4f);
        }
    }

    public void PlayThermalActivate()
    {
        if (weaponSource != null && weaponSounds.thermalActivate != null)
        {
            weaponSource.PlayOneShot(weaponSounds.thermalActivate, 0.5f);
        }
    }

    // === RAT SOUNDS ===

    public void PlayRatSqueak(Vector3 position, RatType type = RatType.Drone)
    {
        if (ratSounds.squeaks == null || ratSounds.squeaks.Length == 0) return;

        AudioSource source = GetPooledAudioSource();
        source.transform.position = position;

        AudioClip clip = ratSounds.squeaks[Random.Range(0, ratSounds.squeaks.Length)];
        float volume = Random.Range(ratVolumeRange, 1f);
        float pitch = 1f + Random.Range(-ratPitchVariation, ratPitchVariation);

        // Adjust pitch based on rat type
        switch (type)
        {
            case RatType.NestMother:
                pitch *= 0.7f; // Lower pitch for larger rat
                volume *= 1.5f;
                break;
            case RatType.Alpha:
                pitch *= 1.1f; // Slightly higher pitch
                break;
        }

        source.pitch = pitch;
        source.PlayOneShot(clip, volume);
    }

    public void PlayRatScurrying(Vector3 position)
    {
        if (ratSounds.scurrying == null || ratSounds.scurrying.Length == 0) return;

        AudioSource source = GetPooledAudioSource();
        source.transform.position = position;

        AudioClip clip = ratSounds.scurrying[Random.Range(0, ratSounds.scurrying.Length)];
        source.pitch = Random.Range(0.9f, 1.1f);
        source.PlayOneShot(clip, 0.5f);
    }

    public void PlayRatDeath(Vector3 position, RatType type = RatType.Drone)
    {
        if (ratSounds.deathSounds == null || ratSounds.deathSounds.Length == 0) return;

        AudioSource source = GetPooledAudioSource();
        source.transform.position = position;

        if (type == RatType.NestMother && ratSounds.nestMotherRoar != null)
        {
            // Special death sound for Nest Mother
            source.pitch = 0.8f;
            source.PlayOneShot(ratSounds.nestMotherRoar, 1.2f);
        }
        else
        {
            AudioClip clip = ratSounds.deathSounds[Random.Range(0, ratSounds.deathSounds.Length)];
            source.pitch = Random.Range(0.8f, 1.2f);
            source.PlayOneShot(clip, 0.8f);
        }

        // Increase tension on death
        IncreaseTension(0.1f);
    }

    public void PlayRatAlert(Vector3 position)
    {
        if (ratSounds.alertSounds == null || ratSounds.alertSounds.Length == 0) return;

        AudioSource source = GetPooledAudioSource();
        source.transform.position = position;

        AudioClip clip = ratSounds.alertSounds[Random.Range(0, ratSounds.alertSounds.Length)];
        source.pitch = Random.Range(1.2f, 1.4f);
        source.PlayOneShot(clip, 0.9f);

        // Increase tension on alert
        IncreaseTension(0.05f);
    }

    public void PlayRatFeeding(Vector3 position)
    {
        if (ratSounds.feedingSounds == null || ratSounds.feedingSounds.Length == 0) return;

        AudioSource source = GetPooledAudioSource();
        source.transform.position = position;

        AudioClip clip = ratSounds.feedingSounds[Random.Range(0, ratSounds.feedingSounds.Length)];
        source.pitch = Random.Range(0.9f, 1.0f);
        source.PlayOneShot(clip, 0.4f);
    }

    // === IMPACT SOUNDS ===

    public void PlayBulletImpact(Vector3 position, SurfaceType surface)
    {
        AudioClip[] impacts = surface switch
        {
            SurfaceType.Flesh => impactSounds.bulletImpactFlesh,
            SurfaceType.Wood => impactSounds.bulletImpactWood,
            SurfaceType.Metal => impactSounds.bulletImpactMetal,
            SurfaceType.Concrete => impactSounds.bulletImpactConcrete,
            _ => impactSounds.bulletImpactConcrete
        };

        if (impacts == null || impacts.Length == 0) return;

        AudioSource source = GetPooledAudioSource();
        source.transform.position = position;

        AudioClip clip = impacts[Random.Range(0, impacts.Length)];
        source.pitch = Random.Range(0.9f, 1.1f);
        source.PlayOneShot(clip, 0.7f);

        // Chance for ricochet on hard surfaces
        if (surface == SurfaceType.Metal || surface == SurfaceType.Concrete)
        {
            if (Random.value < 0.3f && impactSounds.ricochet != null && impactSounds.ricochet.Length > 0)
            {
                AudioClip ricochetClip = impactSounds.ricochet[Random.Range(0, impactSounds.ricochet.Length)];
                source.PlayOneShot(ricochetClip, 0.5f);
            }
        }
    }

    // === AMBIENT SOUNDS ===

    private void StartAmbientSounds()
    {
        if (ambientSource == null || ambientSounds.nightAmbience == null || ambientSounds.nightAmbience.Length == 0)
            return;

        AudioClip ambience = ambientSounds.nightAmbience[Random.Range(0, ambientSounds.nightAmbience.Length)];
        ambientSource.clip = ambience;
        ambientSource.loop = true;
        ambientSource.volume = 0.3f;
        ambientSource.Play();

        Debug.Log("[EnhancedAudio] Started ambient sounds");
    }

    public void PlayEnvironmentSound(EnvironmentSoundType soundType, Vector3 position)
    {
        AudioClip[] sounds = soundType switch
        {
            EnvironmentSoundType.Wind => ambientSounds.windSounds,
            EnvironmentSoundType.BarnCreak => ambientSounds.barnCreaks,
            EnvironmentSoundType.IndustrialHum => ambientSounds.industrialHums,
            EnvironmentSoundType.DistantTraffic => ambientSounds.distantTraffic,
            EnvironmentSoundType.WaterDrip => ambientSounds.waterDrips,
            _ => null
        };

        if (sounds == null || sounds.Length == 0) return;

        AudioSource source = GetPooledAudioSource();
        source.transform.position = position;

        AudioClip clip = sounds[Random.Range(0, sounds.Length)];
        source.pitch = Random.Range(0.95f, 1.05f);
        source.PlayOneShot(clip, 0.5f);
    }

    // === DYNAMIC MIXING ===

    private void UpdateDynamicMixing()
    {
        // Gradually reduce tension over time
        tensionLevel = Mathf.Max(0f, tensionLevel - Time.deltaTime * 0.1f);
        combatIntensity = Mathf.Max(0f, combatIntensity - Time.deltaTime * 0.2f);

        // Adjust ambient volume based on tension
        if (ambientSource != null)
        {
            float targetVolume = Mathf.Lerp(0.3f, 0.1f, tensionLevel);
            ambientSource.volume = Mathf.Lerp(ambientSource.volume, targetVolume, Time.deltaTime * 2f);
        }
    }

    public void IncreaseTension(float amount)
    {
        tensionLevel = Mathf.Clamp01(tensionLevel + amount);
    }

    public void IncreaseCombatIntensity(float amount)
    {
        combatIntensity = Mathf.Clamp01(combatIntensity + amount);
    }

    // Properties
    public float TensionLevel => tensionLevel;
    public float CombatIntensity => combatIntensity;
}

// Enums
public enum RatType
{
    Drone,
    Alpha,
    NestMother
}

public enum SurfaceType
{
    Flesh,
    Wood,
    Metal,
    Concrete
}

public enum EnvironmentSoundType
{
    Wind,
    BarnCreak,
    IndustrialHum,
    DistantTraffic,
    WaterDrip
}
