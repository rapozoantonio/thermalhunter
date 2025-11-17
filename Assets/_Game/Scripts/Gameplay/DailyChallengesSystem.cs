using UnityEngine;
using System.Collections.Generic;
using System;

/// <summary>
/// Daily challenges and session rewards system
/// Encourages 10-15 minute play sessions with daily objectives and quick bonuses
/// </summary>
public class DailyChallengesSystem : MonoBehaviour
{
    [System.Serializable]
    public class DailyChallenge
    {
        public string challengeID;
        public string title;
        public string description;
        public ChallengeType type;
        public int targetValue;
        public int currentProgress;
        public int rewardExperience;
        public int rewardCurrency;
        public bool isCompleted;
        public DateTime expiryDate;
    }

    [System.Serializable]
    public class SessionReward
    {
        public string rewardName;
        public int sessionMinutes;
        public int experienceBonus;
        public int currencyBonus;
        public bool isClaimed;
    }

    public enum ChallengeType
    {
        KillRats,
        Headshots,
        CompleteContracts,
        ThreeStarContracts,
        PlayTime,
        Accuracy,
        NoMisses,
        UnderTime,
        KillAlphas,
        KillNestMothers
    }

    [Header("Daily Challenges")]
    [SerializeField] private List<DailyChallenge> activeChallenges = new List<DailyChallenge>();
    [SerializeField] private int maxDailyChallenges = 3;

    [Header("Session Rewards")]
    [SerializeField] private List<SessionReward> sessionRewards = new List<SessionReward>();
    [SerializeField] private float currentSessionTime = 0f;
    [SerializeField] private bool sessionActive = false;

    [Header("Quick Play Bonuses")]
    [SerializeField] private int firstWinBonus = 500;
    [SerializeField] private int quickCompleteBonus = 300; // For <10 min sessions
    [SerializeField] private int perfectSessionBonus = 750; // No deaths, all objectives
    [SerializeField] private bool hasClaimedFirstWin = false;

    [Header("Streak System")]
    [SerializeField] private int consecutiveDaysPlayed = 0;
    [SerializeField] private int dailyStreakBonus = 100; // Per day streak
    [SerializeField] private DateTime lastPlayDate;

    private SaveManager saveManager;
    private System.Random random;

    private void Awake()
    {
        random = new System.Random();
        ServiceLocator.Instance.Register<DailyChallengesSystem>(this);
    }

    private void Start()
    {
        saveManager = SaveManager.Instance;
        LoadDailyChallenges();
        SetupSessionRewards();
        CheckDailyStreak();
        StartSession();

        // Subscribe to game events
        EventBus.Subscribe<MissionCompleteEvent>(OnMissionComplete);
        EventBus.Subscribe<RatKilledEvent>(OnRatKilled);
    }

    private void OnDestroy()
    {
        EventBus.Unsubscribe<MissionCompleteEvent>(OnMissionComplete);
        EventBus.Unsubscribe<RatKilledEvent>(OnRatKilled);
    }

    private void Update()
    {
        if (sessionActive)
        {
            currentSessionTime += Time.deltaTime;
            CheckSessionRewards();
        }
    }

    // === DAILY CHALLENGES ===

    /// <summary>
    /// Load or generate daily challenges
    /// </summary>
    private void LoadDailyChallenges()
    {
        // Check if we need to generate new challenges
        if (activeChallenges.Count == 0 || ShouldResetChallenges())
        {
            GenerateNewChallenges();
        }
        else
        {
            // Load existing challenges from save
            // TODO: Implement save/load for challenges
            Debug.Log($"[DailyChallenges] Loaded {activeChallenges.Count} active challenges");
        }
    }

    /// <summary>
    /// Check if challenges should be reset (new day)
    /// </summary>
    private bool ShouldResetChallenges()
    {
        if (activeChallenges.Count == 0) return true;

        DateTime now = DateTime.Now;
        foreach (var challenge in activeChallenges)
        {
            if (challenge.expiryDate < now)
                return true;
        }

        return false;
    }

    /// <summary>
    /// Generate new daily challenges
    /// </summary>
    private void GenerateNewChallenges()
    {
        activeChallenges.Clear();
        DateTime tomorrow = DateTime.Now.AddDays(1).Date; // Midnight tomorrow

        // Challenge 1: Kill rats (easy)
        activeChallenges.Add(new DailyChallenge
        {
            challengeID = "daily_kills_" + DateTime.Now.ToString("yyyyMMdd"),
            title = "Exterminator",
            description = "Eliminate 25 rats",
            type = ChallengeType.KillRats,
            targetValue = 25,
            currentProgress = 0,
            rewardExperience = 300,
            rewardCurrency = 500,
            isCompleted = false,
            expiryDate = tomorrow
        });

        // Challenge 2: Complete contracts (medium)
        activeChallenges.Add(new DailyChallenge
        {
            challengeID = "daily_contracts_" + DateTime.Now.ToString("yyyyMMdd"),
            title = "Contract Hunter",
            description = "Complete 3 contracts",
            type = ChallengeType.CompleteContracts,
            targetValue = 3,
            currentProgress = 0,
            rewardExperience = 400,
            rewardCurrency = 750,
            isCompleted = false,
            expiryDate = tomorrow
        });

        // Challenge 3: Random special challenge
        DailyChallenge special = GenerateRandomChallenge(tomorrow);
        activeChallenges.Add(special);

        Debug.Log($"[DailyChallenges] Generated {activeChallenges.Count} new challenges");

        // Publish event
        EventBus.Publish(new ChallengesRefreshedEvent { challenges = activeChallenges });
    }

    /// <summary>
    /// Generate a random special challenge
    /// </summary>
    private DailyChallenge GenerateRandomChallenge(DateTime expiry)
    {
        ChallengeType[] specialTypes = new[]
        {
            ChallengeType.Headshots,
            ChallengeType.ThreeStarContracts,
            ChallengeType.Accuracy,
            ChallengeType.UnderTime,
            ChallengeType.KillAlphas
        };

        ChallengeType type = specialTypes[random.Next(specialTypes.Length)];

        return type switch
        {
            ChallengeType.Headshots => new DailyChallenge
            {
                challengeID = "daily_headshots_" + DateTime.Now.ToString("yyyyMMdd"),
                title = "Sharpshooter",
                description = "Get 15 headshots",
                type = ChallengeType.Headshots,
                targetValue = 15,
                rewardExperience = 500,
                rewardCurrency = 1000,
                expiryDate = expiry
            },

            ChallengeType.ThreeStarContracts => new DailyChallenge
            {
                challengeID = "daily_3stars_" + DateTime.Now.ToString("yyyyMMdd"),
                title = "Perfect Performance",
                description = "Earn 3 stars on 2 contracts",
                type = ChallengeType.ThreeStarContracts,
                targetValue = 2,
                rewardExperience = 600,
                rewardCurrency = 1200,
                expiryDate = expiry
            },

            ChallengeType.Accuracy => new DailyChallenge
            {
                challengeID = "daily_accuracy_" + DateTime.Now.ToString("yyyyMMdd"),
                title = "Precision Strike",
                description = "Achieve 80% accuracy in a mission",
                type = ChallengeType.Accuracy,
                targetValue = 80,
                rewardExperience = 450,
                rewardCurrency = 900,
                expiryDate = expiry
            },

            ChallengeType.UnderTime => new DailyChallenge
            {
                challengeID = "daily_speed_" + DateTime.Now.ToString("yyyyMMdd"),
                title = "Speed Demon",
                description = "Complete a contract in under 3 minutes",
                type = ChallengeType.UnderTime,
                targetValue = 180,
                rewardExperience = 400,
                rewardCurrency = 800,
                expiryDate = expiry
            },

            ChallengeType.KillAlphas => new DailyChallenge
            {
                challengeID = "daily_alphas_" + DateTime.Now.ToString("yyyyMMdd"),
                title = "Alpha Hunter",
                description = "Eliminate 5 Alpha rats",
                type = ChallengeType.KillAlphas,
                targetValue = 5,
                rewardExperience = 550,
                rewardCurrency = 1100,
                expiryDate = expiry
            },

            _ => new DailyChallenge
            {
                challengeID = "daily_kills_" + DateTime.Now.ToString("yyyyMMdd"),
                title = "Exterminator",
                description = "Eliminate 25 rats",
                type = ChallengeType.KillRats,
                targetValue = 25,
                rewardExperience = 300,
                rewardCurrency = 500,
                expiryDate = expiry
            }
        };
    }

    /// <summary>
    /// Update challenge progress
    /// </summary>
    public void UpdateChallengeProgress(ChallengeType type, int value)
    {
        foreach (var challenge in activeChallenges)
        {
            if (challenge.type == type && !challenge.isCompleted)
            {
                challenge.currentProgress += value;

                if (challenge.currentProgress >= challenge.targetValue)
                {
                    CompleteChallenge(challenge);
                }
                else
                {
                    // Publish progress update
                    EventBus.Publish(new ChallengeProgressEvent
                    {
                        challengeID = challenge.challengeID,
                        progress = challenge.currentProgress,
                        target = challenge.targetValue
                    });
                }
            }
        }
    }

    /// <summary>
    /// Complete a daily challenge
    /// </summary>
    private void CompleteChallenge(DailyChallenge challenge)
    {
        challenge.isCompleted = true;
        challenge.currentProgress = challenge.targetValue;

        Debug.Log($"[DailyChallenges] Completed: {challenge.title}");

        // Award rewards
        if (saveManager != null)
        {
            saveManager.AddExperience(challenge.rewardExperience);
            // TODO: Add currency system
        }

        // Publish completion event
        EventBus.Publish(new ChallengeCompletedEvent
        {
            challenge = challenge,
            experienceRewarded = challenge.rewardExperience,
            currencyRewarded = challenge.rewardCurrency
        });
    }

    // === SESSION REWARDS ===

    /// <summary>
    /// Setup session reward milestones
    /// </summary>
    private void SetupSessionRewards()
    {
        sessionRewards = new List<SessionReward>
        {
            new SessionReward { rewardName = "5 Minute Warrior", sessionMinutes = 5, experienceBonus = 100, currencyBonus = 200 },
            new SessionReward { rewardName = "10 Minute Veteran", sessionMinutes = 10, experienceBonus = 250, currencyBonus = 500 },
            new SessionReward { rewardName = "15 Minute Legend", sessionMinutes = 15, experienceBonus = 500, currencyBonus = 1000 }
        };
    }

    /// <summary>
    /// Check and award session rewards
    /// </summary>
    private void CheckSessionRewards()
    {
        int sessionMinutes = Mathf.FloorToInt(currentSessionTime / 60f);

        foreach (var reward in sessionRewards)
        {
            if (!reward.isClaimed && sessionMinutes >= reward.sessionMinutes)
            {
                ClaimSessionReward(reward);
            }
        }
    }

    /// <summary>
    /// Claim session time reward
    /// </summary>
    private void ClaimSessionReward(SessionReward reward)
    {
        reward.isClaimed = true;

        Debug.Log($"[DailyChallenges] Session reward: {reward.rewardName}");

        if (saveManager != null)
        {
            saveManager.AddExperience(reward.experienceBonus);
        }

        EventBus.Publish(new SessionRewardEvent
        {
            rewardName = reward.rewardName,
            experienceBonus = reward.experienceBonus,
            currencyBonus = reward.currencyBonus
        });
    }

    /// <summary>
    /// Start a new play session
    /// </summary>
    public void StartSession()
    {
        sessionActive = true;
        currentSessionTime = 0f;
        hasClaimedFirstWin = false;

        // Reset session rewards
        foreach (var reward in sessionRewards)
        {
            reward.isClaimed = false;
        }

        Debug.Log("[DailyChallenges] Session started");
    }

    /// <summary>
    /// End play session
    /// </summary>
    public void EndSession()
    {
        sessionActive = false;
        Debug.Log($"[DailyChallenges] Session ended. Duration: {currentSessionTime / 60f:F1} minutes");
    }

    // === STREAK SYSTEM ===

    /// <summary>
    /// Check and update daily play streak
    /// </summary>
    private void CheckDailyStreak()
    {
        DateTime today = DateTime.Now.Date;

        if (lastPlayDate.Date == today)
        {
            // Already played today
            return;
        }
        else if (lastPlayDate.Date == today.AddDays(-1))
        {
            // Consecutive day
            consecutiveDaysPlayed++;
            AwardStreakBonus();
        }
        else
        {
            // Streak broken
            consecutiveDaysPlayed = 1;
        }

        lastPlayDate = today;
        Debug.Log($"[DailyChallenges] Daily streak: {consecutiveDaysPlayed} days");
    }

    /// <summary>
    /// Award bonus for daily streak
    /// </summary>
    private void AwardStreakBonus()
    {
        int bonus = consecutiveDaysPlayed * dailyStreakBonus;

        if (saveManager != null)
        {
            saveManager.AddExperience(bonus);
        }

        Debug.Log($"[DailyChallenges] Streak bonus: {bonus} XP for {consecutiveDaysPlayed} day streak");

        EventBus.Publish(new DailyStreakEvent
        {
            streakDays = consecutiveDaysPlayed,
            bonusExperience = bonus
        });
    }

    // === EVENT HANDLERS ===

    private void OnMissionComplete(MissionCompleteEvent evt)
    {
        UpdateChallengeProgress(ChallengeType.CompleteContracts, 1);

        if (evt.stars == 3)
        {
            UpdateChallengeProgress(ChallengeType.ThreeStarContracts, 1);
        }

        if (evt.accuracy >= 80)
        {
            UpdateChallengeProgress(ChallengeType.Accuracy, (int)evt.accuracy);
        }

        if (evt.timeRemaining > 120) // Completed with 2+ minutes left
        {
            UpdateChallengeProgress(ChallengeType.UnderTime, 1);
        }

        // First win bonus
        if (!hasClaimedFirstWin)
        {
            hasClaimedFirstWin = true;
            if (saveManager != null)
            {
                saveManager.AddExperience(firstWinBonus);
            }
            Debug.Log($"[DailyChallenges] First win bonus: {firstWinBonus} XP");
        }
    }

    private void OnRatKilled(RatKilledEvent evt)
    {
        UpdateChallengeProgress(ChallengeType.KillRats, 1);

        if (evt.isHeadshot)
        {
            UpdateChallengeProgress(ChallengeType.Headshots, 1);
        }

        if (evt.ratType == RatType.Alpha)
        {
            UpdateChallengeProgress(ChallengeType.KillAlphas, 1);
        }
        else if (evt.ratType == RatType.NestMother)
        {
            UpdateChallengeProgress(ChallengeType.KillNestMothers, 1);
        }
    }

    // Properties
    public List<DailyChallenge> ActiveChallenges => activeChallenges;
    public float CurrentSessionTime => currentSessionTime;
    public int DailyStreak => consecutiveDaysPlayed;
}

// Events
public struct ChallengesRefreshedEvent
{
    public List<DailyChallengesSystem.DailyChallenge> challenges;
}

public struct ChallengeProgressEvent
{
    public string challengeID;
    public int progress;
    public int target;
}

public struct ChallengeCompletedEvent
{
    public DailyChallengesSystem.DailyChallenge challenge;
    public int experienceRewarded;
    public int currencyRewarded;
}

public struct SessionRewardEvent
{
    public string rewardName;
    public int experienceBonus;
    public int currencyBonus;
}

public struct DailyStreakEvent
{
    public int streakDays;
    public int bonusExperience;
}

public struct MissionCompleteEvent
{
    public int stars;
    public float accuracy;
    public float timeRemaining;
}

public struct RatKilledEvent
{
    public RatType ratType;
    public bool isHeadshot;
}
