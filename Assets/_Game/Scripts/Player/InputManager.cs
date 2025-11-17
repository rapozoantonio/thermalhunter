using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Unified cross-platform input handling
/// Supports mobile touch and desktop mouse/keyboard
/// </summary>
public class InputManager : Singleton<InputManager>
{
    [Header("Mobile Touch Settings")]
    [SerializeField] private float touchSensitivity = 0.5f;
    [SerializeField] private float aimAssistStrength = 0.3f;
    [SerializeField] private float aimAssistRadius = 50f;

    [Header("Desktop Mouse Settings")]
    [SerializeField] private float mouseSensitivity = 1.0f;
    [SerializeField] private bool invertYAxis = false;

    [Header("Touch Zones")]
    [SerializeField] private Rect aimZone = new Rect(0.5f, 0f, 0.5f, 1f); // Right half of screen
    [SerializeField] private Rect fireButtonZone = new Rect(0.8f, 0.1f, 0.15f, 0.15f);

    private Camera mainCamera;
    private bool isMobile;

    // Input state
    private Vector2 aimDelta;
    private bool firePressed;
    private bool fireHeld;
    private bool holdBreathPressed;
    private bool holdBreathHeld;
    private float zoomInput;

    // Touch tracking
    private int aimTouchId = -1;
    private Vector2 lastTouchPosition;

    // Events
    public event System.Action<Vector2> OnAimMove;
    public event System.Action OnFirePressed;
    public event System.Action OnFireReleased;
    public event System.Action OnHoldBreathPressed;
    public event System.Action OnHoldBreathReleased;
    public event System.Action OnZoomIn;
    public event System.Action OnZoomOut;
    public event System.Action OnPausePressed;

    // Properties
    public Vector2 AimDelta => aimDelta;
    public bool FirePressed => firePressed;
    public bool FireHeld => fireHeld;
    public bool HoldBreathPressed => holdBreathPressed;
    public bool HoldBreathHeld => holdBreathHeld;
    public float ZoomInput => zoomInput;

    protected override void Awake()
    {
        base.Awake();
        mainCamera = Camera.main;
        isMobile = Application.isMobilePlatform;

        // Load sensitivity settings
        LoadSettings();

        // Register with ServiceLocator
        ServiceLocator.Instance.Register<InputManager>(this);
    }

    private void LoadSettings()
    {
        SaveManager saveManager = SaveManager.Instance;
        if (saveManager != null && saveManager.CurrentSave != null)
        {
            SetSensitivity(saveManager.CurrentSave.sensitivity);
        }
    }

    private void Update()
    {
        // Reset per-frame inputs
        firePressed = false;
        holdBreathPressed = false;
        zoomInput = 0f;
        aimDelta = Vector2.zero;

        if (isMobile)
        {
            ProcessMobileInput();
        }
        else
        {
            ProcessDesktopInput();
        }

        // Invoke events
        if (aimDelta.magnitude > 0.01f)
        {
            OnAimMove?.Invoke(aimDelta);
        }
    }

    // === Mobile Input ===

    private void ProcessMobileInput()
    {
        ProcessTouches();
    }

    private void ProcessTouches()
    {
        for (int i = 0; i < Input.touchCount; i++)
        {
            Touch touch = Input.GetTouch(i);

            // Skip if touching UI
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(touch.fingerId))
            {
                continue;
            }

            Vector2 normalizedPos = new Vector2(touch.position.x / Screen.width, touch.position.y / Screen.height);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    OnTouchBegan(touch, normalizedPos);
                    break;

                case TouchPhase.Moved:
                case TouchPhase.Stationary:
                    OnTouchMoved(touch, normalizedPos);
                    break;

                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    OnTouchEnded(touch, normalizedPos);
                    break;
            }
        }
    }

    private void OnTouchBegan(Touch touch, Vector2 normalizedPos)
    {
        // Check fire button
        if (fireButtonZone.Contains(normalizedPos))
        {
            firePressed = true;
            fireHeld = true;
            OnFirePressed?.Invoke();
            return;
        }

        // Check aim zone
        if (aimZone.Contains(normalizedPos) && aimTouchId == -1)
        {
            aimTouchId = touch.fingerId;
            lastTouchPosition = touch.position;
        }
    }

    private void OnTouchMoved(Touch touch, Vector2 normalizedPos)
    {
        if (touch.fingerId == aimTouchId)
        {
            Vector2 delta = touch.position - lastTouchPosition;

            // Apply sensitivity
            delta *= touchSensitivity;

            // Apply aim assist
            delta = ApplyAimAssist(touch.position, delta);

            aimDelta = delta;
            lastTouchPosition = touch.position;
        }
    }

    private void OnTouchEnded(Touch touch, Vector2 normalizedPos)
    {
        if (touch.fingerId == aimTouchId)
        {
            aimTouchId = -1;
        }

        if (fireButtonZone.Contains(normalizedPos))
        {
            fireHeld = false;
            OnFireReleased?.Invoke();
        }
    }

    private Vector2 ApplyAimAssist(Vector2 touchPosition, Vector2 delta)
    {
        if (aimAssistStrength <= 0f) return delta;

        Ray ray = mainCamera.ScreenPointToRay(touchPosition);
        RaycastHit[] hits = Physics.RaycastAll(ray, 100f);

        foreach (var hit in hits)
        {
            if (hit.collider.CompareTag(Constants.TAG_RAT))
            {
                // Calculate screen position of target center
                Vector3 targetScreenPos = mainCamera.WorldToScreenPoint(hit.collider.bounds.center);

                // Check if within assist radius
                float distance = Vector2.Distance(touchPosition, new Vector2(targetScreenPos.x, targetScreenPos.y));

                if (distance < aimAssistRadius)
                {
                    // Magnetize aim toward target
                    Vector2 toTarget = new Vector2(targetScreenPos.x, targetScreenPos.y) - touchPosition;
                    delta = Vector2.Lerp(delta, toTarget.normalized * delta.magnitude, aimAssistStrength);
                }

                break;
            }
        }

        return delta;
    }

    // === Desktop Input ===

    private void ProcessDesktopInput()
    {
        // Mouse aim
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        if (invertYAxis)
        {
            mouseY = -mouseY;
        }

        aimDelta = new Vector2(mouseX, mouseY);

        // Fire
        if (Input.GetMouseButtonDown(0))
        {
            firePressed = true;
            fireHeld = true;
            OnFirePressed?.Invoke();
        }

        if (Input.GetMouseButtonUp(0))
        {
            fireHeld = false;
            OnFireReleased?.Invoke();
        }

        // Hold breath
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            holdBreathPressed = true;
            holdBreathHeld = true;
            OnHoldBreathPressed?.Invoke();
        }

        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            holdBreathHeld = false;
            OnHoldBreathReleased?.Invoke();
        }

        // Zoom
        float scrollWheel = Input.GetAxis("Mouse ScrollWheel");
        if (scrollWheel > 0f)
        {
            zoomInput = 1f;
            OnZoomIn?.Invoke();
        }
        else if (scrollWheel < 0f)
        {
            zoomInput = -1f;
            OnZoomOut?.Invoke();
        }

        // Pause
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OnPausePressed?.Invoke();
        }

        // Reload (R key)
        if (Input.GetKeyDown(KeyCode.R))
        {
            // Reload logic would go here
        }
    }

    // === Settings ===

    public void SetSensitivity(float value)
    {
        if (isMobile)
        {
            touchSensitivity = Mathf.Clamp(value, 0.1f, 2f);
        }
        else
        {
            mouseSensitivity = Mathf.Clamp(value, 0.1f, 5f);
        }

        SaveManager.Instance?.SetSensitivity(value);
    }

    public void SetInvertY(bool invert)
    {
        invertYAxis = invert;
    }

    public void SetAimAssist(float strength)
    {
        aimAssistStrength = Mathf.Clamp01(strength);
    }

    public float GetSensitivity()
    {
        return isMobile ? touchSensitivity : mouseSensitivity;
    }

    public bool IsMobilePlatform => isMobile;
}
