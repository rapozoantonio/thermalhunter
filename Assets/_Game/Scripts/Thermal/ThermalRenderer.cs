using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// Core thermal vision rendering system
/// Simulates FLIR/thermal camera aesthetic with heat-based rendering
/// </summary>
[RequireComponent(typeof(Camera))]
public class ThermalRenderer : MonoBehaviour
{
    [Header("Thermal Vision Settings")]
    [SerializeField] private Material thermalMaterial;
    [SerializeField] private Gradient heatGradient;
    [SerializeField] private float noiseIntensity = 0.02f;
    [SerializeField] private float scanLineSpeed = 2f;
    [SerializeField] private float batteryLife = 180f; // 3 minutes

    [Header("Performance")]
    [SerializeField] private RenderTextureFormat textureFormat = RenderTextureFormat.R8;
    [SerializeField] private int mobileResolution = 512;
    [SerializeField] private int pcResolution = 1024;

    private Camera thermalCamera;
    private RenderTexture thermalBuffer;
    private float currentBattery;
    private bool isActive;

    // Shader property IDs (cached for performance)
    private static readonly int HeatGradientID = Shader.PropertyToID("_HeatGradient");
    private static readonly int NoiseIntensityID = Shader.PropertyToID("_NoiseIntensity");
    private static readonly int ScanLineTimeID = Shader.PropertyToID("_ScanLineTime");
    private static readonly int BatteryLevelID = Shader.PropertyToID("_BatteryLevel");

    private void Awake()
    {
        InitializeThermalCamera();
        CreateThermalBuffer();
        ConfigureShaderProperties();
        currentBattery = batteryLife;
    }

    private void InitializeThermalCamera()
    {
        thermalCamera = GetComponent<Camera>();
        if (thermalCamera == null)
        {
            thermalCamera = gameObject.AddComponent<Camera>();
        }

        thermalCamera.enabled = false;
        thermalCamera.clearFlags = CameraClearFlags.SolidColor;
        thermalCamera.backgroundColor = Color.black;
        thermalCamera.cullingMask = LayerMask.GetMask(Constants.LAYER_THERMAL_VISIBLE);
        thermalCamera.renderingPath = RenderingPath.Forward;

        // Mobile optimization
#if UNITY_ANDROID || UNITY_IOS
        thermalCamera.allowHDR = false;
        thermalCamera.allowMSAA = false;
#endif
    }

    private void CreateThermalBuffer()
    {
        int resolution = Application.isMobilePlatform ? mobileResolution : pcResolution;

        thermalBuffer = new RenderTexture(resolution, resolution, 0, textureFormat)
        {
            name = "ThermalBuffer",
            filterMode = FilterMode.Bilinear,
            wrapMode = TextureWrapMode.Clamp,
            useMipMap = false
        };

        thermalCamera.targetTexture = thermalBuffer;
    }

    private void ConfigureShaderProperties()
    {
        if (thermalMaterial == null)
        {
            Debug.LogWarning("[ThermalRenderer] No thermal material assigned!");
            return;
        }

        // Convert gradient to texture for shader
        Texture2D gradientTexture = GenerateGradientTexture(heatGradient, 256);
        thermalMaterial.SetTexture(HeatGradientID, gradientTexture);
        thermalMaterial.SetFloat(NoiseIntensityID, noiseIntensity);
    }

    private void Update()
    {
        if (!isActive) return;

        // Battery drain
        currentBattery -= Time.deltaTime;
        if (currentBattery <= 0f)
        {
            DeactivateThermalVision();
            return;
        }

        UpdateShaderEffects();
    }

    private void UpdateShaderEffects()
    {
        if (thermalMaterial == null) return;

        // Update shader time-based effects
        thermalMaterial.SetFloat(ScanLineTimeID, Time.time * scanLineSpeed);
        thermalMaterial.SetFloat(BatteryLevelID, currentBattery / batteryLife);

        // Low battery warning (flicker effect)
        if (currentBattery < 30f)
        {
            float flicker = Mathf.PingPong(Time.time * 5f, 1f);
            thermalMaterial.SetFloat(NoiseIntensityID, noiseIntensity * (1f + flicker * 0.5f));
        }
    }

    public void ActivateThermalVision()
    {
        isActive = true;
        thermalCamera.enabled = true;
        EventBus.Publish(new ThermalActivatedEvent());

        AudioManager audioManager = ServiceLocator.Instance.TryGet<AudioManager>();
        audioManager?.Play("ThermalActivate");

#if UNITY_IOS || UNITY_ANDROID
        Handheld.Vibrate();
#endif

        Debug.Log("[ThermalRenderer] Thermal vision activated");
    }

    public void DeactivateThermalVision()
    {
        isActive = false;
        thermalCamera.enabled = false;
        EventBus.Publish(new ThermalDeactivatedEvent());

        if (currentBattery <= 0f)
        {
            Debug.LogWarning("[ThermalRenderer] Battery depleted!");
        }

        Debug.Log("[ThermalRenderer] Thermal vision deactivated");
    }

    public void RechargeBattery(float amount)
    {
        currentBattery = Mathf.Min(currentBattery + amount, batteryLife);
        Debug.Log($"[ThermalRenderer] Battery recharged: {currentBattery}/{batteryLife}");
    }

    public float GetBatteryPercentage()
    {
        return currentBattery / batteryLife;
    }

    public bool IsActive => isActive;

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (isActive && thermalMaterial != null)
        {
            Graphics.Blit(source, destination, thermalMaterial);
        }
        else
        {
            Graphics.Blit(source, destination);
        }
    }

    private Texture2D GenerateGradientTexture(Gradient gradient, int width)
    {
        Texture2D texture = new Texture2D(width, 1, TextureFormat.RGBA32, false)
        {
            wrapMode = TextureWrapMode.Clamp,
            filterMode = FilterMode.Bilinear
        };

        for (int i = 0; i < width; i++)
        {
            float t = i / (float)width;
            texture.SetPixel(i, 0, gradient.Evaluate(t));
        }

        texture.Apply();
        return texture;
    }

    private void OnDestroy()
    {
        if (thermalBuffer != null)
        {
            thermalBuffer.Release();
            Destroy(thermalBuffer);
        }
    }
}
