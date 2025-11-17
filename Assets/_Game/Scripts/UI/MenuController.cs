using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Main menu controller
/// Handles menu navigation, contract selection, and loadout setup
/// </summary>
public class MenuController : MonoBehaviour
{
    [Header("Menu Panels")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject contractSelectionPanel;
    [SerializeField] private GameObject loadoutPanel;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject missionCompletePanel;
    [SerializeField] private GameObject missionFailedPanel;

    [Header("Main Menu Buttons")]
    [SerializeField] private Button playButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button quitButton;

    private Canvas menuCanvas;
    private GameManager gameManager;

    private void Awake()
    {
        CreateMenuIfNeeded();
        gameManager = GameManager.Instance;
    }

    private void Start()
    {
        SubscribeToEvents();
        ShowMainMenu();
    }

    private void CreateMenuIfNeeded()
    {
        menuCanvas = GetComponent<Canvas>();
        if (menuCanvas == null)
        {
            menuCanvas = gameObject.AddComponent<Canvas>();
            menuCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            menuCanvas.sortingOrder = 100;

            gameObject.AddComponent<CanvasScaler>();
            gameObject.AddComponent<GraphicRaycaster>();
        }

        // Create main menu panel
        if (mainMenuPanel == null)
        {
            mainMenuPanel = CreatePanel("MainMenuPanel");
            CreateMainMenuUI(mainMenuPanel.transform);
        }

        // Create other panels (simplified)
        if (contractSelectionPanel == null)
        {
            contractSelectionPanel = CreatePanel("ContractSelectionPanel");
            contractSelectionPanel.SetActive(false);
        }

        if (loadoutPanel == null)
        {
            loadoutPanel = CreatePanel("LoadoutPanel");
            loadoutPanel.SetActive(false);
        }

        if (settingsPanel == null)
        {
            settingsPanel = CreatePanel("SettingsPanel");
            settingsPanel.SetActive(false);
        }

        if (missionCompletePanel == null)
        {
            missionCompletePanel = CreatePanel("MissionCompletePanel");
            missionCompletePanel.SetActive(false);
        }

        if (missionFailedPanel == null)
        {
            missionFailedPanel = CreatePanel("MissionFailedPanel");
            missionFailedPanel.SetActive(false);
        }
    }

    private GameObject CreatePanel(string name)
    {
        GameObject panel = new GameObject(name);
        panel.transform.SetParent(menuCanvas.transform, false);

        RectTransform rect = panel.AddComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.sizeDelta = Vector2.zero;

        Image image = panel.AddComponent<Image>();
        image.color = new Color(0, 0, 0, 0.9f);

        return panel;
    }

    private void CreateMainMenuUI(Transform parent)
    {
        // Title
        GameObject titleObj = new GameObject("Title");
        titleObj.transform.SetParent(parent, false);

        RectTransform titleRect = titleObj.AddComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0.5f, 0.7f);
        titleRect.anchorMax = new Vector2(0.5f, 0.7f);
        titleRect.sizeDelta = new Vector2(600, 100);

        TextMeshProUGUI titleText = titleObj.AddComponent<TextMeshProUGUI>();
        titleText.text = "THERMAL HUNT";
        titleText.fontSize = 64;
        titleText.color = Color.green;
        titleText.alignment = TextAlignmentOptions.Center;
        titleText.font = Resources.Load<TMP_FontAsset>("LiberationSans SDF");

        if (titleText.font == null)
        {
            titleText.font = TMP_FontAsset.CreateFontAsset(Font.CreateDynamicFontFromOSFont("Arial", 64));
        }

        // Play Button
        playButton = CreateButton("PlayButton", "START MISSION", parent, new Vector2(0.5f, 0.5f), OnPlayClicked);

        // Settings Button
        settingsButton = CreateButton("SettingsButton", "SETTINGS", parent, new Vector2(0.5f, 0.4f), OnSettingsClicked);

        // Quit Button
        quitButton = CreateButton("QuitButton", "QUIT", parent, new Vector2(0.5f, 0.3f), OnQuitClicked);
    }

    private Button CreateButton(string name, string text, Transform parent, Vector2 anchorPosition, UnityEngine.Events.UnityAction onClick)
    {
        GameObject buttonObj = new GameObject(name);
        buttonObj.transform.SetParent(parent, false);

        RectTransform rect = buttonObj.AddComponent<RectTransform>();
        rect.anchorMin = anchorPosition;
        rect.anchorMax = anchorPosition;
        rect.sizeDelta = new Vector2(300, 60);

        Image image = buttonObj.AddComponent<Image>();
        image.color = new Color(0.2f, 0.6f, 0.2f, 0.8f);

        Button button = buttonObj.AddComponent<Button>();
        button.targetGraphic = image;
        button.onClick.AddListener(onClick);

        // Button text
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(buttonObj.transform, false);

        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.sizeDelta = Vector2.zero;

        TextMeshProUGUI buttonText = textObj.AddComponent<TextMeshProUGUI>();
        buttonText.text = text;
        buttonText.fontSize = 32;
        buttonText.color = Color.white;
        buttonText.alignment = TextAlignmentOptions.Center;
        buttonText.font = Resources.Load<TMP_FontAsset>("LiberationSans SDF");

        if (buttonText.font == null)
        {
            buttonText.font = TMP_FontAsset.CreateFontAsset(Font.CreateDynamicFontFromOSFont("Arial", 32));
        }

        return button;
    }

    private void SubscribeToEvents()
    {
        EventBus.Subscribe<MissionStartedEvent>(OnMissionStarted);
        EventBus.Subscribe<MissionCompletedEvent>(OnMissionCompleted);
        EventBus.Subscribe<MissionFailedEvent>(OnMissionFailed);
    }

    private void OnDestroy()
    {
        EventBus.Unsubscribe<MissionStartedEvent>(OnMissionStarted);
        EventBus.Unsubscribe<MissionCompletedEvent>(OnMissionCompleted);
        EventBus.Unsubscribe<MissionFailedEvent>(OnMissionFailed);
    }

    // === Menu Navigation ===

    public void ShowMainMenu()
    {
        HideAllPanels();
        mainMenuPanel.SetActive(true);
    }

    public void ShowContractSelection()
    {
        HideAllPanels();
        contractSelectionPanel.SetActive(true);
    }

    public void ShowLoadout()
    {
        HideAllPanels();
        loadoutPanel.SetActive(true);
    }

    public void ShowSettings()
    {
        HideAllPanels();
        settingsPanel.SetActive(true);
    }

    public void ShowMissionComplete()
    {
        HideAllPanels();
        missionCompletePanel.SetActive(true);
    }

    public void ShowMissionFailed()
    {
        HideAllPanels();
        missionFailedPanel.SetActive(true);
    }

    public void HideAllPanels()
    {
        mainMenuPanel?.SetActive(false);
        contractSelectionPanel?.SetActive(false);
        loadoutPanel?.SetActive(false);
        settingsPanel?.SetActive(false);
        missionCompletePanel?.SetActive(false);
        missionFailedPanel?.SetActive(false);
    }

    public void HideAll()
    {
        menuCanvas.enabled = false;
    }

    public void ShowAll()
    {
        menuCanvas.enabled = true;
    }

    // === Button Handlers ===

    private void OnPlayClicked()
    {
        Debug.Log("[MenuController] Play button clicked");

        // For now, start directly with default loadout
        StartQuickPlay();
    }

    private void OnSettingsClicked()
    {
        ShowSettings();
    }

    private void OnQuitClicked()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    /// <summary>
    /// Quick play - start immediately with default contract/loadout
    /// </summary>
    private void StartQuickPlay()
    {
        // This would normally load contract and loadout selection
        // For now, start the mission directly

        if (gameManager != null)
        {
            HideAll();
            gameManager.ChangeState(GameManager.GameState.InMission);
        }
    }

    // === Event Handlers ===

    private void OnMissionStarted(MissionStartedEvent evt)
    {
        HideAll();
    }

    private void OnMissionCompleted(MissionCompletedEvent evt)
    {
        ShowAll();
        ShowMissionComplete();
    }

    private void OnMissionFailed(MissionFailedEvent evt)
    {
        ShowAll();
        ShowMissionFailed();
    }
}
