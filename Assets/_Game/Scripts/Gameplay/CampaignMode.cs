using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Campaign Mode system for chaining contracts into 10-15 minute gameplay sessions
/// Provides quick-play experience with progressive difficulty and session rewards
/// </summary>
public class CampaignMode : MonoBehaviour
{
    [System.Serializable]
    public class CampaignSession
    {
        public string sessionName;
        public List<ContractData> contracts = new List<ContractData>();
        public int totalTimeLimit; // Total time for all contracts
        public int sessionRewardMultiplier = 1;
        public int bonusExperience = 250;
    }

    [Header("Campaign Configuration")]
    [SerializeField] private List<CampaignSession> campaignSessions = new List<CampaignSession>();

    [Header("Session State")]
    [SerializeField] private CampaignSession currentSession;
    [SerializeField] private int currentContractIndex = 0;
    [SerializeField] private int totalSessionScore = 0;
    [SerializeField] private int totalSessionKills = 0;
    [SerializeField] private float sessionStartTime = 0f;
    [SerializeField] private bool isInCampaignMode = false;

    [Header("Session Rewards")]
    [SerializeField] private int quickSessionBonus = 500; // Bonus for completing under time
    [SerializeField] private int perfectSessionBonus = 1000; // Bonus for 3-starring all missions
    [SerializeField] private int streakMultiplier = 50; // Per consecutive contract

    private ContractManager contractManager;
    private SaveManager saveManager;

    private void Awake()
    {
        ServiceLocator.Instance.Register<CampaignMode>(this);
    }

    private void Start()
    {
        contractManager = ContractManager.Instance;
        saveManager = SaveManager.Instance;

        GenerateDefaultCampaigns();
    }

    /// <summary>
    /// Generates default campaign sessions for quick play
    /// </summary>
    private void GenerateDefaultCampaigns()
    {
        if (campaignSessions.Count > 0) return;

        // Campaign 1: Rookie Training (Easy, 10 minutes)
        CampaignSession rookie = new CampaignSession
        {
            sessionName = "Rookie Training",
            totalTimeLimit = 600, // 10 minutes
            sessionRewardMultiplier = 1,
            bonusExperience = 200
        };
        campaignSessions.Add(rookie);

        // Campaign 2: Urban Cleanup (Medium, 12 minutes)
        CampaignSession urban = new CampaignSession
        {
            sessionName = "Urban Cleanup",
            totalTimeLimit = 720, // 12 minutes
            sessionRewardMultiplier = 2,
            bonusExperience = 350
        };
        campaignSessions.Add(urban);

        // Campaign 3: Industrial Crisis (Hard, 15 minutes)
        CampaignSession industrial = new CampaignSession
        {
            sessionName = "Industrial Crisis",
            totalTimeLimit = 900, // 15 minutes
            sessionRewardMultiplier = 3,
            bonusExperience = 500
        };
        campaignSessions.Add(industrial);

        Debug.Log($"[CampaignMode] Generated {campaignSessions.Count} default campaigns");
    }

    /// <summary>
    /// Starts a campaign session with multiple contracts
    /// </summary>
    public void StartCampaignSession(int sessionIndex)
    {
        if (sessionIndex < 0 || sessionIndex >= campaignSessions.Count)
        {
            Debug.LogError($"[CampaignMode] Invalid session index: {sessionIndex}");
            return;
        }

        currentSession = campaignSessions[sessionIndex];
        currentContractIndex = 0;
        totalSessionScore = 0;
        totalSessionKills = 0;
        sessionStartTime = Time.time;
        isInCampaignMode = true;

        Debug.Log($"[CampaignMode] Starting campaign: {currentSession.sessionName}");

        // Start first contract
        StartNextContract();
    }

    /// <summary>
    /// Starts a quick play session with auto-selected contracts
    /// </summary>
    public void StartQuickPlaySession()
    {
        // Get available contracts based on player level
        List<ContractData> available = contractManager.GetAvailableContracts();

        if (available.Count == 0)
        {
            Debug.LogWarning("[CampaignMode] No available contracts for quick play!");
            return;
        }

        // Create dynamic session with 2-3 contracts
        int contractCount = Mathf.Min(3, available.Count);
        CampaignSession quickSession = new CampaignSession
        {
            sessionName = "Quick Play Session",
            contracts = available.OrderBy(x => Random.value).Take(contractCount).ToList(),
            totalTimeLimit = 600, // 10 minutes default
            sessionRewardMultiplier = 1,
            bonusExperience = 300
        };

        currentSession = quickSession;
        currentContractIndex = 0;
        totalSessionScore = 0;
        totalSessionKills = 0;
        sessionStartTime = Time.time;
        isInCampaignMode = true;

        Debug.Log($"[CampaignMode] Starting Quick Play with {contractCount} contracts");

        StartNextContract();
    }

    /// <summary>
    /// Starts the next contract in the campaign
    /// </summary>
    private void StartNextContract()
    {
        if (currentSession == null || currentSession.contracts.Count == 0)
        {
            Debug.LogError("[CampaignMode] No contracts in current session!");
            return;
        }

        if (currentContractIndex >= currentSession.contracts.Count)
        {
            CompleteCampaignSession();
            return;
        }

        ContractData nextContract = currentSession.contracts[currentContractIndex];
        contractManager.SelectContract(nextContract);

        Debug.Log($"[CampaignMode] Starting contract {currentContractIndex + 1}/{currentSession.contracts.Count}: {nextContract.contractName}");
    }

    /// <summary>
    /// Called when a contract is completed during campaign
    /// </summary>
    public void OnContractCompleted(int score, int kills, int stars)
    {
        if (!isInCampaignMode) return;

        totalSessionScore += score;
        totalSessionKills += kills;

        Debug.Log($"[CampaignMode] Contract completed. Session totals - Score: {totalSessionScore}, Kills: {totalSessionKills}");

        // Check if more contracts remain
        currentContractIndex++;

        if (currentContractIndex < currentSession.contracts.Count)
        {
            // Continue to next contract
            StartNextContract();
        }
        else
        {
            // All contracts completed
            CompleteCampaignSession();
        }
    }

    /// <summary>
    /// Completes the campaign session and awards bonuses
    /// </summary>
    private void CompleteCampaignSession()
    {
        if (currentSession == null) return;

        float sessionDuration = Time.time - sessionStartTime;
        bool completedUnderTime = sessionDuration < currentSession.totalTimeLimit;

        // Calculate bonuses
        int finalScore = totalSessionScore * currentSession.sessionRewardMultiplier;
        int bonusExp = currentSession.bonusExperience;

        if (completedUnderTime)
        {
            finalScore += quickSessionBonus;
            bonusExp += 100;
            Debug.Log("[CampaignMode] Quick completion bonus awarded!");
        }

        // Streak bonus
        int streakBonus = (currentSession.contracts.Count - 1) * streakMultiplier;
        finalScore += streakBonus;

        Debug.Log($"[CampaignMode] Campaign completed! Final Score: {finalScore}, Bonus XP: {bonusExp}");
        Debug.Log($"[CampaignMode] Total Kills: {totalSessionKills}, Duration: {sessionDuration:F1}s");

        // Award experience
        if (saveManager != null)
        {
            saveManager.AddExperience(bonusExp);
        }

        // Publish completion event
        EventBus.Publish(new CampaignCompletedEvent
        {
            sessionName = currentSession.sessionName,
            totalScore = finalScore,
            totalKills = totalSessionKills,
            duration = sessionDuration,
            contractsCompleted = currentSession.contracts.Count
        });

        // Reset campaign state
        isInCampaignMode = false;
        currentSession = null;
        currentContractIndex = 0;
    }

    /// <summary>
    /// Called when player fails a contract during campaign
    /// </summary>
    public void OnContractFailed()
    {
        if (!isInCampaignMode) return;

        Debug.Log("[CampaignMode] Contract failed. Ending campaign session.");

        // Award partial credit
        int partialScore = totalSessionScore / 2;
        int partialExp = currentSession.bonusExperience / 2;

        if (saveManager != null)
        {
            saveManager.AddExperience(partialExp);
        }

        // Publish failure event
        EventBus.Publish(new CampaignFailedEvent
        {
            sessionName = currentSession.sessionName,
            partialScore = partialScore,
            contractsCompleted = currentContractIndex,
            contractsFailed = currentSession.contracts.Count - currentContractIndex
        });

        // Reset campaign state
        isInCampaignMode = false;
        currentSession = null;
        currentContractIndex = 0;
    }

    // Properties
    public bool IsInCampaignMode => isInCampaignMode;
    public CampaignSession CurrentSession => currentSession;
    public int CurrentContractIndex => currentContractIndex;
    public int TotalSessionScore => totalSessionScore;
    public int TotalSessionKills => totalSessionKills;
    public float SessionDuration => isInCampaignMode ? Time.time - sessionStartTime : 0f;
    public List<CampaignSession> AvailableSessions => campaignSessions;
}

// Events
public struct CampaignCompletedEvent
{
    public string sessionName;
    public int totalScore;
    public int totalKills;
    public float duration;
    public int contractsCompleted;
}

public struct CampaignFailedEvent
{
    public string sessionName;
    public int partialScore;
    public int contractsCompleted;
    public int contractsFailed;
}
