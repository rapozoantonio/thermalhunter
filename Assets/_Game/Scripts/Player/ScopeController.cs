using UnityEngine;

/// <summary>
/// Controls thermal scope functionality and zoom levels
/// Manages scope overlay, reticle, and thermal vision intensity
/// </summary>
public class ScopeController : MonoBehaviour
{
    [Header("Scope Configuration")]
    [SerializeField] private ScopeData currentScope;

    [Header("Scope Visual")]
    [SerializeField] private GameObject scopeOverlay;
    [SerializeField] private GameObject reticle;
    [SerializeField] private float overlayFadeSpeed = 5f;

    [Header("Zoom Levels")]
    [SerializeField] private int currentZoomLevel = 0;
    [SerializeField] private float[] zoomLevels = new float[] { 1f, 2f, 4f, 8f };

    private PlayerController playerController;
    private ThermalRenderer thermalRenderer;
    private CanvasGroup overlayCanvasGroup;
    private GameObject scopeModel;
    private bool isActive;

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
        CreateScopeUI();
    }

    private void Start()
    {
        // Get thermal renderer from camera
        if (playerController != null && playerController.PlayerCamera != null)
        {
            thermalRenderer = playerController.PlayerCamera.GetComponent<ThermalRenderer>();
        }

        // Hide scope overlay initially
        if (overlayCanvasGroup != null)
        {
            overlayCanvasGroup.alpha = 0f;
        }
    }

    private void CreateScopeUI()
    {
        // Create Canvas for scope overlay if it doesn't exist
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasObj = new GameObject("ScopeCanvas");
            canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObj.AddComponent<UnityEngine.UI.CanvasScaler>();
            canvasObj.AddComponent<UnityEngine.UI.GraphicRaycaster>();
        }

        // Create scope overlay
        if (scopeOverlay == null)
        {
            scopeOverlay = new GameObject("ScopeOverlay");
            scopeOverlay.transform.SetParent(canvas.transform, false);

            RectTransform rect = scopeOverlay.AddComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.sizeDelta = Vector2.zero;

            // Add semi-transparent black background
            UnityEngine.UI.Image image = scopeOverlay.AddComponent<UnityEngine.UI.Image>();
            image.color = new Color(0, 0, 0, 0.8f);

            overlayCanvasGroup = scopeOverlay.AddComponent<CanvasGroup>();
            overlayCanvasGroup.alpha = 0f;
            overlayCanvasGroup.blocksRaycasts = false;
        }

        // Create reticle
        if (reticle == null)
        {
            reticle = new GameObject("Reticle");
            reticle.transform.SetParent(scopeOverlay.transform, false);

            RectTransform reticleRect = reticle.AddComponent<RectTransform>();
            reticleRect.anchorMin = new Vector2(0.5f, 0.5f);
            reticleRect.anchorMax = new Vector2(0.5f, 0.5f);
            reticleRect.sizeDelta = new Vector2(40, 40);

            UnityEngine.UI.Image reticleImage = reticle.AddComponent<UnityEngine.UI.Image>();
            reticleImage.color = Color.green;

            // Create crosshair using simple lines
            CreateCrosshair(reticle.transform);
        }
    }

    private void CreateCrosshair(Transform parent)
    {
        // Vertical line
        GameObject vLine = new GameObject("VerticalLine");
        vLine.transform.SetParent(parent, false);
        RectTransform vRect = vLine.AddComponent<RectTransform>();
        vRect.sizeDelta = new Vector2(2, 30);
        UnityEngine.UI.Image vImage = vLine.AddComponent<UnityEngine.UI.Image>();
        vImage.color = Color.green;

        // Horizontal line
        GameObject hLine = new GameObject("HorizontalLine");
        hLine.transform.SetParent(parent, false);
        RectTransform hRect = hLine.AddComponent<RectTransform>();
        hRect.sizeDelta = new Vector2(30, 2);
        UnityEngine.UI.Image hImage = hLine.AddComponent<UnityEngine.UI.Image>();
        hImage.color = Color.green;

        // Center dot
        GameObject dot = new GameObject("CenterDot");
        dot.transform.SetParent(parent, false);
        RectTransform dotRect = dot.AddComponent<RectTransform>();
        dotRect.sizeDelta = new Vector2(4, 4);
        UnityEngine.UI.Image dotImage = dot.AddComponent<UnityEngine.UI.Image>();
        dotImage.color = Color.red;
    }

    private void Update()
    {
        UpdateScopeOverlay();
    }

    private void UpdateScopeOverlay()
    {
        if (overlayCanvasGroup == null) return;

        float targetAlpha = isActive ? 1f : 0f;
        overlayCanvasGroup.alpha = Mathf.Lerp(
            overlayCanvasGroup.alpha,
            targetAlpha,
            Time.deltaTime * overlayFadeSpeed
        );
    }

    /// <summary>
    /// Activates the scope view
    /// </summary>
    public void ActivateScope()
    {
        isActive = true;

        // Enhance thermal vision when scoped
        if (thermalRenderer != null && !thermalRenderer.IsActive)
        {
            thermalRenderer.ActivateThermalVision();
        }

        Debug.Log("[ScopeController] Scope activated");
    }

    /// <summary>
    /// Deactivates the scope view
    /// </summary>
    public void DeactivateScope()
    {
        isActive = false;

        Debug.Log("[ScopeController] Scope deactivated");
    }

    /// <summary>
    /// Cycle through zoom levels
    /// </summary>
    public void IncreaseZoom()
    {
        if (currentScope == null) return;

        currentZoomLevel++;
        if (currentZoomLevel >= zoomLevels.Length)
        {
            currentZoomLevel = zoomLevels.Length - 1;
        }

        ApplyZoom();
    }

    public void DecreaseZoom()
    {
        currentZoomLevel--;
        if (currentZoomLevel < 0)
        {
            currentZoomLevel = 0;
        }

        ApplyZoom();
    }

    private void ApplyZoom()
    {
        float zoomMultiplier = zoomLevels[currentZoomLevel];

        if (playerController != null && playerController.PlayerCamera != null)
        {
            // Zoom is handled by PlayerController FOV changes
            Debug.Log($"[ScopeController] Zoom level: {currentZoomLevel} (x{zoomMultiplier})");
        }
    }

    /// <summary>
    /// Equips a scope
    /// </summary>
    public void EquipScope(ScopeData scope)
    {
        currentScope = scope;

        // Destroy old scope model
        if (scopeModel != null)
        {
            Destroy(scopeModel);
        }

        // Create new scope model
        if (scope != null && playerController != null)
        {
            if (scope.scopePrefab != null)
            {
                scopeModel = Instantiate(scope.scopePrefab, playerController.WeaponMount);
                scopeModel.transform.localPosition = new Vector3(0, 0.1f, 0.2f);
            }
            else
            {
                // Create procedural scope
                scopeModel = ProceduralAssetFactory.Instance.CreateScope(scope.scopeName);
                scopeModel.transform.parent = playerController.WeaponMount;
                scopeModel.transform.localPosition = new Vector3(0, 0.1f, 0.2f);
                scopeModel.transform.localRotation = Quaternion.identity;
                scopeModel.transform.localScale = Vector3.one * 0.8f;
            }
        }

        Debug.Log($"[ScopeController] Equipped scope: {scope?.scopeName ?? "None"}");
    }

    /// <summary>
    /// Gets the current zoom multiplier
    /// </summary>
    public float GetCurrentZoom()
    {
        if (currentZoomLevel >= 0 && currentZoomLevel < zoomLevels.Length)
        {
            return zoomLevels[currentZoomLevel];
        }
        return 1f;
    }

    public ScopeData CurrentScope => currentScope;
    public bool IsActive => isActive;
}
