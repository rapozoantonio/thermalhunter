using UnityEngine;

/// <summary>
/// ScriptableObject for thermal scope configuration
/// Contains zoom, FOV, battery, and visual settings
/// </summary>
[CreateAssetMenu(fileName = "NewScope", menuName = "ThermalHunt/Scope Data")]
public class ScopeData : ScriptableObject
{
    [Header("Scope Info")]
    public string scopeID;
    public string scopeName;
    [TextArea(2, 4)] public string description;
    public Sprite icon;
    public GameObject scopePrefab;

    [Header("Optics")]
    public float magnification = 4f;
    public float minZoom = 1f;
    public float maxZoom = 4f;
    public float fieldOfView = 45f;

    [Header("Battery")]
    public float batteryLife = 180f; // Seconds
    public float batteryDrainRate = 1f;
    public bool hasUnlimitedBattery = false;

    [Header("Thermal Settings")]
    public Gradient heatGradient;
    public float thermalQuality = 1f;
    public float thermalResolution = 512f;
    public float noiseIntensity = 0.02f;
    public bool hasScanLines = true;
    public float batteryDrainMultiplier = 1f;

    [Header("Reticle")]
    public Sprite reticleSprite;
    public Color reticleColor = Color.white;
    public float reticleSize = 16f;

    [Header("Performance")]
    public int renderResolution = 512;
    public bool allowDynamicResolution = true;

    [Header("Unlock Requirements")]
    public int requiredLevel = 1;
    public bool isPremium = false;
    public float price = 0f; // For IAP

    // Validation
    private void OnValidate()
    {
        if (string.IsNullOrEmpty(scopeID))
        {
            scopeID = name;
        }

        minZoom = Mathf.Max(1f, minZoom);
        maxZoom = Mathf.Max(minZoom, maxZoom);
    }
}
