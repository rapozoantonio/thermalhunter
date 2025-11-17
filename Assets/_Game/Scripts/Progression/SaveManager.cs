using UnityEngine;
using System.IO;
using System.Collections.Generic;

/// <summary>
/// Manages player save data and progression persistence
/// Handles save/load operations with JSON serialization
/// </summary>
public class SaveManager : Singleton<SaveManager>
{
    [System.Serializable]
    public class SaveData
    {
        // Player Stats
        public int level = 1;
        public int experience = 0;
        public int totalKills = 0;
        public int totalShots = 0;
        public int totalHeadshots = 0;
        public int missionsCompleted = 0;

        // Unlocks
        public List<string> unlockedWeapons = new List<string>();
        public List<string> unlockedScopes = new List<string>();
        public List<string> completedContracts = new List<string>();

        // Contract Progress
        public Dictionary<string, int> contractStars = new Dictionary<string, int>();

        // Currency/Resources
        public int currency = 0;

        // Settings
        public float masterVolume = 1f;
        public float sfxVolume = 1f;
        public float musicVolume = 0.7f;
        public float sensitivity = 1f;
        public bool adsRemoved = false;

        // Meta
        public float totalPlayTime = 0f;
        public string lastPlayed;
        public int saveVersion = Constants.SAVE_VERSION;

#if UNITY_STANDALONE
        public string steamID;
#endif
    }

    private SaveData currentSave;
    private string savePath;
    private float sessionStartTime;

    // Events
    public event System.Action OnSaveLoaded;
    public event System.Action OnSaveSaved;

    // Properties
    public SaveData CurrentSave => currentSave;
    public int PlayerLevel => currentSave.level;
    public int PlayerExperience => currentSave.experience;
    public int Currency => currentSave.currency;

    protected override void Awake()
    {
        base.Awake();
        savePath = GetSavePath();
        sessionStartTime = Time.realtimeSinceStartup;
        LoadGame();
    }

    private string GetSavePath()
    {
        return Path.Combine(Application.persistentDataPath, Constants.SAVE_FILE_NAME);
    }

    public void SaveGame()
    {
        // Update meta information
        currentSave.lastPlayed = System.DateTime.Now.ToString();
        currentSave.totalPlayTime += Time.realtimeSinceStartup - sessionStartTime;
        sessionStartTime = Time.realtimeSinceStartup;

        string json = JsonUtility.ToJson(currentSave, true);

#if UNITY_STANDALONE && !UNITY_EDITOR
        // Steam Cloud Save (would require Steamworks.NET)
        // SaveToSteamCloud(json);
#endif

        // Always save locally as backup
        try
        {
            File.WriteAllText(savePath, json);
            PlayerPrefs.SetString("LastSavePath", savePath);
            PlayerPrefs.SetString("LastSaveBackup", json); // Backup in PlayerPrefs
            PlayerPrefs.Save();

            Debug.Log($"[SaveManager] Game saved successfully to {savePath}");
            OnSaveSaved?.Invoke();
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[SaveManager] Failed to save game: {e.Message}");
        }
    }

    public void LoadGame()
    {
        string json = "";

#if UNITY_STANDALONE && !UNITY_EDITOR
        // Try Steam Cloud first
        // json = LoadFromSteamCloud();
#endif

        // Try local file
        if (string.IsNullOrEmpty(json) && File.Exists(savePath))
        {
            try
            {
                json = File.ReadAllText(savePath);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[SaveManager] Failed to read save file: {e.Message}");
            }
        }

        // Try PlayerPrefs backup
        if (string.IsNullOrEmpty(json))
        {
            json = PlayerPrefs.GetString("LastSaveBackup", "");
        }

        // Parse JSON
        if (!string.IsNullOrEmpty(json))
        {
            try
            {
                currentSave = JsonUtility.FromJson<SaveData>(json);

                // Handle save version migrations
                if (currentSave.saveVersion < Constants.SAVE_VERSION)
                {
                    MigrateSaveData(currentSave.saveVersion);
                }

                Debug.Log($"[SaveManager] Game loaded successfully - Level {currentSave.level}");
                OnSaveLoaded?.Invoke();
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[SaveManager] Failed to parse save data: {e.Message}");
                CreateNewSave();
            }
        }
        else
        {
            CreateNewSave();
        }

        // Register with ServiceLocator
        ServiceLocator.Instance.Register<SaveManager>(this);
    }

    private void CreateNewSave()
    {
        currentSave = new SaveData();

        // Grant starting items
        currentSave.unlockedWeapons.Add("BasicAirRifle");
        currentSave.unlockedScopes.Add("StandardThermal");

        SaveGame();
        Debug.Log("[SaveManager] Created new save file");
    }

    private void MigrateSaveData(int fromVersion)
    {
        Debug.Log($"[SaveManager] Migrating save data from version {fromVersion} to {Constants.SAVE_VERSION}");

        // Add migration logic here as game evolves
        // Example:
        // if (fromVersion < 2)
        // {
        //     // Add new fields that didn't exist in version 1
        // }

        currentSave.saveVersion = Constants.SAVE_VERSION;
        SaveGame();
    }

    // === Progression Methods ===

    public void AddExperience(int amount)
    {
        currentSave.experience += amount;

        // Level up check
        int requiredExp = CalculateRequiredExperience(currentSave.level);
        while (currentSave.experience >= requiredExp)
        {
            currentSave.experience -= requiredExp;
            LevelUp();
            requiredExp = CalculateRequiredExperience(currentSave.level);
        }

        SaveGame();
        EventBus.Publish(new ExperienceGainedEvent { Amount = amount });
    }

    private int CalculateRequiredExperience(int level)
    {
        // Exponential scaling: level * 100
        return level * 100;
    }

    private void LevelUp()
    {
        currentSave.level++;

        Debug.Log($"[SaveManager] Level up! Now level {currentSave.level}");

        // Check for level-based unlocks
        CheckLevelUnlocks();

        EventBus.Publish(new LevelUpEvent { NewLevel = currentSave.level });
    }

    private void CheckLevelUnlocks()
    {
        // These would normally be defined in a LevelUnlocksData ScriptableObject
        switch (currentSave.level)
        {
            case 5:
                UnlockWeapon("AdvancedAirRifle");
                break;
            case 10:
                UnlockScope("MidRangeThermal");
                break;
            case 15:
                UnlockWeapon("HighPowerRifle");
                break;
            case 20:
                UnlockScope("PremiumThermal");
                break;
        }
    }

    public void UnlockWeapon(string weaponID)
    {
        if (!currentSave.unlockedWeapons.Contains(weaponID))
        {
            currentSave.unlockedWeapons.Add(weaponID);
            SaveGame();

            Debug.Log($"[SaveManager] Unlocked weapon: {weaponID}");
            EventBus.Publish(new WeaponUnlockedEvent { WeaponID = weaponID });
        }
    }

    public void UnlockScope(string scopeID)
    {
        if (!currentSave.unlockedScopes.Contains(scopeID))
        {
            currentSave.unlockedScopes.Add(scopeID);
            SaveGame();

            Debug.Log($"[SaveManager] Unlocked scope: {scopeID}");
            EventBus.Publish(new ScopeUnlockedEvent { ScopeID = scopeID });
        }
    }

    public void CompleteContract(string contractID, int stars)
    {
        if (!currentSave.completedContracts.Contains(contractID))
        {
            currentSave.completedContracts.Add(contractID);
            currentSave.missionsCompleted++;
        }

        // Update best stars for this contract
        if (!currentSave.contractStars.ContainsKey(contractID) || currentSave.contractStars[contractID] < stars)
        {
            currentSave.contractStars[contractID] = stars;
        }

        // Award experience
        AddExperience(stars * 50);

        SaveGame();
        Debug.Log($"[SaveManager] Completed contract: {contractID} with {stars} stars");
    }

    public void AddCurrency(int amount)
    {
        currentSave.currency += amount;
        SaveGame();
    }

    public void SpendCurrency(int amount)
    {
        if (currentSave.currency >= amount)
        {
            currentSave.currency -= amount;
            SaveGame();
        }
        else
        {
            Debug.LogWarning($"[SaveManager] Not enough currency! Need {amount}, have {currentSave.currency}");
        }
    }

    public void IncrementStat(string statName, int amount = 1)
    {
        switch (statName.ToLower())
        {
            case "kills":
                currentSave.totalKills += amount;
                break;
            case "shots":
                currentSave.totalShots += amount;
                break;
            case "headshots":
                currentSave.totalHeadshots += amount;
                break;
        }
    }

    // === Query Methods ===

    public bool IsWeaponUnlocked(string weaponID)
    {
        return currentSave.unlockedWeapons.Contains(weaponID);
    }

    public bool IsScopeUnlocked(string scopeID)
    {
        return currentSave.unlockedScopes.Contains(scopeID);
    }

    public bool IsContractCompleted(string contractID)
    {
        return currentSave.completedContracts.Contains(contractID);
    }

    public int GetContractStars(string contractID)
    {
        return currentSave.contractStars.ContainsKey(contractID) ? currentSave.contractStars[contractID] : 0;
    }

    public float GetAccuracy()
    {
        if (currentSave.totalShots == 0) return 0f;
        return (float)currentSave.totalKills / currentSave.totalShots;
    }

    public float GetHeadshotRate()
    {
        if (currentSave.totalKills == 0) return 0f;
        return (float)currentSave.totalHeadshots / currentSave.totalKills;
    }

    // === Settings ===

    public void SetVolume(string volumeType, float value)
    {
        value = Mathf.Clamp01(value);

        switch (volumeType.ToLower())
        {
            case "master":
                currentSave.masterVolume = value;
                break;
            case "sfx":
                currentSave.sfxVolume = value;
                break;
            case "music":
                currentSave.musicVolume = value;
                break;
        }

        SaveGame();
    }

    public void SetSensitivity(float value)
    {
        currentSave.sensitivity = Mathf.Clamp(value, 0.1f, 2f);
        SaveGame();
    }

    public void SetAdsRemoved(bool removed)
    {
        currentSave.adsRemoved = removed;
        SaveGame();
    }

    // === Debugging ===

    public void ResetProgress()
    {
        if (File.Exists(savePath))
        {
            File.Delete(savePath);
        }

        PlayerPrefs.DeleteAll();
        CreateNewSave();

        Debug.Log("[SaveManager] Progress reset!");
    }

    private void OnApplicationQuit()
    {
        SaveGame();
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            SaveGame();
        }
    }
}
