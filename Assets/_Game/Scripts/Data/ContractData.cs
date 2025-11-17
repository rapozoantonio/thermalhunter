using UnityEngine;

/// <summary>
/// ScriptableObject for mission/contract configuration
/// Defines objectives, rewards, and environment settings
/// </summary>
[CreateAssetMenu(fileName = "NewContract", menuName = "ThermalHunt/Contract Data")]
public class ContractData : ScriptableObject
{
    [Header("Contract Info")]
    public string contractID;
    public string contractName;
    [TextArea(3, 5)] public string description;
    public string clientName;
    public Sprite locationImage;
    public ContractDifficulty difficulty;

    [Header("Objectives")]
    public int targetCount = 10;
    public float timeLimit = 300f; // 5 minutes
    public int ammunitionAllowed = 15;
    public bool allowResupply = false;

    [Header("Environment")]
    public EnvironmentType environment;
    public string sceneName;
    public Vector3 spawnPosition;
    public float ambientNoise = 0.1f;
    public bool hasDecoys = false;
    public bool hasNonTargets = false; // Cats/birds to avoid

    [Header("Target Distribution")]
    public int droneRatCount = 8;
    public int nestMotherCount = 1;
    public int alphaRatCount = 1;

    [Header("Rewards")]
    public int baseReward = 1000;
    public int oneStarThreshold = 5;
    public int twoStarThreshold = 8;
    public int threeStarThreshold = 10;

    [Header("Experience")]
    public int experienceReward = 100;
    public int bonusExperiencePerStar = 50;

    [Header("Unlocks")]
    public ContractData nextContract;
    public WeaponData unlockedWeapon;
    public ScopeData unlockedScope;

    [Header("Requirements")]
    public int requiredLevel = 1;
    public ContractData[] prerequisiteContracts;

    // Enums
    public enum ContractDifficulty { Easy, Medium, Hard, Expert }
    public enum EnvironmentType { Barn, Warehouse, Field, Garden, Sewer, Industrial }

    // Properties
    public int TotalTargets => droneRatCount + nestMotherCount + alphaRatCount;

    // Validation
    private void OnValidate()
    {
        if (string.IsNullOrEmpty(contractID))
        {
            contractID = name;
        }

        // Ensure thresholds make sense
        oneStarThreshold = Mathf.Min(oneStarThreshold, targetCount);
        twoStarThreshold = Mathf.Clamp(twoStarThreshold, oneStarThreshold, targetCount);
        threeStarThreshold = Mathf.Clamp(threeStarThreshold, twoStarThreshold, targetCount);

        // Ensure target distribution matches total
        int distributionTotal = droneRatCount + nestMotherCount + alphaRatCount;
        if (distributionTotal != targetCount)
        {
            Debug.LogWarning($"[ContractData] '{contractName}' target distribution ({distributionTotal}) doesn't match targetCount ({targetCount})");
        }
    }

    // Helper methods
    public int CalculateStars(int killedCount)
    {
        if (killedCount >= threeStarThreshold) return 3;
        if (killedCount >= twoStarThreshold) return 2;
        if (killedCount >= oneStarThreshold) return 1;
        return 0;
    }

    public int CalculateTotalExperience(int stars)
    {
        return experienceReward + (stars * bonusExperiencePerStar);
    }

    public bool IsUnlocked(int playerLevel, string[] completedContractIDs)
    {
        // Check level requirement
        if (playerLevel < requiredLevel)
            return false;

        // Check prerequisite contracts
        if (prerequisiteContracts != null && prerequisiteContracts.Length > 0)
        {
            foreach (var prereq in prerequisiteContracts)
            {
                if (prereq == null) continue;

                bool isCompleted = false;
                foreach (var completed in completedContractIDs)
                {
                    if (completed == prereq.contractID)
                    {
                        isCompleted = true;
                        break;
                    }
                }

                if (!isCompleted)
                    return false;
            }
        }

        return true;
    }
}
