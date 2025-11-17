using UnityEngine;
using System;

/// <summary>
/// Central game state manager
/// Handles game flow, state transitions, and core game loop
/// </summary>
public class GameManager : Singleton<GameManager>
{
    [Header("Game State")]
    [SerializeField] private GameState currentState = GameState.MainMenu;

    [Header("Mission Settings")]
    [SerializeField] private ContractData currentContract;
    [SerializeField] private float missionTimeRemaining;
    [SerializeField] private int currentScore;
    [SerializeField] private int targetsKilled;
    [SerializeField] private int shotsFired;
    [SerializeField] private int shotsHit;

    [Header("Loadout")]
    [SerializeField] private WeaponData equippedWeapon;
    [SerializeField] private ScopeData equippedScope;
    [SerializeField] private int currentAmmo;

    private float missionStartTime;
    private bool isPaused;

    public enum GameState
    {
        MainMenu,
        ContractSelection,
        LoadoutSetup,
        MissionBriefing,
        InMission,
        MissionComplete,
        MissionFailed,
        Paused
    }

    // Properties
    public GameState CurrentState => currentState;
    public ContractData CurrentContract => currentContract;
    public float MissionTimeRemaining => missionTimeRemaining;
    public int CurrentScore => currentScore;
    public int TargetsKilled => targetsKilled;
    public int ShotsFired => shotsFired;
    public float Accuracy => shotsFired > 0 ? (float)shotsHit / shotsFired : 0f;
    public WeaponData EquippedWeapon => equippedWeapon;
    public ScopeData EquippedScope => equippedScope;
    public int CurrentAmmo => currentAmmo;

    protected override void Awake()
    {
        base.Awake();
        SubscribeToEvents();
    }

    private void Start()
    {
        ChangeState(GameState.MainMenu);
    }

    private void Update()
    {
        if (currentState == GameState.InMission && !isPaused)
        {
            UpdateMission();
        }
    }

    private void UpdateMission()
    {
        // Update mission timer
        missionTimeRemaining -= Time.deltaTime;

        if (missionTimeRemaining <= 0f)
        {
            FailMission("Time's up!");
            return;
        }

        // Check win condition
        if (currentContract != null && targetsKilled >= currentContract.targetCount)
        {
            CompleteMission();
        }

        // Check fail conditions
        if (currentAmmo <= 0 && shotsFired >= currentContract.ammunitionAllowed)
        {
            FailMission("Out of ammunition!");
        }
    }

    private void SubscribeToEvents()
    {
        EventBus.Subscribe<TargetKilledEvent>(OnTargetKilled);
        EventBus.Subscribe<WeaponFiredEvent>(OnWeaponFired);
        EventBus.Subscribe<TargetHitEvent>(OnTargetHit);
    }

    private void OnDestroy()
    {
        EventBus.Unsubscribe<TargetKilledEvent>(OnTargetKilled);
        EventBus.Unsubscribe<WeaponFiredEvent>(OnWeaponFired);
        EventBus.Unsubscribe<TargetHitEvent>(OnTargetHit);
    }

    // === State Management ===

    public void ChangeState(GameState newState)
    {
        if (currentState == newState) return;

        ExitState(currentState);
        currentState = newState;
        EnterState(newState);

        Debug.Log($"[GameManager] State changed to: {newState}");
    }

    private void EnterState(GameState state)
    {
        switch (state)
        {
            case GameState.MainMenu:
                Time.timeScale = 1f;
                break;

            case GameState.InMission:
                StartMission();
                break;

            case GameState.Paused:
                Time.timeScale = 0f;
                EventBus.Publish(new GamePausedEvent());
                break;

            case GameState.MissionComplete:
                Time.timeScale = 0f;
                break;

            case GameState.MissionFailed:
                Time.timeScale = 0f;
                break;
        }
    }

    private void ExitState(GameState state)
    {
        switch (state)
        {
            case GameState.Paused:
                Time.timeScale = 1f;
                EventBus.Publish(new GameResumedEvent());
                break;
        }
    }

    // === Mission Management ===

    public void SelectContract(ContractData contract)
    {
        currentContract = contract;
        Debug.Log($"[GameManager] Contract selected: {contract.contractName}");
    }

    public void EquipLoadout(WeaponData weapon, ScopeData scope)
    {
        equippedWeapon = weapon;
        equippedScope = scope;
        currentAmmo = currentContract != null ? currentContract.ammunitionAllowed : Constants.DEFAULT_AMMO;
        Debug.Log($"[GameManager] Loadout equipped: {weapon.weaponName} + {scope.scopeName}");
    }

    private void StartMission()
    {
        if (currentContract == null)
        {
            Debug.LogError("[GameManager] Cannot start mission - no contract selected!");
            return;
        }

        // Reset mission stats
        missionStartTime = Time.time;
        missionTimeRemaining = currentContract.timeLimit;
        currentScore = 0;
        targetsKilled = 0;
        shotsFired = 0;
        shotsHit = 0;
        currentAmmo = currentContract.ammunitionAllowed;

        Debug.Log($"[GameManager] Mission started: {currentContract.contractName}");
        EventBus.Publish(new MissionStartedEvent { ContractID = currentContract.contractID });
    }

    private void CompleteMission()
    {
        float completionTime = Time.time - missionStartTime;
        int stars = CalculateStars();

        ChangeState(GameState.MissionComplete);

        // Save progress
        SaveManager.Instance.CompleteContract(currentContract.contractID, stars);
        SaveManager.Instance.SaveGame();

        Debug.Log($"[GameManager] Mission completed! Stars: {stars}, Score: {currentScore}");

        EventBus.Publish(new MissionCompletedEvent
        {
            ContractID = currentContract.contractID,
            Stars = stars,
            Score = currentScore,
            CompletionTime = completionTime
        });
    }

    private void FailMission(string reason)
    {
        ChangeState(GameState.MissionFailed);

        Debug.Log($"[GameManager] Mission failed: {reason}");

        EventBus.Publish(new MissionFailedEvent
        {
            ContractID = currentContract.contractID,
            Reason = reason
        });
    }

    private int CalculateStars()
    {
        if (currentContract == null) return 0;

        if (targetsKilled >= currentContract.threeStarThreshold)
            return 3;
        else if (targetsKilled >= currentContract.twoStarThreshold)
            return 2;
        else if (targetsKilled >= currentContract.oneStarThreshold)
            return 1;
        else
            return 0;
    }

    public void AddScore(int amount)
    {
        int previousScore = currentScore;
        currentScore += amount;

        EventBus.Publish(new ScoreChangedEvent
        {
            NewScore = currentScore,
            Delta = amount
        });
    }

    public void ConsumeAmmo()
    {
        if (currentAmmo > 0)
        {
            currentAmmo--;
        }
    }

    public void AddAmmo(int amount)
    {
        currentAmmo += amount;
    }

    // === Event Handlers ===

    private void OnTargetKilled(TargetKilledEvent evt)
    {
        targetsKilled++;
        AddScore(Constants.BASE_SCORE_PER_KILL);

        if (evt.IsWeakPoint)
        {
            AddScore(Constants.HEADSHOT_BONUS);
        }
    }

    private void OnWeaponFired(WeaponFiredEvent evt)
    {
        shotsFired++;
        ConsumeAmmo();
    }

    private void OnTargetHit(TargetHitEvent evt)
    {
        shotsHit++;
    }

    // === Pause/Resume ===

    public void PauseGame()
    {
        if (currentState == GameState.InMission)
        {
            isPaused = true;
            ChangeState(GameState.Paused);
        }
    }

    public void ResumeGame()
    {
        if (currentState == GameState.Paused)
        {
            isPaused = false;
            ChangeState(GameState.InMission);
        }
    }

    public void RestartMission()
    {
        ChangeState(GameState.InMission);
    }

    public void ReturnToMenu()
    {
        ChangeState(GameState.MainMenu);
    }
}
