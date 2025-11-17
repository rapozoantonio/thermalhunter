using UnityEngine;

/// <summary>
/// Main player controller for first-person camera and movement
/// Handles camera rotation, weapon positioning, and player state
/// </summary>
[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Camera Settings")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float lookSensitivity = 2f;
    [SerializeField] private float maxLookAngle = 80f;
    [SerializeField] private Vector3 cameraOffset = new Vector3(0, 1.6f, 0);

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float sprintMultiplier = 1.5f;
    [SerializeField] private bool allowMovement = false; // Static position for scope gameplay

    [Header("Weapon Mount")]
    [SerializeField] private Transform weaponMount;
    [SerializeField] private Vector3 weaponMountOffset = new Vector3(0.3f, -0.2f, 0.5f);

    [Header("Scope Settings")]
    [SerializeField] private float normalFOV = 60f;
    [SerializeField] private float scopedFOV = 20f;
    [SerializeField] private float zoomSpeed = 5f;

    [Header("Breathing Effect")]
    [SerializeField] private float breathingAmplitude = 0.005f;
    [SerializeField] private float breathingSpeed = 1.5f;
    [SerializeField] private float holdBreathReduction = 0.8f;

    private CharacterController characterController;
    private InputManager inputManager;
    private WeaponController weaponController;
    private ScopeController scopeController;

    private float currentRotationX;
    private float currentRotationY;
    private float targetFOV;
    private bool isScoped;
    private bool isHoldingBreath;
    private float breathingTimer;

    // Properties
    public Camera PlayerCamera => playerCamera;
    public Transform WeaponMount => weaponMount;
    public bool IsScoped => isScoped;

    private void Awake()
    {
        InitializeComponents();
        SetupWeaponMount();
    }

    private void InitializeComponents()
    {
        characterController = GetComponent<CharacterController>();

        // Create camera if not assigned
        if (playerCamera == null)
        {
            GameObject cameraObj = new GameObject("PlayerCamera");
            cameraObj.transform.parent = transform;
            cameraObj.transform.localPosition = cameraOffset;
            playerCamera = cameraObj.AddComponent<Camera>();
            playerCamera.fieldOfView = normalFOV;

            // Add thermal renderer
            cameraObj.AddComponent<ThermalRenderer>();

            // Set as main camera
            playerCamera.tag = "MainCamera";
        }

        inputManager = InputManager.Instance;
        targetFOV = normalFOV;

        // Register with ServiceLocator
        ServiceLocator.Instance.Register<PlayerController>(this);
    }

    private void SetupWeaponMount()
    {
        if (weaponMount == null)
        {
            GameObject mountObj = new GameObject("WeaponMount");
            mountObj.transform.parent = playerCamera.transform;
            mountObj.transform.localPosition = weaponMountOffset;
            mountObj.transform.localRotation = Quaternion.identity;
            weaponMount = mountObj.transform;
        }
    }

    private void Start()
    {
        // Lock and hide cursor for FPS
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Get weapon and scope controllers
        weaponController = GetComponent<WeaponController>();
        scopeController = GetComponent<ScopeController>();

        // Subscribe to input events
        SubscribeToInput();

        // Activate thermal vision at start
        ThermalRenderer thermalRenderer = playerCamera.GetComponent<ThermalRenderer>();
        if (thermalRenderer != null)
        {
            thermalRenderer.ActivateThermalVision();
        }
    }

    private void SubscribeToInput()
    {
        if (inputManager != null)
        {
            inputManager.OnAimMove += OnAimMove;
            inputManager.OnFirePressed += OnFirePressed;
            inputManager.OnHoldBreathPressed += OnHoldBreathStart;
            inputManager.OnHoldBreathReleased += OnHoldBreathEnd;
            inputManager.OnZoomIn += OnZoomIn;
            inputManager.OnZoomOut += OnZoomOut;
            inputManager.OnPausePressed += OnPausePressed;
        }
    }

    private void OnDestroy()
    {
        if (inputManager != null)
        {
            inputManager.OnAimMove -= OnAimMove;
            inputManager.OnFirePressed -= OnFirePressed;
            inputManager.OnHoldBreathPressed -= OnHoldBreathStart;
            inputManager.OnHoldBreathReleased -= OnHoldBreathEnd;
            inputManager.OnZoomIn -= OnZoomIn;
            inputManager.OnZoomOut -= OnZoomOut;
            inputManager.OnPausePressed -= OnPausePressed;
        }
    }

    private void Update()
    {
        UpdateBreathing();
        UpdateFOV();

        if (allowMovement)
        {
            HandleMovement();
        }
    }

    private void LateUpdate()
    {
        ApplyBreathingEffect();
    }

    // === Camera Control ===

    private void OnAimMove(Vector2 delta)
    {
        // Apply sensitivity
        float sensitivity = lookSensitivity * (isScoped ? 0.5f : 1f);
        delta *= sensitivity;

        // Update rotation
        currentRotationY += delta.x;
        currentRotationX -= delta.y;

        // Clamp vertical rotation
        currentRotationX = Mathf.Clamp(currentRotationX, -maxLookAngle, maxLookAngle);

        // Apply rotation to camera
        transform.rotation = Quaternion.Euler(0, currentRotationY, 0);
        playerCamera.transform.localRotation = Quaternion.Euler(currentRotationX, 0, 0);
    }

    // === Movement (optional for this game) ===

    private void HandleMovement()
    {
        Vector3 moveDirection = Vector3.zero;

        // Get input (WASD or virtual joystick)
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        if (horizontal != 0 || vertical != 0)
        {
            Vector3 forward = transform.forward;
            Vector3 right = transform.right;

            forward.y = 0;
            right.y = 0;
            forward.Normalize();
            right.Normalize();

            moveDirection = (forward * vertical + right * horizontal).normalized;

            // Apply speed
            float speed = moveSpeed;
            if (Input.GetKey(KeyCode.LeftShift))
            {
                speed *= sprintMultiplier;
            }

            moveDirection *= speed;
        }

        // Apply gravity
        moveDirection.y = -9.81f;

        // Move character
        characterController.Move(moveDirection * Time.deltaTime);
    }

    // === Breathing Effect ===

    private void UpdateBreathing()
    {
        breathingTimer += Time.deltaTime * breathingSpeed;
    }

    private void ApplyBreathingEffect()
    {
        if (playerCamera == null) return;

        float amplitude = isHoldingBreath ? breathingAmplitude * holdBreathReduction : breathingAmplitude;

        // Subtle breathing sway
        float breathX = Mathf.Sin(breathingTimer) * amplitude;
        float breathY = Mathf.Cos(breathingTimer * 0.7f) * amplitude * 0.5f;

        Vector3 breathingOffset = new Vector3(breathX, breathY, 0);
        weaponMount.localPosition = weaponMountOffset + breathingOffset;

        // Camera rotation breathing
        Quaternion breathingRotation = Quaternion.Euler(
            breathY * 10f,
            breathX * 10f,
            breathX * 5f
        );

        weaponMount.localRotation = breathingRotation;
    }

    // === Scope Control ===

    private void OnZoomIn()
    {
        if (!isScoped)
        {
            EnterScope();
        }
    }

    private void OnZoomOut()
    {
        if (isScoped)
        {
            ExitScope();
        }
    }

    public void EnterScope()
    {
        isScoped = true;
        targetFOV = scopedFOV;

        if (scopeController != null)
        {
            scopeController.ActivateScope();
        }

        EventBus.Publish(new ScopeEnteredEvent());
    }

    public void ExitScope()
    {
        isScoped = false;
        targetFOV = normalFOV;

        if (scopeController != null)
        {
            scopeController.DeactivateScope();
        }

        EventBus.Publish(new ScopeExitedEvent());
    }

    private void UpdateFOV()
    {
        if (playerCamera != null)
        {
            playerCamera.fieldOfView = Mathf.Lerp(
                playerCamera.fieldOfView,
                targetFOV,
                Time.deltaTime * zoomSpeed
            );
        }
    }

    // === Input Callbacks ===

    private void OnFirePressed()
    {
        if (weaponController != null)
        {
            weaponController.Fire();
        }
    }

    private void OnHoldBreathStart()
    {
        isHoldingBreath = true;
    }

    private void OnHoldBreathEnd()
    {
        isHoldingBreath = false;
    }

    private void OnPausePressed()
    {
        GameManager.Instance?.PauseGame();
    }

    // === Public Methods ===

    public void SetLookSensitivity(float sensitivity)
    {
        lookSensitivity = sensitivity;
    }

    public void SetAllowMovement(bool allow)
    {
        allowMovement = allow;
    }

    public Vector3 GetAimPoint()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        return ray.GetPoint(100f);
    }

    public Ray GetAimRay()
    {
        return new Ray(playerCamera.transform.position, playerCamera.transform.forward);
    }
}

// Events
public struct ScopeEnteredEvent { }
public struct ScopeExitedEvent { }
