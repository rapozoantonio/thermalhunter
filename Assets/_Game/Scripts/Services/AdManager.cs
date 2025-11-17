using UnityEngine;

/// <summary>
/// Cross-platform ad monetization manager
/// Handles rewarded and interstitial ads with Unity Ads
/// </summary>
public class AdManager : Singleton<AdManager>
{
    [Header("Ad Settings")]
    [SerializeField] private bool testMode = true;
    [SerializeField] private int sessionsUntilInterstitial = 3;
    [SerializeField] private int maxAdsPerSession = 5;

    private int sessionAdsWatched = 0;
    private int sessionCount = 0;
    private System.Action<bool> onRewardedAdComplete;
    private bool isInitialized = false;

    protected override void Awake()
    {
        base.Awake();

#if !UNITY_STANDALONE
        InitializeAds();
#endif

        // Register with ServiceLocator
        ServiceLocator.Instance.Register<AdManager>(this);
    }

    private void InitializeAds()
    {
        // Unity Ads initialization would go here
        // Advertisement.Initialize(Constants.UNITY_GAME_ID, testMode, this);

        Debug.Log($"[AdManager] Initializing ads - Test Mode: {testMode}");

        // Simulate initialization for now
        isInitialized = true;
    }

    // === Rewarded Ads ===

    public void ShowRewardedAd(System.Action<bool> onComplete, string placement = "")
    {
#if UNITY_STANDALONE
        // No ads on PC - give reward directly
        Debug.Log("[AdManager] PC platform - granting reward without ad");
        onComplete?.Invoke(true);
        return;
#endif

        // Check if ads are removed via IAP
        if (SaveManager.Instance?.CurrentSave?.adsRemoved == true)
        {
            Debug.Log("[AdManager] Ads removed via purchase - granting reward");
            onComplete?.Invoke(true);
            return;
        }

        // Check ad limit
        if (sessionAdsWatched >= maxAdsPerSession)
        {
            Debug.LogWarning("[AdManager] Ad limit reached for this session");
            onComplete?.Invoke(false);
            return;
        }

        if (!isInitialized)
        {
            Debug.LogError("[AdManager] Ads not initialized!");
            onComplete?.Invoke(false);
            return;
        }

        onRewardedAdComplete = onComplete;

        // Unity Ads implementation would go here
        // Advertisement.Load(Constants.REWARDED_AD_UNIT, this);

        // Simulate ad for development
        SimulateRewardedAd();
    }

    private void SimulateRewardedAd()
    {
        Debug.Log("[AdManager] Simulating rewarded ad...");

        // Simulate ad watch delay
        this.InvokeDelayed(() =>
        {
            OnRewardedAdComplete(true);
        }, 2f);
    }

    private void OnRewardedAdComplete(bool success)
    {
        if (success)
        {
            sessionAdsWatched++;

            // Track analytics
            AnalyticsManager analytics = ServiceLocator.Instance.TryGet<AnalyticsManager>();
            analytics?.TrackAdWatched("rewarded");

            // Publish event
            EventBus.Publish(new AdWatchedEvent
            {
                AdType = "rewarded",
                Success = true
            });

            Debug.Log($"[AdManager] Rewarded ad completed - Total this session: {sessionAdsWatched}");
        }

        onRewardedAdComplete?.Invoke(success);
        onRewardedAdComplete = null;
    }

    // === Interstitial Ads ===

    public void ShowInterstitialAd()
    {
#if UNITY_STANDALONE
        return;
#endif

        // Check if ads are removed
        if (SaveManager.Instance?.CurrentSave?.adsRemoved == true)
        {
            Debug.Log("[AdManager] Ads removed via purchase");
            return;
        }

        sessionCount++;

        if (sessionCount >= sessionsUntilInterstitial)
        {
            sessionCount = 0;

            if (!isInitialized)
            {
                Debug.LogError("[AdManager] Ads not initialized!");
                return;
            }

            // Unity Ads implementation would go here
            // Advertisement.Load(Constants.INTERSTITIAL_AD_UNIT, this);

            Debug.Log("[AdManager] Showing interstitial ad");

            // Track analytics
            AnalyticsManager analytics = ServiceLocator.Instance.TryGet<AnalyticsManager>();
            analytics?.TrackAdWatched("interstitial");
        }
    }

    // === Utility ===

    public bool CanShowAd()
    {
        if (SaveManager.Instance?.CurrentSave?.adsRemoved == true)
        {
            return false;
        }

        if (sessionAdsWatched >= maxAdsPerSession)
        {
            return false;
        }

        return isInitialized;
    }

    public int GetRemainingAds()
    {
        return Mathf.Max(0, maxAdsPerSession - sessionAdsWatched);
    }

    public void ResetSessionCount()
    {
        sessionAdsWatched = 0;
        sessionCount = 0;
        Debug.Log("[AdManager] Session counts reset");
    }

    // === IUnityAdsInitializationListener (would be implemented) ===
    // public void OnInitializationComplete() { }
    // public void OnInitializationFailed(UnityAdsInitializationError error, string message) { }

    // === IUnityAdsLoadListener (would be implemented) ===
    // public void OnUnityAdsAdLoaded(string placementId) { }
    // public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message) { }

    // === IUnityAdsShowListener (would be implemented) ===
    // public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState) { }
    // public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message) { }
    // public void OnUnityAdsShowStart(string placementId) { }
    // public void OnUnityAdsShowClick(string placementId) { }
}
