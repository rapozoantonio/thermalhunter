using UnityEngine;
using System.Collections;

/// <summary>
/// Improved battery management system optimized for 10-15 minute sessions
/// Features battery upgrades, recharge mechanics, and session-friendly power management
/// </summary>
public class ImprovedBatterySystem : MonoBehaviour
{
    [Header("Battery Configuration")]
    [SerializeField] private float maxBatteryCapacity = 300f; // 5 minutes default
    [SerializeField] private float currentBattery = 300f;
    [SerializeField] private float drainRate = 1f; // Units per second
    [SerializeField] private bool isActive = false;

    [Header("Quick Recharge System")]
    [SerializeField] private bool enableQuickRecharge = true;
    [SerializeField] private float quickRechargeAmount = 60f; // 1 minute boost
    [SerializeField] private int quickRechargesAvailable = 2; // Per mission
    [SerializeField] private float quickRechargeCooldown = 30f; // Seconds
    [SerializeField] private float lastRechargeTime = 0f;

    [Header("Efficiency System")]
    [SerializeField] private float efficiencyLevel = 1f; // 1.0 = 100% efficiency
    [SerializeField] private float lowPowerThreshold = 0.25f; // 25%
    [SerializeField] private bool lowPowerMode = false;

    [Header("Upgrades")]
    [SerializeField] private int batteryUpgradeLevel = 0;
    [SerializeField] private float[] capacityUpgrades = { 300f, 420f, 540f, 660f, 900f }; // 5-15 mins
    [SerializeField] private float[] efficiencyUpgrades = { 1f, 0.9f, 0.8f, 0.7f, 0.6f }; // Lower = better

    [Header("Power Saving Modes")]
    [SerializeField] private bool autoPowerSaving = true;
    [SerializeField] private float powerSavingDrainMultiplier = 0.5f;

    [Header("Warning System")]
    [SerializeField] private float criticalBatteryThreshold = 30f; // 30 seconds
    [SerializeField] private float warningBatteryThreshold = 60f; // 1 minute
    [SerializeField] private bool hasShownWarning = false;
    [SerializeField] private bool hasShownCritical = false;

    private ThermalRenderer thermalRenderer;

    private void Awake()
    {
        ServiceLocator.Instance.Register<ImprovedBatterySystem>(this);
    }

    private void Start()
    {
        thermalRenderer = FindObjectOfType<ThermalRenderer>();
        LoadUpgrades();
        ResetForNewMission();
    }

    private void Update()
    {
        if (isActive)
        {
            DrainBattery();
            CheckBatteryWarnings();
        }
    }

    /// <summary>
    /// Load battery upgrades from save system
    /// </summary>
    private void LoadUpgrades()
    {
        SaveManager saveManager = SaveManager.Instance;
        if (saveManager != null)
        {
            // TODO: Load upgrade level from save
            // batteryUpgradeLevel = saveManager.GetBatteryUpgradeLevel();
        }

        ApplyUpgrades();
    }

    /// <summary>
    /// Apply current upgrade level to battery stats
    /// </summary>
    private void ApplyUpgrades()
    {
        if (batteryUpgradeLevel >= 0 && batteryUpgradeLevel < capacityUpgrades.Length)
        {
            maxBatteryCapacity = capacityUpgrades[batteryUpgradeLevel];
            efficiencyLevel = efficiencyUpgrades[batteryUpgradeLevel];

            Debug.Log($"[BatterySystem] Applied upgrades - Capacity: {maxBatteryCapacity}s, Efficiency: {efficiencyLevel * 100}%");
        }
    }

    /// <summary>
    /// Reset battery for new mission
    /// </summary>
    public void ResetForNewMission()
    {
        currentBattery = maxBatteryCapacity;
        quickRechargesAvailable = 2;
        lowPowerMode = false;
        hasShownWarning = false;
        hasShownCritical = false;
        lastRechargeTime = -quickRechargeCooldown; // Allow immediate first recharge

        Debug.Log($"[BatterySystem] Reset for new mission - Capacity: {maxBatteryCapacity}s");
    }

    /// <summary>
    /// Activate thermal vision and start draining battery
    /// </summary>
    public void ActivateThermal()
    {
        isActive = true;
        Debug.Log("[BatterySystem] Thermal vision activated");
    }

    /// <summary>
    /// Deactivate thermal vision and stop draining
    /// </summary>
    public void DeactivateThermal()
    {
        isActive = false;
        Debug.Log("[BatterySystem] Thermal vision deactivated");
    }

    /// <summary>
    /// Drain battery over time
    /// </summary>
    private void DrainBattery()
    {
        if (currentBattery <= 0)
        {
            currentBattery = 0;
            OnBatteryDepleted();
            return;
        }

        float effectiveDrainRate = drainRate * efficiencyLevel;

        // Apply power saving mode
        if (lowPowerMode || (autoPowerSaving && currentBattery < lowPowerThreshold * maxBatteryCapacity))
        {
            effectiveDrainRate *= powerSavingDrainMultiplier;
            lowPowerMode = true;
        }

        currentBattery -= effectiveDrainRate * Time.deltaTime;
        currentBattery = Mathf.Max(0f, currentBattery);

        // Publish battery update event
        EventBus.Publish(new BatteryUpdateEvent
        {
            currentBattery = currentBattery,
            maxBattery = maxBatteryCapacity,
            percentage = BatteryPercentage
        });
    }

    /// <summary>
    /// Quick recharge battery (limited uses per mission)
    /// </summary>
    public bool QuickRecharge()
    {
        if (!enableQuickRecharge)
        {
            Debug.LogWarning("[BatterySystem] Quick recharge is disabled");
            return false;
        }

        if (quickRechargesAvailable <= 0)
        {
            Debug.LogWarning("[BatterySystem] No quick recharges remaining");
            return false;
        }

        if (Time.time - lastRechargeTime < quickRechargeCooldown)
        {
            float remaining = quickRechargeCooldown - (Time.time - lastRechargeTime);
            Debug.LogWarning($"[BatterySystem] Quick recharge on cooldown: {remaining:F1}s remaining");
            return false;
        }

        // Perform recharge
        currentBattery += quickRechargeAmount;
        currentBattery = Mathf.Min(currentBattery, maxBatteryCapacity);
        quickRechargesAvailable--;
        lastRechargeTime = Time.time;

        Debug.Log($"[BatterySystem] Quick recharge used. Remaining: {quickRechargesAvailable}");

        // Publish recharge event
        EventBus.Publish(new BatteryRechargedEvent
        {
            rechargeAmount = quickRechargeAmount,
            rechargesRemaining = quickRechargesAvailable
        });

        return true;
    }

    /// <summary>
    /// Watch ad to fully recharge battery
    /// </summary>
    public void WatchAdForRecharge()
    {
        // TODO: Integrate with ad system
        Debug.Log("[BatterySystem] Watching ad for full recharge...");

        // For now, just do a full recharge
        FullRecharge();
    }

    /// <summary>
    /// Fully recharge battery
    /// </summary>
    public void FullRecharge()
    {
        currentBattery = maxBatteryCapacity;
        lowPowerMode = false;
        hasShownWarning = false;
        hasShownCritical = false;

        Debug.Log("[BatterySystem] Battery fully recharged");

        EventBus.Publish(new BatteryRechargedEvent
        {
            rechargeAmount = maxBatteryCapacity,
            rechargesRemaining = quickRechargesAvailable
        });
    }

    /// <summary>
    /// Toggle power saving mode
    /// </summary>
    public void TogglePowerSavingMode()
    {
        lowPowerMode = !lowPowerMode;
        Debug.Log($"[BatterySystem] Power saving mode: {(lowPowerMode ? "ON" : "OFF")}");

        EventBus.Publish(new PowerSavingModeEvent { enabled = lowPowerMode });
    }

    /// <summary>
    /// Check and display battery warnings
    /// </summary>
    private void CheckBatteryWarnings()
    {
        // Critical warning
        if (currentBattery <= criticalBatteryThreshold && !hasShownCritical)
        {
            hasShownCritical = true;
            EventBus.Publish(new BatteryWarningEvent
            {
                level = WarningLevel.Critical,
                remainingSeconds = currentBattery
            });

            Debug.LogWarning("[BatterySystem] CRITICAL: Battery very low!");
        }
        // Low warning
        else if (currentBattery <= warningBatteryThreshold && !hasShownWarning)
        {
            hasShownWarning = true;
            EventBus.Publish(new BatteryWarningEvent
            {
                level = WarningLevel.Low,
                remainingSeconds = currentBattery
            });

            Debug.LogWarning("[BatterySystem] WARNING: Battery low");
        }
    }

    /// <summary>
    /// Called when battery is completely depleted
    /// </summary>
    private void OnBatteryDepleted()
    {
        if (isActive)
        {
            isActive = false;
            DeactivateThermal();

            EventBus.Publish(new BatteryDepletedEvent { });

            Debug.LogError("[BatterySystem] Battery depleted! Thermal vision disabled.");
        }
    }

    /// <summary>
    /// Upgrade battery capacity and efficiency
    /// </summary>
    public bool UpgradeBattery()
    {
        if (batteryUpgradeLevel >= capacityUpgrades.Length - 1)
        {
            Debug.LogWarning("[BatterySystem] Battery already at max upgrade level");
            return false;
        }

        batteryUpgradeLevel++;
        ApplyUpgrades();

        // Save upgrade
        // TODO: saveManager.SetBatteryUpgradeLevel(batteryUpgradeLevel);

        Debug.Log($"[BatterySystem] Battery upgraded to level {batteryUpgradeLevel}");

        EventBus.Publish(new BatteryUpgradedEvent
        {
            newLevel = batteryUpgradeLevel,
            newCapacity = maxBatteryCapacity,
            newEfficiency = efficiencyLevel
        });

        return true;
    }

    /// <summary>
    /// Calculate time remaining in minutes
    /// </summary>
    public float TimeRemainingMinutes => currentBattery / 60f;

    /// <summary>
    /// Get battery percentage (0-100)
    /// </summary>
    public float BatteryPercentage => (currentBattery / maxBatteryCapacity) * 100f;

    /// <summary>
    /// Check if battery is low
    /// </summary>
    public bool IsLowBattery => currentBattery <= warningBatteryThreshold;

    /// <summary>
    /// Check if battery is critical
    /// </summary>
    public bool IsCriticalBattery => currentBattery <= criticalBatteryThreshold;

    /// <summary>
    /// Can quick recharge right now?
    /// </summary>
    public bool CanQuickRecharge =>
        enableQuickRecharge &&
        quickRechargesAvailable > 0 &&
        Time.time - lastRechargeTime >= quickRechargeCooldown;

    // Properties
    public float CurrentBattery => currentBattery;
    public float MaxBattery => maxBatteryCapacity;
    public int QuickRechargesAvailable => quickRechargesAvailable;
    public float QuickRechargeCooldownRemaining =>
        Mathf.Max(0f, quickRechargeCooldown - (Time.time - lastRechargeTime));
    public bool IsActive => isActive;
    public bool IsInPowerSavingMode => lowPowerMode;
    public int UpgradeLevel => batteryUpgradeLevel;
}

// Events
public struct BatteryUpdateEvent
{
    public float currentBattery;
    public float maxBattery;
    public float percentage;
}

public struct BatteryWarningEvent
{
    public WarningLevel level;
    public float remainingSeconds;
}

public struct BatteryDepletedEvent { }

public struct BatteryRechargedEvent
{
    public float rechargeAmount;
    public int rechargesRemaining;
}

public struct PowerSavingModeEvent
{
    public bool enabled;
}

public struct BatteryUpgradedEvent
{
    public int newLevel;
    public float newCapacity;
    public float newEfficiency;
}

public enum WarningLevel
{
    Low,
    Critical
}
