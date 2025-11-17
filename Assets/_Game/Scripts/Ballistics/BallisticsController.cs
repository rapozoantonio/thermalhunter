using UnityEngine;
using System.Collections;

/// <summary>
/// Realistic bullet physics and ballistics calculation
/// Handles shooting, hit detection, and feedback
/// </summary>
public class BallisticsController : MonoBehaviour
{
    [Header("Weapon Configuration")]
    [SerializeField] private WeaponData currentWeapon;

    [Header("Ballistics Settings")]
    [SerializeField] private float gravity = 9.81f;
    [SerializeField] private float airResistance = 0.01f;
    [SerializeField] private LayerMask hitMask;

    [Header("Visual Effects")]
    [SerializeField] private LineRenderer bulletTrailPrefab;
    [SerializeField] private GameObject impactEffectPrefab;
    [SerializeField] private float trailDuration = 0.2f;

    private Camera mainCamera;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    public void Fire(Vector3 origin, Vector3 aimDirection, WeaponData weapon)
    {
        currentWeapon = weapon;

        // Calculate trajectory with bullet drop
        BulletTrajectory trajectory = CalculateTrajectory(origin, aimDirection, weapon.muzzleVelocity);

        // Perform raycast
        RaycastHit hit;
        bool didHit = Physics.Raycast(origin, trajectory.initialDirection, out hit, weapon.maxRange, hitMask);

        Vector3 endPoint = didHit ? hit.point : origin + trajectory.initialDirection * weapon.maxRange;

        // Visual effects
        CreateBulletTrail(origin, endPoint);

        if (didHit)
        {
            ProcessHit(hit, trajectory.impactVelocity);
        }

        // Sound propagation
        PropagateShotSound(origin, weapon.soundLevel);

        // Weapon effects
        ApplyRecoil(weapon.recoilAmount);

        // Publish event
        EventBus.Publish(new WeaponFiredEvent
        {
            WeaponID = weapon.weaponID,
            Origin = origin,
            HitPoint = endPoint
        });

        Debug.Log($"[Ballistics] Fired {weapon.weaponName} - Hit: {didHit}");
    }

    private struct BulletTrajectory
    {
        public Vector3 initialDirection;
        public float impactVelocity;
        public float travelTime;
    }

    private BulletTrajectory CalculateTrajectory(Vector3 origin, Vector3 direction, float muzzleVelocity)
    {
        BulletTrajectory trajectory = new BulletTrajectory();

        // Simplified ballistics - good enough for gameplay
        float distance = currentWeapon.maxRange;
        float time = distance / muzzleVelocity;

        // Bullet drop calculation
        float drop = 0.5f * gravity * time * time;

        // Adjust aim direction for drop
        Vector3 adjusted = direction;
        adjusted.y += drop / distance;

        trajectory.initialDirection = adjusted.normalized;
        trajectory.travelTime = time;
        trajectory.impactVelocity = muzzleVelocity * (1f - airResistance * time);

        return trajectory;
    }

    private void ProcessHit(RaycastHit hit, float impactVelocity)
    {
        // Check if hit a target with heat signature
        HeatSignature heatSig = hit.collider.GetComponent<HeatSignature>();

        if (heatSig != null && heatSig.IsAlive)
        {
            ProcessTargetHit(hit, heatSig, impactVelocity);
        }
        else
        {
            ProcessEnvironmentHit(hit, impactVelocity);
        }
    }

    private void ProcessTargetHit(RaycastHit hit, HeatSignature target, float velocity)
    {
        RatAI ratAI = target.GetComponent<RatAI>();
        if (ratAI == null || ratAI.IsDead) return;

        // Check if weak point hit
        bool isWeakPoint = hit.collider.CompareTag(Constants.TAG_WEAK_POINT);

        // Calculate damage
        int damage = CalculateDamage(velocity, isWeakPoint);

        // Apply damage
        ratAI.TakeDamage(damage, hit.point, isWeakPoint);

        // Feedback
        ShowHitMarker(isWeakPoint);
        SpawnDamageNumber(hit.point, damage, isWeakPoint);

        // Analytics
        ServiceLocator.Instance.TryGet<AnalyticsManager>()?.TrackShot(true, isWeakPoint);

        // Publish event
        EventBus.Publish(new TargetHitEvent
        {
            Target = ratAI.gameObject,
            IsWeakPoint = isWeakPoint,
            Damage = damage
        });

        Debug.Log($"[Ballistics] Hit {ratAI.Type} rat - Weak point: {isWeakPoint}, Damage: {damage}");
    }

    private int CalculateDamage(float velocity, bool isWeakPoint)
    {
        int baseDamage = currentWeapon.damage;

        // Velocity affects damage slightly
        float velocityMultiplier = Mathf.Clamp(velocity / currentWeapon.muzzleVelocity, 0.8f, 1.2f);
        int finalDamage = Mathf.RoundToInt(baseDamage * velocityMultiplier);

        // Weak point multiplier
        if (isWeakPoint)
        {
            finalDamage *= 2;
        }

        return finalDamage;
    }

    private void ProcessEnvironmentHit(RaycastHit hit, float velocity)
    {
        // Spawn impact effect
        if (impactEffectPrefab != null)
        {
            GameObject impact = Instantiate(impactEffectPrefab, hit.point, Quaternion.LookRotation(hit.normal));
            Destroy(impact, 2f);
        }

        // Play sound
        ServiceLocator.Instance.TryGet<AudioManager>()?.PlayAtPoint("BulletImpact", hit.point);

        // Analytics
        ServiceLocator.Instance.TryGet<AnalyticsManager>()?.TrackShot(false, false);

        // Publish event
        EventBus.Publish(new MissedShotEvent { HitPoint = hit.point });

        Debug.Log($"[Ballistics] Hit environment at {hit.point}");
    }

    private void CreateBulletTrail(Vector3 start, Vector3 end)
    {
        if (bulletTrailPrefab == null) return;

        LineRenderer trail = Instantiate(bulletTrailPrefab, start, Quaternion.identity);
        trail.SetPosition(0, start);
        trail.SetPosition(1, end);

        StartCoroutine(FadeTrail(trail, trailDuration));
    }

    private IEnumerator FadeTrail(LineRenderer trail, float duration)
    {
        float elapsed = 0f;
        Color startColor = trail.startColor;
        Color endColor = trail.endColor;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            trail.startColor = Color.Lerp(startColor, Color.clear, t);
            trail.endColor = Color.Lerp(endColor, Color.clear, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        Destroy(trail.gameObject);
    }

    private void PropagateShotSound(Vector3 origin, float soundLevel)
    {
        // Notify all rats within hearing range
        Collider[] nearbyColliders = Physics.OverlapSphere(origin, soundLevel * 10f);

        foreach (var col in nearbyColliders)
        {
            RatAI rat = col.GetComponent<RatAI>();
            if (rat != null && !rat.IsDead)
            {
                rat.OnHeardSound(origin, soundLevel);
            }
        }
    }

    private void ApplyRecoil(float recoilAmount)
    {
        // Apply camera kick (if we have a camera controller)
        // CameraController.Instance?.ApplyRecoil(recoilAmount);

        // Controller vibration on mobile
#if UNITY_IOS || UNITY_ANDROID
        Handheld.Vibrate();
#endif
    }

    private void ShowHitMarker(bool isWeakPoint)
    {
        // Show UI hit marker
        // UIManager.Instance?.ShowHitMarker(isWeakPoint);
    }

    private void SpawnDamageNumber(Vector3 worldPosition, int damage, bool isCritical)
    {
        // Spawn floating damage number
        // ObjectPooler.Instance?.Spawn("DamageNumber", worldPosition, Quaternion.identity);
    }
}
