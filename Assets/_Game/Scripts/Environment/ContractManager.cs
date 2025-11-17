using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Manages contract/mission selection and availability
/// Handles contract unlocking and progression
/// </summary>
public class ContractManager : Singleton<ContractManager>
{
    [Header("Contract Database")]
    [SerializeField] private List<ContractData> allContracts = new List<ContractData>();

    [Header("Current Contract")]
    [SerializeField] private ContractData currentContract;

    private SaveManager saveManager;

    // Properties
    public ContractData CurrentContract => currentContract;
    public List<ContractData> AllContracts => allContracts;

    protected override void Awake()
    {
        base.Awake();
        saveManager = SaveManager.Instance;

        // Register with ServiceLocator
        ServiceLocator.Instance.Register<ContractManager>(this);
    }

    private void Start()
    {
        LoadContractsFromResources();
    }

    private void LoadContractsFromResources()
    {
        // Load all ContractData assets from Resources
        ContractData[] contracts = Resources.LoadAll<ContractData>("Contracts");

        if (contracts.Length > 0)
        {
            allContracts.AddRange(contracts);
            Debug.Log($"[ContractManager] Loaded {contracts.Length} contracts from Resources");
        }
        else
        {
            Debug.LogWarning("[ContractManager] No contracts found in Resources/Contracts");
        }
    }

    // === Contract Selection ===

    public void SelectContract(ContractData contract)
    {
        if (contract == null)
        {
            Debug.LogError("[ContractManager] Cannot select null contract!");
            return;
        }

        if (!IsContractAvailable(contract))
        {
            Debug.LogWarning($"[ContractManager] Contract '{contract.contractName}' is not available!");
            return;
        }

        currentContract = contract;
        GameManager.Instance.SelectContract(contract);

        Debug.Log($"[ContractManager] Selected contract: {contract.contractName}");
    }

    public bool IsContractAvailable(ContractData contract)
    {
        if (contract == null) return false;
        if (saveManager == null) return false;

        // Check if unlocked
        string[] completedContracts = saveManager.CurrentSave.completedContracts.ToArray();
        return contract.IsUnlocked(saveManager.PlayerLevel, completedContracts);
    }

    public bool IsContractCompleted(ContractData contract)
    {
        if (contract == null || saveManager == null) return false;
        return saveManager.IsContractCompleted(contract.contractID);
    }

    public int GetContractStars(ContractData contract)
    {
        if (contract == null || saveManager == null) return 0;
        return saveManager.GetContractStars(contract.contractID);
    }

    // === Contract Queries ===

    public List<ContractData> GetAvailableContracts()
    {
        if (saveManager == null) return new List<ContractData>();

        return allContracts.Where(c => IsContractAvailable(c)).ToList();
    }

    public List<ContractData> GetContractsByDifficulty(ContractData.ContractDifficulty difficulty)
    {
        return allContracts.Where(c => c.difficulty == difficulty).ToList();
    }

    public List<ContractData> GetContractsByEnvironment(ContractData.EnvironmentType environment)
    {
        return allContracts.Where(c => c.environment == environment).ToList();
    }

    public ContractData GetContractByID(string contractID)
    {
        return allContracts.FirstOrDefault(c => c.contractID == contractID);
    }

    public int GetTotalStarsEarned()
    {
        if (saveManager == null) return 0;

        int totalStars = 0;
        foreach (var contract in allContracts)
        {
            totalStars += GetContractStars(contract);
        }

        return totalStars;
    }

    public int GetMaxPossibleStars()
    {
        return allContracts.Count * 3; // 3 stars per contract
    }

    public float GetCompletionPercentage()
    {
        int maxStars = GetMaxPossibleStars();
        if (maxStars == 0) return 0f;

        int earnedStars = GetTotalStarsEarned();
        return (float)earnedStars / maxStars;
    }

    // === Debug ===

    public void UnlockAllContracts()
    {
        if (saveManager == null) return;

        foreach (var contract in allContracts)
        {
            if (!IsContractCompleted(contract))
            {
                saveManager.CompleteContract(contract.contractID, 3);
            }
        }

        Debug.Log("[ContractManager] All contracts unlocked");
    }
}
