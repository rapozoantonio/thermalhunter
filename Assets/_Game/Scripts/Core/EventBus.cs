using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Central event bus for decoupled communication between systems
/// Uses pub/sub pattern for loose coupling
/// </summary>
public static class EventBus
{
    private static Dictionary<Type, List<Delegate>> subscribers = new Dictionary<Type, List<Delegate>>();

    /// <summary>
    /// Subscribe to an event type
    /// </summary>
    public static void Subscribe<T>(Action<T> handler) where T : IGameEvent
    {
        Type eventType = typeof(T);

        if (!subscribers.ContainsKey(eventType))
        {
            subscribers[eventType] = new List<Delegate>();
        }

        subscribers[eventType].Add(handler);
    }

    /// <summary>
    /// Unsubscribe from an event type
    /// </summary>
    public static void Unsubscribe<T>(Action<T> handler) where T : IGameEvent
    {
        Type eventType = typeof(T);

        if (subscribers.ContainsKey(eventType))
        {
            subscribers[eventType].Remove(handler);
        }
    }

    /// <summary>
    /// Publish an event to all subscribers
    /// </summary>
    public static void Publish<T>(T eventData) where T : IGameEvent
    {
        Type eventType = typeof(T);

        if (subscribers.ContainsKey(eventType))
        {
            foreach (var subscriber in subscribers[eventType])
            {
                try
                {
                    ((Action<T>)subscriber)?.Invoke(eventData);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"[EventBus] Error invoking handler for {eventType.Name}: {ex.Message}");
                }
            }
        }
    }

    /// <summary>
    /// Clear all subscribers (useful for scene transitions)
    /// </summary>
    public static void Clear()
    {
        subscribers.Clear();
    }

    /// <summary>
    /// Clear subscribers for specific event type
    /// </summary>
    public static void Clear<T>() where T : IGameEvent
    {
        Type eventType = typeof(T);
        if (subscribers.ContainsKey(eventType))
        {
            subscribers[eventType].Clear();
        }
    }
}

/// <summary>
/// Base interface for all game events
/// </summary>
public interface IGameEvent { }

// ===== Game Events =====

public struct GameStartedEvent : IGameEvent { }
public struct GamePausedEvent : IGameEvent { }
public struct GameResumedEvent : IGameEvent { }
public struct GameOverEvent : IGameEvent
{
    public bool Success;
    public int FinalScore;
    public float CompletionTime;
}

public struct ThermalActivatedEvent : IGameEvent { }
public struct ThermalDeactivatedEvent : IGameEvent { }

public struct WeaponFiredEvent : IGameEvent
{
    public string WeaponID;
    public UnityEngine.Vector3 Origin;
    public UnityEngine.Vector3 HitPoint;
}

public struct TargetHitEvent : IGameEvent
{
    public UnityEngine.GameObject Target;
    public bool IsWeakPoint;
    public int Damage;
}

public struct TargetKilledEvent : IGameEvent
{
    public UnityEngine.GameObject Target;
    public bool IsWeakPoint;
    public int ScoreGained;
}

public struct MissionStartedEvent : IGameEvent
{
    public string ContractID;
}

public struct MissionCompletedEvent : IGameEvent
{
    public string ContractID;
    public int Stars;
    public int Score;
    public float CompletionTime;
}

public struct MissionFailedEvent : IGameEvent
{
    public string ContractID;
    public string Reason;
}

public struct ScoreChangedEvent : IGameEvent
{
    public int NewScore;
    public int Delta;
}

public struct ExperienceGainedEvent : IGameEvent
{
    public int Amount;
}

public struct LevelUpEvent : IGameEvent
{
    public int NewLevel;
}

public struct WeaponUnlockedEvent : IGameEvent
{
    public string WeaponID;
}

public struct ScopeUnlockedEvent : IGameEvent
{
    public string ScopeID;
}

public struct AdWatchedEvent : IGameEvent
{
    public string AdType;
    public bool Success;
}

public struct PurchaseCompletedEvent : IGameEvent
{
    public string ProductID;
    public decimal Price;
}

public struct MissedShotEvent : IGameEvent
{
    public UnityEngine.Vector3 HitPoint;
}
