using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Manages thermal emission for all objects in the scene
/// Provides realistic heat signatures with breathing, movement, and death effects
/// </summary>
[RequireComponent(typeof(Renderer))]
public class HeatSignature : MonoBehaviour
{
    [Header("Base Thermal Properties")]
    [SerializeField, Range(0f, 1f)] private float baseBodyHeat = 0.8f;
    [SerializeField] private HeatProfile heatProfile = HeatProfile.Mammal;
    [SerializeField] private bool isAlive = true;

    [Header("Heat Variation")]
    [SerializeField] private float breathingRate = 2f; // Cycles per second
    [SerializeField] private float breathingIntensity = 0.1f;
    [SerializeField] private float movementHeatBonus = 0.15f;

    [Header("Weak Points (High Heat)")]
    [SerializeField] private List<Transform> weakPoints = new List<Transform>();
    [SerializeField] private float weakPointHeatMultiplier = 1.5f;

    private Renderer[] renderers;
    private MaterialPropertyBlock propertyBlock;
    private float currentHeat;
    private float targetHeat;
    private Vector3 lastPosition;
    private float movementSpeed;

    // Shader property IDs
    private static readonly int EmissionColorID = Shader.PropertyToID("_EmissionColor");
    private static readonly int EmissionIntensityID = Shader.PropertyToID("_EmissionIntensity");

    public enum HeatProfile
    {
        Mammal,          // High heat, stable
        Environment,     // Low heat, constant
        Metal,           // Very low heat
        ActiveMachine,   // Medium heat, pulsing
        Decoy            // False heat signature
    }

    private void Awake()
    {
        renderers = GetComponentsInChildren<Renderer>();
        propertyBlock = new MaterialPropertyBlock();
        lastPosition = transform.position;

        InitializeHeatProfile();
        CreateWeakPointVisuals();
    }

    private void InitializeHeatProfile()
    {
        switch (heatProfile)
        {
            case HeatProfile.Mammal:
                baseBodyHeat = Random.Range(0.7f, 0.9f);
                breathingRate = Random.Range(1.8f, 2.2f);
                break;
            case HeatProfile.Environment:
                baseBodyHeat = 0.15f;
                breathingRate = 0f;
                break;
            case HeatProfile.Metal:
                baseBodyHeat = 0.05f;
                breathingRate = 0f;
                break;
            case HeatProfile.ActiveMachine:
                baseBodyHeat = 0.5f;
                breathingRate = 0.5f;
                break;
            case HeatProfile.Decoy:
                baseBodyHeat = 0.7f;
                breathingRate = 1.5f;
                break;
        }

        currentHeat = baseBodyHeat;
        targetHeat = baseBodyHeat;
    }

    private void CreateWeakPointVisuals()
    {
        foreach (var point in weakPoints)
        {
            if (point == null) continue;

            GameObject weakSpot = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            weakSpot.name = "WeakPoint_Visual";
            weakSpot.transform.SetParent(point);
            weakSpot.transform.localPosition = Vector3.zero;
            weakSpot.transform.localScale = Vector3.one * 0.1f;
            weakSpot.layer = LayerMask.NameToLayer(Constants.LAYER_THERMAL_VISIBLE);
            weakSpot.tag = Constants.TAG_WEAK_POINT;

            // Remove collider to avoid interference
            Destroy(weakSpot.GetComponent<Collider>());

            Renderer weakRenderer = weakSpot.GetComponent<Renderer>();
            if (weakRenderer != null)
            {
                // Create unique material instance for weak point
                Material weakMaterial = new Material(Shader.Find("Standard"));
                weakMaterial.EnableKeyword("_EMISSION");
                weakRenderer.material = weakMaterial;
            }

            // Pulsing animation
            StartCoroutine(AnimateWeakPoint(weakSpot.transform));
        }
    }

    private IEnumerator AnimateWeakPoint(Transform point)
    {
        float elapsed = 0f;
        Vector3 baseScale = point.localScale;

        while (isAlive && point != null)
        {
            float pulse = Mathf.Sin(elapsed * 3f) * 0.2f + 1f;
            point.localScale = baseScale * pulse;
            elapsed += Time.deltaTime;
            yield return null;
        }
    }

    private void Update()
    {
        if (!isAlive)
        {
            UpdateDeadHeat();
            return;
        }

        CalculateCurrentHeat();
        ApplyHeatToRenderers();
    }

    private void CalculateCurrentHeat()
    {
        // Base heat with breathing simulation
        float breathingEffect = Mathf.Sin(Time.time * breathingRate * Mathf.PI) * breathingIntensity;
        targetHeat = baseBodyHeat + breathingEffect;

        // Movement increases heat
        movementSpeed = (transform.position - lastPosition).magnitude / Time.deltaTime;
        if (movementSpeed > 0.1f)
        {
            targetHeat += movementHeatBonus * Mathf.Clamp01(movementSpeed / 5f);
        }

        lastPosition = transform.position;

        // Smooth interpolation
        currentHeat = Mathf.Lerp(currentHeat, targetHeat, Time.deltaTime * 2f);
    }

    private void ApplyHeatToRenderers()
    {
        Color heatColor = GetHeatColor(currentHeat);
        float intensity = currentHeat * 2f;

        foreach (var renderer in renderers)
        {
            if (renderer == null) continue;

            renderer.GetPropertyBlock(propertyBlock);
            propertyBlock.SetColor(EmissionColorID, heatColor);
            propertyBlock.SetFloat(EmissionIntensityID, intensity);
            renderer.SetPropertyBlock(propertyBlock);
        }
    }

    private void UpdateDeadHeat()
    {
        // Exponential cooling after death
        float coolingRate = 0.3f;
        currentHeat = Mathf.Lerp(currentHeat, 0.1f, Time.deltaTime * coolingRate);
        ApplyHeatToRenderers();

        // Stop updating when fully cold
        if (currentHeat < 0.15f)
        {
            enabled = false;
        }
    }

    public void OnDeath()
    {
        isAlive = false;

        // Stop weak point animations
        StopAllCoroutines();

        // Hide weak points
        foreach (var point in weakPoints)
        {
            if (point != null)
            {
                point.gameObject.SetActive(false);
            }
        }

        Debug.Log($"[HeatSignature] {gameObject.name} died - cooling down");
    }

    public void OnHit(Vector3 hitPoint, bool isWeakPoint)
    {
        // Temporary heat spike from impact
        StartCoroutine(HeatSpike(0.3f, 0.5f));

        // Spawn heat particles (blood splash)
        SpawnHeatParticles(hitPoint, isWeakPoint);
    }

    private IEnumerator HeatSpike(float intensity, float duration)
    {
        float elapsed = 0f;
        float originalHeat = currentHeat;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            currentHeat = originalHeat + intensity * (1f - t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        currentHeat = originalHeat;
    }

    private void SpawnHeatParticles(Vector3 position, bool isWeakPoint)
    {
        ObjectPooler pooler = ServiceLocator.Instance.TryGet<ObjectPooler>();
        if (pooler == null) return;

        GameObject particleObj = pooler.Spawn("HeatParticles", position, Quaternion.identity);
        if (particleObj == null) return;

        ParticleSystem ps = particleObj.GetComponent<ParticleSystem>();
        if (ps != null)
        {
            var main = ps.main;
            main.startColor = isWeakPoint ? Color.yellow : Color.white;
            main.startSpeed = isWeakPoint ? 3f : 2f;

            ps.Play();
        }
    }

    private Color GetHeatColor(float heat)
    {
        // Thermal gradient: Black -> Gray -> White -> Yellow/Red
        if (heat < 0.3f)
            return Color.Lerp(Color.black, Color.gray, heat / 0.3f);
        else if (heat < 0.7f)
            return Color.Lerp(Color.gray, Color.white, (heat - 0.3f) / 0.4f);
        else
            return Color.Lerp(Color.white, new Color(1f, 0.9f, 0.3f), (heat - 0.7f) / 0.3f);
    }

    public bool IsAlive => isAlive;
    public float CurrentHeat => currentHeat;
}
