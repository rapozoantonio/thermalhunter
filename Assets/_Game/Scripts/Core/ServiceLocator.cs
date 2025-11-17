using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Service locator pattern for dependency injection
/// Provides centralized access to game services
/// </summary>
public class ServiceLocator : Singleton<ServiceLocator>
{
    private Dictionary<Type, object> services = new Dictionary<Type, object>();

    /// <summary>
    /// Register a service instance
    /// </summary>
    public void Register<T>(T service) where T : class
    {
        Type type = typeof(T);

        if (services.ContainsKey(type))
        {
            Debug.LogWarning($"[ServiceLocator] Service {type.Name} is already registered. Overwriting.");
            services[type] = service;
        }
        else
        {
            services.Add(type, service);
            Debug.Log($"[ServiceLocator] Registered service: {type.Name}");
        }
    }

    /// <summary>
    /// Unregister a service
    /// </summary>
    public void Unregister<T>() where T : class
    {
        Type type = typeof(T);

        if (services.ContainsKey(type))
        {
            services.Remove(type);
            Debug.Log($"[ServiceLocator] Unregistered service: {type.Name}");
        }
    }

    /// <summary>
    /// Get a registered service
    /// </summary>
    public T Get<T>() where T : class
    {
        Type type = typeof(T);

        if (services.ContainsKey(type))
        {
            return services[type] as T;
        }

        Debug.LogError($"[ServiceLocator] Service {type.Name} not found!");
        return null;
    }

    /// <summary>
    /// Try to get a service (returns null if not found)
    /// </summary>
    public T TryGet<T>() where T : class
    {
        Type type = typeof(T);
        return services.ContainsKey(type) ? services[type] as T : null;
    }

    /// <summary>
    /// Check if a service is registered
    /// </summary>
    public bool Has<T>() where T : class
    {
        return services.ContainsKey(typeof(T));
    }

    /// <summary>
    /// Clear all services
    /// </summary>
    public void ClearAll()
    {
        services.Clear();
        Debug.Log("[ServiceLocator] All services cleared");
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        ClearAll();
    }
}
