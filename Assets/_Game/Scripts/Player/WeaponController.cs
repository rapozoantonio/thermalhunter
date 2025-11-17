using UnityEngine;

/// <summary>
/// Controls weapon firing, recoil, and ballistics
/// Works with PlayerController and BallisticsController
/// </summary>
public class WeaponController : MonoBehaviour
{
    [Header("Weapon Configuration")]
    [SerializeField] private WeaponData currentWeapon;
    [SerializeField] private Transform muzzlePoint;

    [Header("Visual Feedback")]
    [SerializeField] private GameObject muzzleFlashPrefab;
    [SerializeField] private LineRenderer tracerRenderer;
    [SerializeField] private float tracerDuration = 0.05f;

    [Header("Recoil")]
    [SerializeField] private float recoilRecoverySpeed = 5f;

    private PlayerController playerController;
    private BallisticsController ballisticsController;
    private AudioManager audioManager;
    private float nextFireTime;
    private Vector3 recoilOffset;
    private GameObject weaponModel;

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
        ballisticsController = GetComponent<BallisticsController>();

        if (ballisticsController == null)
        {
            ballisticsController = gameObject.AddComponent<BallisticsController>();
        }

        SetupMuzzlePoint();
        SetupTracerRenderer();
    }

    private void Start()
    {
        audioManager = ServiceLocator.Instance.TryGet<AudioManager>();

        // Load default weapon if not set
        if (currentWeapon == null)
        {
            Debug.LogWarning("[WeaponController] No weapon assigned, creating default");
        }
    }

    private void SetupMuzzlePoint()
    {
        if (muzzlePoint == null && playerController != null && playerController.WeaponMount != null)
        {
            GameObject muzzleObj = new GameObject("MuzzlePoint");
            muzzleObj.transform.parent = playerController.WeaponMount;
            muzzleObj.transform.localPosition = new Vector3(0, 0, 0.6f);
            muzzlePoint = muzzleObj.transform;
        }
    }

    private void SetupTracerRenderer()
    {
        if (tracerRenderer == null && muzzlePoint != null)
        {
            GameObject tracerObj = new GameObject("TracerRenderer");
            tracerObj.transform.parent = transform;
            tracerRenderer = tracerObj.AddComponent<LineRenderer>();
            tracerRenderer.startWidth = 0.01f;
            tracerRenderer.endWidth = 0.005f;
            tracerRenderer.material = new Material(Shader.Find("Sprites/Default"));
            tracerRenderer.startColor = Color.yellow;
            tracerRenderer.endColor = Color.red;
            tracerRenderer.enabled = false;
        }
    }

    private void Update()
    {
        RecoverFromRecoil();
    }

    /// <summary>
    /// Fires the weapon
    /// </summary>
    public void Fire()
    {
        // Check if can fire
        if (!CanFire())
        {
            return;
        }

        // Check ammo
        if (GameManager.Instance.CurrentAmmo <= 0)
        {
            PlayDryFireSound();
            return;
        }

        // Fire
        PerformShot();

        // Update fire time
        nextFireTime = Time.time + currentWeapon.FireDelay;

        // Publish event
        EventBus.Publish(new WeaponFiredEvent
        {
            WeaponID = currentWeapon.weaponID,
            Position = muzzlePoint.position
        });
    }

    private bool CanFire()
    {
        if (currentWeapon == null)
        {
            Debug.LogWarning("[WeaponController] No weapon equipped!");
            return false;
        }

        if (Time.time < nextFireTime)
        {
            return false;
        }

        if (GameManager.Instance == null || GameManager.Instance.CurrentState != GameManager.GameState.InMission)
        {
            return false;
        }

        return true;
    }

    private void PerformShot()
    {
        // Get aim direction
        Ray aimRay = playerController.GetAimRay();

        // Apply accuracy deviation
        Vector3 spreadDirection = ApplySpread(aimRay.direction);

        // Fire using ballistics controller
        if (ballisticsController != null)
        {
            ballisticsController.FireBullet(
                muzzlePoint.position,
                spreadDirection,
                currentWeapon.muzzleVelocity,
                currentWeapon.damage
            );
        }

        // Visual feedback
        ShowMuzzleFlash();
        ShowTracer(muzzlePoint.position, muzzlePoint.position + spreadDirection * 100f);

        // Audio feedback
        PlayFireSound();

        // Recoil
        ApplyRecoil();

        // Haptic feedback (mobile)
#if UNITY_IOS || UNITY_ANDROID
        Handheld.Vibrate();
#endif
    }

    private Vector3 ApplySpread(Vector3 direction)
    {
        if (currentWeapon == null) return direction;

        float inaccuracy = 1f - currentWeapon.accuracy;

        // Add random deviation based on weapon accuracy
        float spreadAngle = inaccuracy * 5f; // Max 5 degrees spread

        float randomX = Random.Range(-spreadAngle, spreadAngle);
        float randomY = Random.Range(-spreadAngle, spreadAngle);

        Quaternion spread = Quaternion.Euler(randomX, randomY, 0);
        return spread * direction;
    }

    private void ApplyRecoil()
    {
        if (currentWeapon == null) return;

        // Vertical recoil (camera kick)
        float recoilAmount = currentWeapon.recoilAmount;
        recoilOffset += new Vector3(-recoilAmount * 2f, 0, -recoilAmount * 0.5f);

        // Apply to weapon mount
        if (playerController != null && playerController.WeaponMount != null)
        {
            playerController.WeaponMount.localPosition += recoilOffset;
        }
    }

    private void RecoverFromRecoil()
    {
        recoilOffset = Vector3.Lerp(recoilOffset, Vector3.zero, Time.deltaTime * recoilRecoverySpeed);
    }

    private void ShowMuzzleFlash()
    {
        if (muzzleFlashPrefab != null && muzzlePoint != null)
        {
            GameObject flash = Instantiate(muzzleFlashPrefab, muzzlePoint.position, muzzlePoint.rotation);
            Destroy(flash, 0.1f);
        }
    }

    private void ShowTracer(Vector3 start, Vector3 end)
    {
        if (tracerRenderer != null)
        {
            StartCoroutine(ShowTracerCoroutine(start, end));
        }
    }

    private System.Collections.IEnumerator ShowTracerCoroutine(Vector3 start, Vector3 end)
    {
        tracerRenderer.enabled = true;
        tracerRenderer.SetPosition(0, start);
        tracerRenderer.SetPosition(1, end);

        yield return new WaitForSeconds(tracerDuration);

        tracerRenderer.enabled = false;
    }

    private void PlayFireSound()
    {
        if (audioManager != null && currentWeapon != null && currentWeapon.fireSound != null)
        {
            audioManager.Play("WeaponFire");
        }
    }

    private void PlayDryFireSound()
    {
        if (audioManager != null)
        {
            audioManager.Play("DryFire");
        }
    }

    /// <summary>
    /// Equips a weapon
    /// </summary>
    public void EquipWeapon(WeaponData weapon)
    {
        currentWeapon = weapon;

        // Destroy old weapon model
        if (weaponModel != null)
        {
            Destroy(weaponModel);
        }

        // Create new weapon model
        if (weapon != null)
        {
            if (weapon.weaponPrefab != null)
            {
                weaponModel = Instantiate(weapon.weaponPrefab, playerController.WeaponMount);
            }
            else
            {
                // Create procedural weapon
                weaponModel = ProceduralAssetFactory.Instance.CreateWeapon(weapon.weaponName);
                weaponModel.transform.parent = playerController.WeaponMount;
                weaponModel.transform.localPosition = Vector3.zero;
                weaponModel.transform.localRotation = Quaternion.identity;
            }
        }

        Debug.Log($"[WeaponController] Equipped weapon: {weapon?.weaponName ?? "None"}");
    }

    public WeaponData CurrentWeapon => currentWeapon;
}

// Event
public struct WeaponFiredEvent
{
    public string WeaponID;
    public Vector3 Position;
}
