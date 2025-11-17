using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Contract generation helper for creating diverse mission content
/// Creates 5 recommended contracts optimized for 10-15 minute play sessions
/// </summary>
#if UNITY_EDITOR
public class ContractGenerator : EditorWindow
{
    [MenuItem("Thermal Hunt/Generate Contracts")]
    public static void ShowWindow()
    {
        GetWindow<ContractGenerator>("Contract Generator");
    }

    private Vector2 scrollPosition;

    private void OnGUI()
    {
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        GUILayout.Label("Contract Generator", EditorStyles.boldLabel);
        GUILayout.Label("Generate 5 diverse contracts for 10-15 minute gameplay sessions", EditorStyles.helpBox);

        GUILayout.Space(10);

        if (GUILayout.Button("Generate All 5 Contracts", GUILayout.Height(40)))
        {
            GenerateAllContracts();
        }

        GUILayout.Space(20);
        GUILayout.Label("Contract Blueprints:", EditorStyles.boldLabel);

        DrawContractBlueprint("Contract 1: Barn Basics (Tutorial)", GetContract1());
        DrawContractBlueprint("Contract 2: Warehouse Cleanup", GetContract2());
        DrawContractBlueprint("Contract 3: Garden Infestation", GetContract3());
        DrawContractBlueprint("Contract 4: Industrial Complex", GetContract4());
        DrawContractBlueprint("Contract 5: Sewer System Crisis", GetContract5());

        EditorGUILayout.EndScrollView();
    }

    private void DrawContractBlueprint(string title, ContractBlueprint blueprint)
    {
        EditorGUILayout.BeginVertical("box");

        EditorGUILayout.LabelField(title, EditorStyles.boldLabel);
        EditorGUILayout.LabelField("Environment:", blueprint.environment.ToString());
        EditorGUILayout.LabelField("Difficulty:", blueprint.difficulty.ToString());
        EditorGUILayout.LabelField("Targets:", $"{blueprint.targetCount} rats");
        EditorGUILayout.LabelField("Time:", $"{blueprint.timeLimit}s ({blueprint.timeLimit / 60}min)");
        EditorGUILayout.LabelField("Ammo:", $"{blueprint.ammunition} rounds");
        EditorGUILayout.LabelField("Description:", blueprint.description);

        if (GUILayout.Button($"Create {title}"))
        {
            CreateContract(blueprint);
        }

        EditorGUILayout.EndVertical();
        GUILayout.Space(5);
    }

    private void GenerateAllContracts()
    {
        CreateContract(GetContract1());
        CreateContract(GetContract2());
        CreateContract(GetContract3());
        CreateContract(GetContract4());
        CreateContract(GetContract5());

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("[ContractGenerator] Created all 5 contracts!");
        EditorUtility.DisplayDialog("Success", "Created all 5 contracts in Resources/Contracts/", "OK");
    }

    private void CreateContract(ContractBlueprint blueprint)
    {
        // Create ScriptableObject
        ContractData contract = ScriptableObject.CreateInstance<ContractData>();

        // Set basic info
        contract.contractID = blueprint.contractID;
        contract.contractName = blueprint.contractName;
        contract.description = blueprint.description;
        contract.clientName = blueprint.clientName;
        contract.difficulty = blueprint.difficulty;

        // Set objectives
        contract.targetCount = blueprint.targetCount;
        contract.timeLimit = blueprint.timeLimit;
        contract.ammunitionAllowed = blueprint.ammunition;
        contract.allowResupply = blueprint.allowResupply;

        // Set environment
        contract.environment = blueprint.environment;
        contract.sceneName = blueprint.sceneName;

        // Set target distribution
        contract.droneRatCount = blueprint.droneRatCount;
        contract.nestMotherCount = blueprint.nestMotherCount;
        contract.alphaRatCount = blueprint.alphaRatCount;

        // Set rewards
        contract.baseReward = blueprint.baseReward;
        contract.oneStarThreshold = blueprint.oneStarThreshold;
        contract.twoStarThreshold = blueprint.twoStarThreshold;
        contract.threeStarThreshold = blueprint.threeStarThreshold;

        // Set experience
        contract.experienceReward = blueprint.experienceReward;
        contract.bonusExperiencePerStar = blueprint.bonusExperiencePerStar;

        // Set requirements
        contract.requiredLevel = blueprint.requiredLevel;

        // Save asset
        string path = $"Assets/_Game/Resources/Contracts/{blueprint.contractID}.asset";
        AssetDatabase.CreateAsset(contract, path);

        Debug.Log($"[ContractGenerator] Created contract: {blueprint.contractName} at {path}");
    }

    // === CONTRACT BLUEPRINTS ===

    private ContractBlueprint GetContract1()
    {
        return new ContractBlueprint
        {
            contractID = "contract_barn_basics",
            contractName = "Barn Basics",
            description = "A local farmer needs help clearing rats from his old barn. Simple job, good for beginners. Take your time and focus on accuracy.",
            clientName = "Farmer Johnson",
            difficulty = ContractData.ContractDifficulty.Easy,
            environment = ContractData.EnvironmentType.Barn,
            sceneName = "BarnScene",
            targetCount = 8,
            timeLimit = 240f, // 4 minutes
            ammunition = 12,
            allowResupply = true,
            droneRatCount = 7,
            nestMotherCount = 0,
            alphaRatCount = 1,
            baseReward = 500,
            oneStarThreshold = 4,
            twoStarThreshold = 6,
            threeStarThreshold = 8,
            experienceReward = 100,
            bonusExperiencePerStar = 25,
            requiredLevel = 1
        };
    }

    private ContractBlueprint GetContract2()
    {
        return new ContractBlueprint
        {
            contractID = "contract_warehouse_cleanup",
            contractName = "Warehouse Cleanup",
            description = "A shipping warehouse has a severe rat problem. Multiple targets, limited time. The warehouse foreman is offering good pay for quick results.",
            clientName = "Warehouse Manager",
            difficulty = ContractData.ContractDifficulty.Medium,
            environment = ContractData.EnvironmentType.Warehouse,
            sceneName = "WarehouseScene",
            targetCount = 12,
            timeLimit = 300f, // 5 minutes
            ammunition = 18,
            allowResupply = false,
            droneRatCount = 9,
            nestMotherCount = 1,
            alphaRatCount = 2,
            baseReward = 1200,
            oneStarThreshold = 7,
            twoStarThreshold = 10,
            threeStarThreshold = 12,
            experienceReward = 200,
            bonusExperiencePerStar = 50,
            requiredLevel = 3
        };
    }

    private ContractBlueprint GetContract3()
    {
        return new ContractBlueprint
        {
            contractID = "contract_garden_infestation",
            contractName = "Garden Infestation",
            description = "A community garden is overrun with rats at night. They're damaging crops and scaring residents. Silent operation preferred - use your suppressed rifle.",
            clientName = "Community Board",
            difficulty = ContractData.ContractDifficulty.Medium,
            environment = ContractData.EnvironmentType.Garden,
            sceneName = "GardenScene",
            targetCount = 10,
            timeLimit = 360f, // 6 minutes
            ammunition = 15,
            allowResupply = true,
            droneRatCount = 8,
            nestMotherCount = 1,
            alphaRatCount = 1,
            baseReward = 1000,
            oneStarThreshold = 6,
            twoStarThreshold = 8,
            threeStarThreshold = 10,
            experienceReward = 180,
            bonusExperiencePerStar = 40,
            requiredLevel = 2
        };
    }

    private ContractBlueprint GetContract4()
    {
        return new ContractBlueprint
        {
            contractID = "contract_industrial_complex",
            contractName = "Industrial Crisis",
            description = "An abandoned industrial complex has become a rat haven. Multiple nests confirmed. This is a large-scale operation with significant rewards for the skilled hunter.",
            clientName = "City Health Department",
            difficulty = ContractData.ContractDifficulty.Hard,
            environment = ContractData.EnvironmentType.Industrial,
            sceneName = "IndustrialScene",
            targetCount = 15,
            timeLimit = 420f, // 7 minutes
            ammunition = 20,
            allowResupply = false,
            droneRatCount = 11,
            nestMotherCount = 2,
            alphaRatCount = 2,
            baseReward = 2000,
            oneStarThreshold = 10,
            twoStarThreshold = 13,
            threeStarThreshold = 15,
            experienceReward = 350,
            bonusExperiencePerStar = 75,
            requiredLevel = 5
        };
    }

    private ContractBlueprint GetContract5()
    {
        return new ContractBlueprint
        {
            contractID = "contract_sewer_crisis",
            contractName = "Sewer System Crisis",
            description = "The city sewer system is infested. Large colony with aggressive alphas and a massive nest mother. Extreme danger - expert hunters only. Premium pay for completion.",
            clientName = "City Municipal Authority",
            difficulty = ContractData.ContractDifficulty.Expert,
            environment = ContractData.EnvironmentType.Sewer,
            sceneName = "SewerScene",
            targetCount = 18,
            timeLimit = 480f, // 8 minutes
            ammunition = 25,
            allowResupply = true,
            droneRatCount = 13,
            nestMotherCount = 2,
            alphaRatCount = 3,
            baseReward = 3500,
            oneStarThreshold = 12,
            twoStarThreshold = 15,
            threeStarThreshold = 18,
            experienceReward = 500,
            bonusExperiencePerStar = 100,
            requiredLevel = 8
        };
    }

    // Helper class
    private class ContractBlueprint
    {
        public string contractID;
        public string contractName;
        public string description;
        public string clientName;
        public ContractData.ContractDifficulty difficulty;
        public ContractData.EnvironmentType environment;
        public string sceneName;
        public int targetCount;
        public float timeLimit;
        public int ammunition;
        public bool allowResupply;
        public int droneRatCount;
        public int nestMotherCount;
        public int alphaRatCount;
        public int baseReward;
        public int oneStarThreshold;
        public int twoStarThreshold;
        public int threeStarThreshold;
        public int experienceReward;
        public int bonusExperiencePerStar;
        public int requiredLevel;
    }
}
#endif
