using UnityEngine;
using System.Collections;

/// <summary>
/// Master session manager for 10-15 minute gameplay sessions
/// Coordinates all systems to provide optimal short-session experience
/// </summary>
public class SessionManager : MonoBehaviour
{
    [Header("Session Configuration")]
    [SerializeField] private float optimalSessionLength = 720f; // 12 minutes
    [SerializeField] private float minSessionLength = 600f; // 10 minutes
    [SerializeField] private float maxSessionLength = 900f; // 15 minutes

    [Header("Session State")]
    [SerializeField] private bool sessionActive = false;
    [SerializeField] private float sessionStartTime = 0f;
    [SerializeField] private float totalSessionTime = 0f;
    [SerializeField] private int missionsCompletedThisSession = 0;
    [SerializeField] private int totalKillsThisSession = 0;
    [SerializeField] private int totalScoreThisSession = 0;

    [Header("Session Quality Tracking")]
    [SerializeField] private int perfectMissions = 0; // 3-star missions
    [SerializeField] private int headshotsThisSession = 0;
    [SerializeField] private float averageAccuracy = 0f;
    [SerializeField] private bool hasSeenTutorial = false;

    [Header("Session Flow Control")]
    [SerializeField] private bool autoChainMissions = true;
    [SerializeField] private float breakSuggestionTime = 900f; // Suggest break after 15 mins
    [SerializeField] private bool hasSuggestedBreak = false;

    [Header("Integration")]
    private CampaignMode campaignMode;
    private DailyChallengesSystem challengesSystem;
    private ImprovedBatterySystem batterySystem;
    private EnhancedAudioSystem audioSystem;
    private ParticleEffectsManager particleEffects;
    private ContractManager contractManager;
    private SaveManager saveManager;

    private void Awake()
    {
        ServiceLocator.Instance.Register<SessionManager>(this);
    }

    private void Start()
    {
        InitializeSystems();
        StartNewSession();
        SubscribeToEvents();
    }

    private void OnDestroy()
    {
        UnsubscribeFromEvents();
    }

    private void Update()
    {
        if (sessionActive)
        {
            totalSessionTime = Time.time - sessionStartTime;

            // Check for break suggestion
            if (!hasSuggestedBreak && totalSessionTime >= breakSuggestionTime)
            {
                SuggestBreak();
            }
        }
    }

    /// <summary>
    /// Initialize all session-related systems
    /// </summary>
    private void InitializeSystems()
    {
        campaignMode = CampaignMode.Instance;
        challengesSystem = FindObjectOfType<DailyChallengesSystem>();
        batterySystem = FindObjectOfType<ImprovedBatterySystem>();
        audioSystem = FindObjectOfType<EnhancedAudioSystem>();
        particleEffects = FindObjectOfType<ParticleEffectsManager>();
        contractManager = ContractManager.Instance;
        saveManager = SaveManager.Instance;

        Debug.Log("[SessionManager] Systems initialized");
    }

    /// <summary>
    /// Subscribe to game events
    /// </summary>
    private void SubscribeToEvents()
    {
        EventBus.Subscribe<MissionCompleteEvent>(OnMissionComplete);
        EventBus.Subscribe<RatKilledEvent>(OnRatKilled);
        EventBus.Subscribe<CampaignCompletedEvent>(OnCampaignCompleted);
        EventBus.Subscribe<ChallengeCompletedEvent>(OnChallengeCompleted);
    }

    /// <summary>
    /// Unsubscribe from game events
    /// </summary>
    private void UnsubscribeFromEvents()
    {
        EventBus.Unsubscribe<MissionCompleteEvent>(OnMissionComplete);
        EventBus.Unsubscribe<RatKilledEvent>(OnRatKilled);
        EventBus.Unsubscribe<CampaignCompletedEvent>(OnCampaignCompleted);
        EventBus.Unsubscribe<ChallengeCompletedEvent>(OnChallengeCompleted);
    }

    // === SESSION MANAGEMENT ===

    /// <summary>
    /// Start a new play session
    /// </summary>
    public void StartNewSession()
    {
        sessionActive = true;
        sessionStartTime = Time.time;
        totalSessionTime = 0f;
        missionsCompletedThisSession = 0;
        totalKillsThisSession = 0;
        totalScoreThisSession = 0;
        perfectMissions = 0;
        headshotsThisSession = 0;
        hasSuggestedBreak = false;

        Debug.Log("[SessionManager] New session started");

        // Start daily challenges system
        if (challengesSystem != null)
        {
            challengesSystem.StartSession();
        }

        // Show welcome message with daily challenges
        ShowWelcomeMessage();

        // Publish session start event
        EventBus.Publish(new SessionStartedEvent
        {
            sessionID = System.Guid.NewGuid().ToString()
        });
    }

    /// <summary>
    /// End current play session
    /// </summary>
    public void EndSession()
    {
        if (!sessionActive) return;

        sessionActive = false;

        Debug.Log($"[SessionManager] Session ended - Duration: {totalSessionTime / 60f:F1} mins");
        Debug.Log($"[SessionManager] Stats - Missions: {missionsCompletedThisSession}, Kills: {totalKillsThisSession}, Score: {totalScoreThisSession}");

        // Show session summary
        ShowSessionSummary();

        // Save progress
        if (saveManager != null)
        {
            saveManager.Save();
        }

        // End challenges system session
        if (challengesSystem != null)
        {
            challengesSystem.EndSession();
        }

        // Publish session end event
        EventBus.Publish(new SessionEndedEvent
        {
            duration = totalSessionTime,
            missionsCompleted = missionsCompletedThisSession,
            totalKills = totalKillsThisSession,
            totalScore = totalScoreThisSession,
            perfectMissions = perfectMissions
        });
    }

    /// <summary>
    /// Start quick play mode for optimal 10-15 minute session
    /// </summary>
    public void StartQuickPlay()
    {
        if (campaignMode != null)
        {
            campaignMode.StartQuickPlaySession();
            Debug.Log("[SessionManager] Started Quick Play mode");
        }
        else
        {
            Debug.LogError("[SessionManager] CampaignMode not available!");
        }
    }

    /// <summary>
    /// Start a specific campaign session
    /// </summary>
    public void StartCampaign(int sessionIndex)
    {
        if (campaignMode != null)
        {
            campaignMode.StartCampaignSession(sessionIndex);
            Debug.Log($"[SessionManager] Started Campaign session {sessionIndex}");
        }
        else
        {
            Debug.LogError("[SessionManager] CampaignMode not available!");
        }
    }

    // === EVENT HANDLERS ===

    private void OnMissionComplete(MissionCompleteEvent evt)
    {
        missionsCompletedThisSession++;
        totalScoreThisSession += evt.stars * 1000; // Estimate score from stars

        if (evt.stars == 3)
        {
            perfectMissions++;
        }

        UpdateAverageAccuracy(evt.accuracy);

        Debug.Log($"[SessionManager] Mission completed. Total: {missionsCompletedThisSession}");

        // Check if we're in optimal session range
        if (totalSessionTime >= minSessionLength && totalSessionTime <= maxSessionLength)
        {
            // Award session bonus for playing optimal length
            if (saveManager != null)
            {
                saveManager.AddExperience(50);
            }
        }
    }

    private void OnRatKilled(RatKilledEvent evt)
    {
        totalKillsThisSession++;

        if (evt.isHeadshot)
        {
            headshotsThisSession++;
        }
    }

    private void OnCampaignCompleted(CampaignCompletedEvent evt)
    {
        totalScoreThisSession += evt.totalScore;
        totalKillsThisSession += evt.totalKills;

        Debug.Log($"[SessionManager] Campaign completed: {evt.sessionName}");

        // Show campaign completion celebration
        ShowCampaignCompletion(evt);
    }

    private void OnChallengeCompleted(ChallengeCompletedEvent evt)
    {
        Debug.Log($"[SessionManager] Challenge completed: {evt.challenge.title}");

        // Show challenge completion notification
        ShowChallengeNotification(evt);
    }

    // === UI AND FEEDBACK ===

    private void ShowWelcomeMessage()
    {
        // TODO: Display welcome UI with:
        // - Daily challenges preview
        // - Daily streak status
        // - Quick Play button
        // - Featured campaign

        Debug.Log("=== WELCOME TO THERMAL HUNT ===");

        if (challengesSystem != null)
        {
            Debug.Log($"Daily Streak: {challengesSystem.DailyStreak} days");
            Debug.Log($"Active Challenges: {challengesSystem.ActiveChallenges.Count}");
        }
    }

    private void ShowSessionSummary()
    {
        // TODO: Display session summary UI with:
        // - Session duration
        // - Missions completed
        // - Total kills and headshots
        // - Perfect missions (3-stars)
        // - Challenges completed
        // - Experience gained

        Debug.Log("=== SESSION SUMMARY ===");
        Debug.Log($"Duration: {totalSessionTime / 60f:F1} minutes");
        Debug.Log($"Missions: {missionsCompletedThisSession} (Perfect: {perfectMissions})");
        Debug.Log($"Kills: {totalKillsThisSession} (Headshots: {headshotsThisSession})");
        Debug.Log($"Score: {totalScoreThisSession}");
        Debug.Log($"Avg Accuracy: {averageAccuracy:F1}%");
    }

    private void ShowCampaignCompletion(CampaignCompletedEvent evt)
    {
        Debug.Log($"=== CAMPAIGN COMPLETE: {evt.sessionName} ===");
        Debug.Log($"Duration: {evt.duration / 60f:F1} minutes");
        Debug.Log($"Score: {evt.totalScore}");
        Debug.Log($"Kills: {evt.totalKills}");
    }

    private void ShowChallengeNotification(ChallengeCompletedEvent evt)
    {
        Debug.Log($"CHALLENGE COMPLETE: {evt.challenge.title}");
        Debug.Log($"Reward: {evt.experienceRewarded} XP");
    }

    private void SuggestBreak()
    {
        hasSuggestedBreak = true;

        Debug.Log("[SessionManager] === BREAK SUGGESTION ===");
        Debug.Log($"You've been playing for {totalSessionTime / 60f:F1} minutes.");
        Debug.Log("Consider taking a break! Your progress has been saved.");

        // TODO: Display break suggestion UI
        // - Optional: Continue playing
        // - Optional: Take a break (end session)
    }

    /// <summary>
    /// Update running average accuracy
    /// </summary>
    private void UpdateAverageAccuracy(float missionAccuracy)
    {
        if (missionsCompletedThisSession == 1)
        {
            averageAccuracy = missionAccuracy;
        }
        else
        {
            averageAccuracy = (averageAccuracy * (missionsCompletedThisSession - 1) + missionAccuracy) / missionsCompletedThisSession;
        }
    }

    // === SESSION QUALITY METRICS ===

    /// <summary>
    /// Get session quality score (0-100)
    /// </summary>
    public float GetSessionQuality()
    {
        if (missionsCompletedThisSession == 0) return 0f;

        float qualityScore = 0f;

        // Missions completed (30 points)
        qualityScore += Mathf.Min(missionsCompletedThisSession * 10f, 30f);

        // Perfect missions (25 points)
        float perfectRatio = (float)perfectMissions / missionsCompletedThisSession;
        qualityScore += perfectRatio * 25f;

        // Accuracy (25 points)
        qualityScore += (averageAccuracy / 100f) * 25f;

        // Headshot ratio (20 points)
        if (totalKillsThisSession > 0)
        {
            float headshotRatio = (float)headshotsThisSession / totalKillsThisSession;
            qualityScore += headshotRatio * 20f;
        }

        return Mathf.Clamp(qualityScore, 0f, 100f);
    }

    /// <summary>
    /// Check if session is in optimal time range
    /// </summary>
    public bool IsOptimalSessionLength =>
        totalSessionTime >= minSessionLength && totalSessionTime <= maxSessionLength;

    // Properties
    public bool IsSessionActive => sessionActive;
    public float SessionDuration => totalSessionTime;
    public int MissionsCompleted => missionsCompletedThisSession;
    public int TotalKills => totalKillsThisSession;
    public int TotalScore => totalScoreThisSession;
    public int PerfectMissions => perfectMissions;
    public float AverageAccuracy => averageAccuracy;
    public int Headshots => headshotsThisSession;

    // Singleton accessor
    private static SessionManager instance;
    public static SessionManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<SessionManager>();
            }
            return instance;
        }
    }
}

// Events
public struct SessionStartedEvent
{
    public string sessionID;
}

public struct SessionEndedEvent
{
    public float duration;
    public int missionsCompleted;
    public int totalKills;
    public int totalScore;
    public int perfectMissions;
}
