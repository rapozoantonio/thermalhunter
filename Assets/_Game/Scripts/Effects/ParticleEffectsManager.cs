using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Manages particle effects for combat feedback and visual polish
/// Handles muzzle flash, blood splatter, bullet impacts, and environmental effects
/// Uses object pooling for performance optimization
/// </summary>
public class ParticleEffectsManager : MonoBehaviour
{
    [System.Serializable]
    public class EffectPool
    {
        public string effectName;
        public GameObject prefab;
        public int poolSize = 10;
        [HideInInspector] public Queue<GameObject> pool = new Queue<GameObject>();
    }

    [Header("Combat Effects")]
    [SerializeField] private GameObject muzzleFlashPrefab;
    [SerializeField] private GameObject bloodSplatterPrefab;
    [SerializeField] private GameObject bulletTracerPrefab;
    [SerializeField] private GameObject shellEjectionPrefab;

    [Header("Impact Effects")]
    [SerializeField] private GameObject fleshImpactPrefab;
    [SerializeField] private GameObject woodImpactPrefab;
    [SerializeField] private GameObject metalImpactPrefab;
    [SerializeField] private GameObject concreteImpactPrefab;
    [SerializeField] private GameObject sparksPrefab;

    [Header("Environmental Effects")]
    [SerializeField] private GameObject dustParticlesPrefab;
    [SerializeField] private GameObject smokeEffectPrefab;
    [SerializeField] private GameObject thermalDistortionPrefab;

    [Header("Heat Signature Effects")]
    [SerializeField] private GameObject heatTrailPrefab;
    [SerializeField] private GameObject bodyHeatPrefab;
    [SerializeField] private GameObject bloodHeatPrefab;

    [Header("Pool Settings")]
    [SerializeField] private List<EffectPool> effectPools = new List<EffectPool>();

    private Transform poolContainer;
    private Dictionary<string, EffectPool> poolDictionary = new Dictionary<string, EffectPool>();

    private void Awake()
    {
        InitializePools();
        ServiceLocator.Instance.Register<ParticleEffectsManager>(this);
    }

    private void Start()
    {
        SetupDefaultEffects();
    }

    /// <summary>
    /// Initialize object pools for all effects
    /// </summary>
    private void InitializePools()
    {
        poolContainer = new GameObject("EffectPools").transform;
        poolContainer.SetParent(transform);

        foreach (var pool in effectPools)
        {
            if (pool.prefab == null)
            {
                Debug.LogWarning($"[ParticleEffects] No prefab assigned for {pool.effectName}");
                continue;
            }

            pool.pool = new Queue<GameObject>();

            for (int i = 0; i < pool.poolSize; i++)
            {
                GameObject obj = Instantiate(pool.prefab, poolContainer);
                obj.name = $"{pool.effectName}_{i}";
                obj.SetActive(false);
                pool.pool.Enqueue(obj);
            }

            poolDictionary[pool.effectName] = pool;
            Debug.Log($"[ParticleEffects] Initialized pool '{pool.effectName}' with {pool.poolSize} objects");
        }
    }

    /// <summary>
    /// Setup default effect pools
    /// </summary>
    private void SetupDefaultEffects()
    {
        // Add default pools if prefabs are assigned
        if (muzzleFlashPrefab != null)
            AddEffectPool("MuzzleFlash", muzzleFlashPrefab, 5);

        if (bloodSplatterPrefab != null)
            AddEffectPool("BloodSplatter", bloodSplatterPrefab, 20);

        if (bulletTracerPrefab != null)
            AddEffectPool("BulletTracer", bulletTracerPrefab, 10);

        if (shellEjectionPrefab != null)
            AddEffectPool("ShellEjection", shellEjectionPrefab, 5);

        if (fleshImpactPrefab != null)
            AddEffectPool("FleshImpact", fleshImpactPrefab, 15);

        if (woodImpactPrefab != null)
            AddEffectPool("WoodImpact", woodImpactPrefab, 10);

        if (metalImpactPrefab != null)
            AddEffectPool("MetalImpact", metalImpactPrefab, 10);

        if (concreteImpactPrefab != null)
            AddEffectPool("ConcreteImpact", concreteImpactPrefab, 10);

        if (sparksPrefab != null)
            AddEffectPool("Sparks", sparksPrefab, 10);

        if (dustParticlesPrefab != null)
            AddEffectPool("Dust", dustParticlesPrefab, 5);

        if (smokeEffectPrefab != null)
            AddEffectPool("Smoke", smokeEffectPrefab, 5);

        if (thermalDistortionPrefab != null)
            AddEffectPool("ThermalDistortion", thermalDistortionPrefab, 8);

        if (heatTrailPrefab != null)
            AddEffectPool("HeatTrail", heatTrailPrefab, 20);

        if (bodyHeatPrefab != null)
            AddEffectPool("BodyHeat", bodyHeatPrefab, 15);

        if (bloodHeatPrefab != null)
            AddEffectPool("BloodHeat", bloodHeatPrefab, 20);
    }

    /// <summary>
    /// Add new effect pool at runtime
    /// </summary>
    private void AddEffectPool(string effectName, GameObject prefab, int poolSize)
    {
        if (poolDictionary.ContainsKey(effectName))
            return;

        EffectPool newPool = new EffectPool
        {
            effectName = effectName,
            prefab = prefab,
            poolSize = poolSize,
            pool = new Queue<GameObject>()
        };

        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(prefab, poolContainer);
            obj.name = $"{effectName}_{i}";
            obj.SetActive(false);
            newPool.pool.Enqueue(obj);
        }

        effectPools.Add(newPool);
        poolDictionary[effectName] = newPool;
    }

    /// <summary>
    /// Get effect from pool
    /// </summary>
    private GameObject GetPooledEffect(string effectName)
    {
        if (!poolDictionary.ContainsKey(effectName))
        {
            Debug.LogWarning($"[ParticleEffects] Effect pool '{effectName}' not found!");
            return null;
        }

        EffectPool pool = poolDictionary[effectName];

        if (pool.pool.Count == 0)
        {
            // Pool exhausted, create new instance
            GameObject newObj = Instantiate(pool.prefab, poolContainer);
            return newObj;
        }

        GameObject obj = pool.pool.Dequeue();
        return obj;
    }

    /// <summary>
    /// Return effect to pool
    /// </summary>
    private void ReturnToPool(string effectName, GameObject obj)
    {
        if (!poolDictionary.ContainsKey(effectName))
            return;

        obj.SetActive(false);
        obj.transform.SetParent(poolContainer);
        poolDictionary[effectName].pool.Enqueue(obj);
    }

    /// <summary>
    /// Play effect with automatic return to pool
    /// </summary>
    private void PlayEffect(string effectName, Vector3 position, Quaternion rotation, float duration = 2f)
    {
        GameObject effect = GetPooledEffect(effectName);
        if (effect == null) return;

        effect.transform.position = position;
        effect.transform.rotation = rotation;
        effect.SetActive(true);

        // Get particle system and play
        ParticleSystem ps = effect.GetComponent<ParticleSystem>();
        if (ps != null)
        {
            ps.Clear();
            ps.Play();
        }

        // Return to pool after duration
        StartCoroutine(ReturnAfterDelay(effectName, effect, duration));
    }

    private System.Collections.IEnumerator ReturnAfterDelay(string effectName, GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        ReturnToPool(effectName, obj);
    }

    // === PUBLIC EFFECT METHODS ===

    /// <summary>
    /// Play muzzle flash effect at weapon position
    /// </summary>
    public void PlayMuzzleFlash(Vector3 position, Vector3 direction)
    {
        Quaternion rotation = Quaternion.LookRotation(direction);
        PlayEffect("MuzzleFlash", position, rotation, 0.1f);
    }

    /// <summary>
    /// Play blood splatter effect on rat hit
    /// </summary>
    public void PlayBloodSplatter(Vector3 position, Vector3 direction, bool isKill = false)
    {
        Quaternion rotation = Quaternion.LookRotation(direction);
        PlayEffect("BloodSplatter", position, rotation, 2f);

        // Additional blood heat signature in thermal vision
        if (isKill)
        {
            PlayBloodHeatSignature(position, 5f);
        }
    }

    /// <summary>
    /// Play bullet tracer effect
    /// </summary>
    public void PlayBulletTracer(Vector3 startPosition, Vector3 endPosition)
    {
        GameObject tracer = GetPooledEffect("BulletTracer");
        if (tracer == null) return;

        tracer.transform.position = startPosition;
        tracer.transform.LookAt(endPosition);
        tracer.SetActive(true);

        // Animate tracer
        StartCoroutine(AnimateTracer(tracer, startPosition, endPosition));
    }

    private System.Collections.IEnumerator AnimateTracer(GameObject tracer, Vector3 start, Vector3 end)
    {
        float duration = 0.05f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            if (tracer == null) yield break;

            float t = elapsed / duration;
            tracer.transform.position = Vector3.Lerp(start, end, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        ReturnToPool("BulletTracer", tracer);
    }

    /// <summary>
    /// Play shell ejection effect
    /// </summary>
    public void PlayShellEjection(Vector3 position, Vector3 ejectionDirection)
    {
        Quaternion rotation = Quaternion.LookRotation(ejectionDirection);
        PlayEffect("ShellEjection", position, rotation, 1f);
    }

    /// <summary>
    /// Play bullet impact effect based on surface type
    /// </summary>
    public void PlayBulletImpact(Vector3 position, Vector3 normal, SurfaceType surfaceType)
    {
        Quaternion rotation = Quaternion.LookRotation(normal);

        string effectName = surfaceType switch
        {
            SurfaceType.Flesh => "FleshImpact",
            SurfaceType.Wood => "WoodImpact",
            SurfaceType.Metal => "MetalImpact",
            SurfaceType.Concrete => "ConcreteImpact",
            _ => "ConcreteImpact"
        };

        PlayEffect(effectName, position, rotation, 1.5f);

        // Add sparks for metal impacts
        if (surfaceType == SurfaceType.Metal && Random.value < 0.6f)
        {
            PlayEffect("Sparks", position, rotation, 1f);
        }
    }

    /// <summary>
    /// Play heat signature effect for living rat
    /// </summary>
    public void PlayBodyHeatSignature(Vector3 position, float duration)
    {
        GameObject heatEffect = GetPooledEffect("BodyHeat");
        if (heatEffect == null) return;

        heatEffect.transform.position = position;
        heatEffect.SetActive(true);

        ParticleSystem ps = heatEffect.GetComponent<ParticleSystem>();
        if (ps != null)
        {
            var main = ps.main;
            main.duration = duration;
            ps.Play();
        }

        StartCoroutine(ReturnAfterDelay("BodyHeat", heatEffect, duration));
    }

    /// <summary>
    /// Play blood heat signature (cooling over time)
    /// </summary>
    public void PlayBloodHeatSignature(Vector3 position, float duration)
    {
        GameObject bloodHeat = GetPooledEffect("BloodHeat");
        if (bloodHeat == null) return;

        bloodHeat.transform.position = position;
        bloodHeat.SetActive(true);

        ParticleSystem ps = bloodHeat.GetComponent<ParticleSystem>();
        if (ps != null)
        {
            var main = ps.main;
            main.duration = duration;
            ps.Play();
        }

        StartCoroutine(ReturnAfterDelay("BloodHeat", bloodHeat, duration));
    }

    /// <summary>
    /// Play heat trail behind moving rat
    /// </summary>
    public void PlayHeatTrail(Vector3 position)
    {
        PlayEffect("HeatTrail", position, Quaternion.identity, 1f);
    }

    /// <summary>
    /// Play thermal distortion effect
    /// </summary>
    public void PlayThermalDistortion(Vector3 position, float intensity = 1f)
    {
        GameObject distortion = GetPooledEffect("ThermalDistortion");
        if (distortion == null) return;

        distortion.transform.position = position;
        distortion.SetActive(true);

        ParticleSystem ps = distortion.GetComponent<ParticleSystem>();
        if (ps != null)
        {
            var main = ps.main;
            main.startSize = 0.5f * intensity;
            ps.Play();
        }

        StartCoroutine(ReturnAfterDelay("ThermalDistortion", distortion, 2f));
    }

    /// <summary>
    /// Play environmental dust effect
    /// </summary>
    public void PlayDustEffect(Vector3 position, float duration = 3f)
    {
        PlayEffect("Dust", position, Quaternion.identity, duration);
    }

    /// <summary>
    /// Play smoke effect
    /// </summary>
    public void PlaySmokeEffect(Vector3 position, float duration = 5f)
    {
        PlayEffect("Smoke", position, Quaternion.identity, duration);
    }

    /// <summary>
    /// Create procedural muzzle flash (fallback if no prefab)
    /// </summary>
    public void CreateProceduralMuzzleFlash(Vector3 position, Vector3 direction)
    {
        // Create simple light flash
        GameObject flashObj = new GameObject("MuzzleFlash_Procedural");
        flashObj.transform.position = position;

        Light flashLight = flashObj.AddComponent<Light>();
        flashLight.type = LightType.Point;
        flashLight.color = new Color(1f, 0.8f, 0.3f);
        flashLight.intensity = 3f;
        flashLight.range = 5f;

        StartCoroutine(FadeMuzzleFlash(flashObj, flashLight));
    }

    private System.Collections.IEnumerator FadeMuzzleFlash(GameObject flashObj, Light flashLight)
    {
        float duration = 0.1f;
        float elapsed = 0f;
        float startIntensity = flashLight.intensity;

        while (elapsed < duration)
        {
            flashLight.intensity = Mathf.Lerp(startIntensity, 0f, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        Destroy(flashObj);
    }
}
