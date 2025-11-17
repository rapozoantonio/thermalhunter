using UnityEngine;

/// <summary>
/// Game bootstrapper - initializes all core systems
/// Place this on a GameObject in the first scene
/// </summary>
public class Bootstrapper : MonoBehaviour
{
    [Header("Scene Setup")]
    [SerializeField] private bool autoSetupScene = true;
    [SerializeField] private bool createPlayer = true;
    [SerializeField] private bool createEnvironment = true;
    [SerializeField] private bool createUI = true;

    [Header("Environment Settings")]
    [SerializeField] private Vector3 playerStartPosition = new Vector3(0, 1, 0);
    [SerializeField] private float environmentSize = 50f;

    private void Awake()
    {
        Debug.Log("[Bootstrapper] Initializing game systems...");

        if (autoSetupScene)
        {
            SetupScene();
        }
    }

    private void SetupScene()
    {
        // Ensure core managers exist
        EnsureGameManager();
        EnsureSaveManager();
        EnsureEventBus();
        EnsureServiceLocator();

        // Create procedural asset factory
        EnsureProceduralFactory();

        // Setup scene components
        if (createEnvironment)
        {
            CreateEnvironment();
        }

        if (createPlayer)
        {
            CreatePlayer();
        }

        if (createUI)
        {
            CreateUI();
        }

        // Load default content
        LoadDefaultContent();

        Debug.Log("[Bootstrapper] Scene setup complete!");
    }

    private void EnsureGameManager()
    {
        if (GameManager.Instance == null)
        {
            GameObject go = new GameObject("GameManager");
            go.AddComponent<GameManager>();
            Debug.Log("[Bootstrapper] Created GameManager");
        }
    }

    private void EnsureSaveManager()
    {
        if (SaveManager.Instance == null)
        {
            GameObject go = new GameObject("SaveManager");
            go.AddComponent<SaveManager>();
            Debug.Log("[Bootstrapper] Created SaveManager");
        }
    }

    private void EnsureEventBus()
    {
        if (FindObjectOfType<EventBus>() == null)
        {
            GameObject go = new GameObject("EventBus");
            go.AddComponent<EventBus>();
            Debug.Log("[Bootstrapper] Created EventBus");
        }
    }

    private void EnsureServiceLocator()
    {
        if (ServiceLocator.Instance == null)
        {
            GameObject go = new GameObject("ServiceLocator");
            go.AddComponent<ServiceLocator>();
            Debug.Log("[Bootstrapper] Created ServiceLocator");
        }
    }

    private void EnsureProceduralFactory()
    {
        if (ProceduralAssetFactory.Instance == null)
        {
            GameObject go = new GameObject("ProceduralAssetFactory");
            go.AddComponent<ProceduralAssetFactory>();
            Debug.Log("[Bootstrapper] Created ProceduralAssetFactory");
        }
    }

    private void CreatePlayer()
    {
        // Check if player already exists
        if (FindObjectOfType<PlayerController>() != null)
        {
            Debug.Log("[Bootstrapper] Player already exists");
            return;
        }

        // Create player GameObject
        GameObject player = new GameObject("Player");
        player.transform.position = playerStartPosition;
        player.tag = Constants.TAG_PLAYER;
        player.layer = LayerMask.NameToLayer(Constants.LAYER_PLAYER);

        // Add CharacterController
        CharacterController cc = player.AddComponent<CharacterController>();
        cc.height = 1.8f;
        cc.radius = 0.4f;
        cc.center = new Vector3(0, 0.9f, 0);

        // Add player components
        PlayerController playerController = player.AddComponent<PlayerController>();
        WeaponController weaponController = player.AddComponent<WeaponController>();
        ScopeController scopeController = player.AddComponent<ScopeController>();
        BallisticsController ballisticsController = player.AddComponent<BallisticsController>();

        Debug.Log("[Bootstrapper] Created Player");
    }

    private void CreateEnvironment()
    {
        // Check if environment already exists
        if (GameObject.Find("Environment") != null)
        {
            Debug.Log("[Bootstrapper] Environment already exists");
            return;
        }

        GameObject environment = new GameObject("Environment");

        // Create procedural environment manager
        ProceduralEnvironmentManager envManager = environment.AddComponent<ProceduralEnvironmentManager>();
        envManager.GenerateEnvironment(environmentSize);

        Debug.Log("[Bootstrapper] Created Environment");
    }

    private void CreateUI()
    {
        // Check if UI already exists
        if (FindObjectOfType<HUDController>() != null)
        {
            Debug.Log("[Bootstrapper] UI already exists");
            return;
        }

        // Create HUD
        GameObject hudObj = new GameObject("HUD");
        HUDController hud = hudObj.AddComponent<HUDController>();

        // Create Menu
        GameObject menuObj = new GameObject("Menu");
        MenuController menu = menuObj.AddComponent<MenuController>();

        Debug.Log("[Bootstrapper] Created UI");
    }

    private void LoadDefaultContent()
    {
        // Create default contract data
        ContractData defaultContract = ScriptableObject.CreateInstance<ContractData>();
        defaultContract.contractID = "default_01";
        defaultContract.contractName = "First Hunt";
        defaultContract.description = "Eliminate 20 rats in the barn area";
        defaultContract.targetCount = 20;
        defaultContract.timeLimit = 300f; // 5 minutes
        defaultContract.ammunitionAllowed = 30;
        defaultContract.oneStarThreshold = 10;
        defaultContract.twoStarThreshold = 15;
        defaultContract.threeStarThreshold = 20;

        // Create default weapon data
        WeaponData defaultWeapon = ScriptableObject.CreateInstance<WeaponData>();
        defaultWeapon.weaponID = "rifle_basic";
        defaultWeapon.weaponName = "Basic Rifle";
        defaultWeapon.description = "Standard hunting rifle";
        defaultWeapon.damage = 1;
        defaultWeapon.muzzleVelocity = 300f;
        defaultWeapon.maxRange = 100f;
        defaultWeapon.accuracy = 0.95f;
        defaultWeapon.recoilAmount = 0.1f;
        defaultWeapon.fireRate = 1f;

        // Create default scope data
        ScopeData defaultScope = ScriptableObject.CreateInstance<ScopeData>();
        defaultScope.scopeID = "thermal_basic";
        defaultScope.scopeName = "Basic Thermal Scope";
        defaultScope.description = "Standard thermal imaging scope";
        defaultScope.magnification = 4f;
        defaultScope.thermalQuality = 1f;
        defaultScope.batteryDrainMultiplier = 1f;

        // Apply to game manager
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SelectContract(defaultContract);
            GameManager.Instance.EquipLoadout(defaultWeapon, defaultScope);

            // Also equip on player
            PlayerController player = FindObjectOfType<PlayerController>();
            if (player != null)
            {
                WeaponController weaponController = player.GetComponent<WeaponController>();
                ScopeController scopeController = player.GetComponent<ScopeController>();

                weaponController?.EquipWeapon(defaultWeapon);
                scopeController?.EquipScope(defaultScope);
            }
        }

        Debug.Log("[Bootstrapper] Loaded default content");
    }
}
