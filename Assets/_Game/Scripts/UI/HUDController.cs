using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Heads-Up Display controller
/// Shows ammo, score, time, battery, and mission objectives
/// </summary>
public class HUDController : MonoBehaviour
{
    [Header("HUD Elements")]
    [SerializeField] private TextMeshProUGUI ammoText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private TextMeshProUGUI targetsText;
    [SerializeField] private Image batteryBar;
    [SerializeField] private TextMeshProUGUI batteryText;

    [Header("Settings")]
    [SerializeField] private Color lowBatteryColor = Color.red;
    [SerializeField] private Color normalBatteryColor = Color.green;
    [SerializeField] private float lowBatteryThreshold = 0.25f;

    private GameManager gameManager;
    private ThermalRenderer thermalRenderer;
    private Canvas hudCanvas;

    private void Awake()
    {
        CreateHUDIfNeeded();
    }

    private void Start()
    {
        gameManager = GameManager.Instance;

        // Get thermal renderer from main camera
        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            thermalRenderer = mainCamera.GetComponent<ThermalRenderer>();
        }

        SubscribeToEvents();
    }

    private void CreateHUDIfNeeded()
    {
        // Find or create canvas
        hudCanvas = GetComponent<Canvas>();
        if (hudCanvas == null)
        {
            hudCanvas = gameObject.AddComponent<Canvas>();
            hudCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            hudCanvas.sortingOrder = 10;

            gameObject.AddComponent<CanvasScaler>();
            gameObject.AddComponent<GraphicRaycaster>();
        }

        // Create HUD elements if not assigned
        if (ammoText == null) ammoText = CreateTextElement("AmmoText", new Vector2(20, 20), TextAnchor.LowerLeft, 36);
        if (scoreText == null) scoreText = CreateTextElement("ScoreText", new Vector2(20, 60), TextAnchor.LowerLeft, 24);
        if (timeText == null) timeText = CreateTextElement("TimeText", new Vector2(-20, 20), TextAnchor.LowerRight, 32);
        if (targetsText == null) targetsText = CreateTextElement("TargetsText", new Vector2(-20, 60), TextAnchor.LowerRight, 24);

        CreateBatteryIndicator();
    }

    private TextMeshProUGUI CreateTextElement(string name, Vector2 position, TextAnchor anchor, int fontSize)
    {
        GameObject textObj = new GameObject(name);
        textObj.transform.SetParent(hudCanvas.transform, false);

        RectTransform rect = textObj.AddComponent<RectTransform>();

        // Set anchor based on position
        switch (anchor)
        {
            case TextAnchor.LowerLeft:
                rect.anchorMin = new Vector2(0, 0);
                rect.anchorMax = new Vector2(0, 0);
                rect.pivot = new Vector2(0, 0);
                break;
            case TextAnchor.LowerRight:
                rect.anchorMin = new Vector2(1, 0);
                rect.anchorMax = new Vector2(1, 0);
                rect.pivot = new Vector2(1, 0);
                break;
            case TextAnchor.UpperLeft:
                rect.anchorMin = new Vector2(0, 1);
                rect.anchorMax = new Vector2(0, 1);
                rect.pivot = new Vector2(0, 1);
                break;
            case TextAnchor.UpperRight:
                rect.anchorMin = new Vector2(1, 1);
                rect.anchorMax = new Vector2(1, 1);
                rect.pivot = new Vector2(1, 1);
                break;
        }

        rect.anchoredPosition = position;
        rect.sizeDelta = new Vector2(400, 50);

        TextMeshProUGUI text = textObj.AddComponent<TextMeshProUGUI>();
        text.fontSize = fontSize;
        text.color = Color.white;
        text.font = Resources.Load<TMP_FontAsset>("LiberationSans SDF");

        if (text.font == null)
        {
            // Fallback: Create from system font
            text.font = TMP_FontAsset.CreateFontAsset(Font.CreateDynamicFontFromOSFont("Arial", 32));
        }

        return text;
    }

    private void CreateBatteryIndicator()
    {
        // Battery container
        GameObject batteryContainer = new GameObject("BatteryIndicator");
        batteryContainer.transform.SetParent(hudCanvas.transform, false);

        RectTransform containerRect = batteryContainer.AddComponent<RectTransform>();
        containerRect.anchorMin = new Vector2(1, 1);
        containerRect.anchorMax = new Vector2(1, 1);
        containerRect.pivot = new Vector2(1, 1);
        containerRect.anchoredPosition = new Vector2(-20, -20);
        containerRect.sizeDelta = new Vector2(200, 30);

        // Background
        GameObject bgObj = new GameObject("Background");
        bgObj.transform.SetParent(batteryContainer.transform, false);
        RectTransform bgRect = bgObj.AddComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.sizeDelta = Vector2.zero;
        Image bgImage = bgObj.AddComponent<Image>();
        bgImage.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);

        // Battery bar
        GameObject barObj = new GameObject("BatteryBar");
        barObj.transform.SetParent(batteryContainer.transform, false);
        RectTransform barRect = barObj.AddComponent<RectTransform>();
        barRect.anchorMin = Vector2.zero;
        barRect.anchorMax = new Vector2(1, 1);
        barRect.sizeDelta = new Vector2(-4, -4);
        barRect.anchoredPosition = Vector2.zero;
        batteryBar = barObj.AddComponent<Image>();
        batteryBar.color = normalBatteryColor;
        batteryBar.type = Image.Type.Filled;
        batteryBar.fillMethod = Image.FillMethod.Horizontal;

        // Battery text
        GameObject textObj = new GameObject("BatteryText");
        textObj.transform.SetParent(batteryContainer.transform, false);
        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.sizeDelta = Vector2.zero;
        batteryText = textObj.AddComponent<TextMeshProUGUI>();
        batteryText.fontSize = 16;
        batteryText.color = Color.white;
        batteryText.alignment = TextAlignmentOptions.Center;
        batteryText.font = Resources.Load<TMP_FontAsset>("LiberationSans SDF");

        if (batteryText.font == null)
        {
            batteryText.font = TMP_FontAsset.CreateFontAsset(Font.CreateDynamicFontFromOSFont("Arial", 16));
        }
    }

    private void Update()
    {
        if (gameManager == null || gameManager.CurrentState != GameManager.GameState.InMission)
        {
            return;
        }

        UpdateHUD();
    }

    private void UpdateHUD()
    {
        // Update ammo
        if (ammoText != null)
        {
            int currentAmmo = gameManager.CurrentAmmo;
            int totalAmmo = gameManager.CurrentContract != null ? gameManager.CurrentContract.ammunitionAllowed : 0;
            ammoText.text = $"AMMO: {currentAmmo}/{totalAmmo}";

            // Color based on ammo level
            if (currentAmmo <= 3)
            {
                ammoText.color = Color.red;
            }
            else if (currentAmmo <= 5)
            {
                ammoText.color = Color.yellow;
            }
            else
            {
                ammoText.color = Color.white;
            }
        }

        // Update score
        if (scoreText != null)
        {
            scoreText.text = $"SCORE: {gameManager.CurrentScore}";
        }

        // Update time
        if (timeText != null)
        {
            float timeRemaining = gameManager.MissionTimeRemaining;
            int minutes = Mathf.FloorToInt(timeRemaining / 60f);
            int seconds = Mathf.FloorToInt(timeRemaining % 60f);
            timeText.text = $"TIME: {minutes:00}:{seconds:00}";

            // Color based on time
            if (timeRemaining <= 30f)
            {
                timeText.color = Color.red;
            }
            else if (timeRemaining <= 60f)
            {
                timeText.color = Color.yellow;
            }
            else
            {
                timeText.color = Color.white;
            }
        }

        // Update targets
        if (targetsText != null && gameManager.CurrentContract != null)
        {
            int killed = gameManager.TargetsKilled;
            int required = gameManager.CurrentContract.targetCount;
            targetsText.text = $"TARGETS: {killed}/{required}";
        }

        // Update battery
        if (batteryBar != null && thermalRenderer != null)
        {
            float batteryPercent = thermalRenderer.GetBatteryPercentage();
            batteryBar.fillAmount = batteryPercent;

            // Color based on battery level
            if (batteryPercent <= lowBatteryThreshold)
            {
                batteryBar.color = lowBatteryColor;
            }
            else
            {
                batteryBar.color = normalBatteryColor;
            }

            if (batteryText != null)
            {
                batteryText.text = $"BATTERY: {Mathf.RoundToInt(batteryPercent * 100)}%";
            }
        }
    }

    private void SubscribeToEvents()
    {
        EventBus.Subscribe<ScoreChangedEvent>(OnScoreChanged);
        EventBus.Subscribe<TargetKilledEvent>(OnTargetKilled);
    }

    private void OnDestroy()
    {
        EventBus.Unsubscribe<ScoreChangedEvent>(OnScoreChanged);
        EventBus.Unsubscribe<TargetKilledEvent>(OnTargetKilled);
    }

    private void OnScoreChanged(ScoreChangedEvent evt)
    {
        // Could add score popup animation here
    }

    private void OnTargetKilled(TargetKilledEvent evt)
    {
        // Could add kill confirmation visual here
    }

    public void Show()
    {
        hudCanvas.enabled = true;
    }

    public void Hide()
    {
        hudCanvas.enabled = false;
    }
}

// Additional events
public struct ScoreChangedEvent
{
    public int NewScore;
    public int Delta;
}

public struct TargetKilledEvent
{
    public Vector3 Position;
    public bool IsWeakPoint;
}

public struct TargetHitEvent
{
    public Vector3 Position;
    public int Damage;
}

public struct GamePausedEvent { }
public struct GameResumedEvent { }

public struct MissionStartedEvent
{
    public string ContractID;
}

public struct MissionCompletedEvent
{
    public string ContractID;
    public int Stars;
    public int Score;
    public float CompletionTime;
}

public struct MissionFailedEvent
{
    public string ContractID;
    public string Reason;
}

public struct ThermalActivatedEvent { }
public struct ThermalDeactivatedEvent { }
