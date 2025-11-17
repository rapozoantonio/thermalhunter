using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Analytics and telemetry manager
/// Tracks player behavior and game metrics
/// </summary>
public class AnalyticsManager : Singleton<AnalyticsManager>
{
    [Header("Settings")]
    [SerializeField] private bool enableAnalytics = true;
    [SerializeField] private bool debugMode = false;

    private Dictionary<string, int> sessionStats;

    protected override void Awake()
    {
        base.Awake();
        sessionStats = new Dictionary<string, int>();

        // Register with ServiceLocator
        ServiceLocator.Instance.Register<AnalyticsManager>(this);

        if (enableAnalytics)
        {
            InitializeAnalytics();
        }
    }

    private void InitializeAnalytics()
    {
        // Unity Analytics would be initialized here
        // Analytics.initializeOnStartup = true;

        Debug.Log("[AnalyticsManager] Analytics initialized");
    }

    // === Event Tracking ===

    public void TrackEvent(string eventName, Dictionary<string, object> parameters = null)
    {
        if (!enableAnalytics) return;

        if (debugMode)
        {
            string paramString = parameters != null ? string.Join(", ", parameters) : "none";
            Debug.Log($"[Analytics] Event: {eventName} | Params: {paramString}");
        }

        // Unity Analytics
        // Analytics.CustomEvent(eventName, parameters);

        // Increment session stats
        if (!sessionStats.ContainsKey(eventName))
        {
            sessionStats[eventName] = 0;
        }
        sessionStats[eventName]++;
    }

    // === Mission Events ===

    public void TrackMissionStart(string contractID, string difficulty)
    {
        TrackEvent(Constants.EVENT_MISSION_START, new Dictionary<string, object>
        {
            { "contract_id", contractID },
            { "difficulty", difficulty }
        });
    }

    public void TrackMissionComplete(string contractID, int stars, float time, float accuracy)
    {
        TrackEvent(Constants.EVENT_MISSION_COMPLETE, new Dictionary<string, object>
        {
            { "contract_id", contractID },
            { "stars", stars },
            { "completion_time", time },
            { "accuracy", accuracy }
        });
    }

    public void TrackMissionFail(string contractID, string reason)
    {
        TrackEvent(Constants.EVENT_MISSION_FAIL, new Dictionary<string, object>
        {
            { "contract_id", contractID },
            { "reason", reason }
        });
    }

    // === Combat Events ===

    public void TrackShot(bool hit, bool isHeadshot)
    {
        TrackEvent(Constants.EVENT_SHOT_FIRED, new Dictionary<string, object>
        {
            { "hit", hit },
            { "headshot", isHeadshot }
        });
    }

    public void TrackTargetHit(string targetType, bool isWeakPoint, int damage)
    {
        TrackEvent(Constants.EVENT_TARGET_HIT, new Dictionary<string, object>
        {
            { "target_type", targetType },
            { "weak_point", isWeakPoint },
            { "damage", damage }
        });
    }

    // === Progression Events ===

    public void TrackLevelUp(int newLevel)
    {
        TrackEvent(Constants.EVENT_LEVEL_UP, new Dictionary<string, object>
        {
            { "level", newLevel }
        });
    }

    public void TrackUnlock(string itemType, string itemID)
    {
        TrackEvent("item_unlocked", new Dictionary<string, object>
        {
            { "item_type", itemType },
            { "item_id", itemID }
        });
    }

    // === Monetization Events ===

    public void TrackAdWatched(string adType)
    {
        TrackEvent(Constants.EVENT_AD_WATCHED, new Dictionary<string, object>
        {
            { "ad_type", adType }
        });
    }

    public void TrackPurchase(string productID, decimal price)
    {
        if (!enableAnalytics) return;

        if (debugMode)
        {
            Debug.Log($"[Analytics] Purchase: {productID} - ${price}");
        }

        // Unity Analytics Transaction
        // Analytics.Transaction(productID, price, "USD");
    }

    public void TrackIAPAttempt(string productID, bool success)
    {
        TrackEvent("iap_attempt", new Dictionary<string, object>
        {
            { "product_id", productID },
            { "success", success }
        });
    }

    // === Retention Events ===

    public void TrackSessionStart()
    {
        TrackEvent("session_start", new Dictionary<string, object>
        {
            { "day_number", GetDaysSinceInstall() }
        });
    }

    public void TrackSessionEnd(float duration)
    {
        TrackEvent("session_end", new Dictionary<string, object>
        {
            { "duration", duration }
        });
    }

    public void TrackRetention(int daysSinceInstall)
    {
        TrackEvent("retention", new Dictionary<string, object>
        {
            { "days_since_install", daysSinceInstall }
        });
    }

    // === Helper Methods ===

    private int GetDaysSinceInstall()
    {
        if (!PlayerPrefs.HasKey("InstallDate"))
        {
            PlayerPrefs.SetString("InstallDate", System.DateTime.Now.ToString());
            PlayerPrefs.Save();
            return 0;
        }

        System.DateTime installDate = System.DateTime.Parse(PlayerPrefs.GetString("InstallDate"));
        System.TimeSpan timeSinceInstall = System.DateTime.Now - installDate;
        return (int)timeSinceInstall.TotalDays;
    }

    public Dictionary<string, int> GetSessionStats()
    {
        return new Dictionary<string, int>(sessionStats);
    }

    public void ClearSessionStats()
    {
        sessionStats.Clear();
    }

    // === Debugging ===

    public void SetDebugMode(bool enabled)
    {
        debugMode = enabled;
    }

    public void SetAnalyticsEnabled(bool enabled)
    {
        enableAnalytics = enabled;
    }
}
