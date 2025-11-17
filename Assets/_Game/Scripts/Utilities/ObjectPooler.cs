using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Object pooling system for efficient object reuse
/// Reduces GC pressure and improves performance
/// </summary>
public class ObjectPooler : Singleton<ObjectPooler>
{
    [System.Serializable]
    public class Pool
    {
        public string tag;
        public GameObject prefab;
        public int size;
    }

    [Header("Pool Configuration")]
    [SerializeField] private List<Pool> pools = new List<Pool>();
    [SerializeField] private bool expandPools = true;

    private Dictionary<string, Queue<GameObject>> poolDictionary;
    private Dictionary<string, GameObject> prefabDictionary;

    protected override void Awake()
    {
        base.Awake();
        InitializePools();
    }

    private void InitializePools()
    {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();
        prefabDictionary = new Dictionary<string, GameObject>();

        foreach (Pool pool in pools)
        {
            if (pool.prefab == null)
            {
                Debug.LogWarning($"[ObjectPooler] Pool '{pool.tag}' has null prefab!");
                continue;
            }

            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = CreatePooledObject(pool.prefab, pool.tag);
                objectPool.Enqueue(obj);
            }

            poolDictionary.Add(pool.tag, objectPool);
            prefabDictionary.Add(pool.tag, pool.prefab);

            Debug.Log($"[ObjectPooler] Initialized pool '{pool.tag}' with {pool.size} objects");
        }
    }

    private GameObject CreatePooledObject(GameObject prefab, string poolTag)
    {
        GameObject obj = Instantiate(prefab);
        obj.name = $"{prefab.name} (Pooled)";
        obj.SetActive(false);
        obj.transform.SetParent(transform);

        // Add pooled tag component for tracking
        PooledObject pooledComponent = obj.AddComponent<PooledObject>();
        pooledComponent.poolTag = poolTag;

        return obj;
    }

    public GameObject Spawn(string tag, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogError($"[ObjectPooler] Pool with tag '{tag}' doesn't exist!");
            return null;
        }

        GameObject objectToSpawn;

        // If pool is empty and we can expand, create new object
        if (poolDictionary[tag].Count == 0)
        {
            if (expandPools)
            {
                Debug.LogWarning($"[ObjectPooler] Pool '{tag}' exhausted, creating new object");
                objectToSpawn = CreatePooledObject(prefabDictionary[tag], tag);
            }
            else
            {
                Debug.LogError($"[ObjectPooler] Pool '{tag}' exhausted and expand is disabled!");
                return null;
            }
        }
        else
        {
            objectToSpawn = poolDictionary[tag].Dequeue();
        }

        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;
        objectToSpawn.SetActive(true);

        // Notify IPoolable components
        IPoolable[] poolables = objectToSpawn.GetComponents<IPoolable>();
        foreach (var poolable in poolables)
        {
            poolable.OnSpawn();
        }

        // Return to pool after use
        poolDictionary[tag].Enqueue(objectToSpawn);

        return objectToSpawn;
    }

    public void Despawn(GameObject obj)
    {
        if (obj == null) return;

        // Notify IPoolable components
        IPoolable[] poolables = obj.GetComponents<IPoolable>();
        foreach (var poolable in poolables)
        {
            poolable.OnDespawn();
        }

        obj.SetActive(false);
        obj.transform.SetParent(transform);
    }

    public void DespawnAfterDelay(GameObject obj, float delay)
    {
        if (obj == null) return;
        StartCoroutine(DespawnDelayed(obj, delay));
    }

    private System.Collections.IEnumerator DespawnDelayed(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        Despawn(obj);
    }

    public void PrewarmPool(string tag, int count)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogError($"[ObjectPooler] Pool with tag '{tag}' doesn't exist!");
            return;
        }

        for (int i = 0; i < count; i++)
        {
            GameObject obj = CreatePooledObject(prefabDictionary[tag], tag);
            poolDictionary[tag].Enqueue(obj);
        }

        Debug.Log($"[ObjectPooler] Prewarmed pool '{tag}' with {count} additional objects");
    }

    public void ClearPool(string tag)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogError($"[ObjectPooler] Pool with tag '{tag}' doesn't exist!");
            return;
        }

        Queue<GameObject> pool = poolDictionary[tag];
        while (pool.Count > 0)
        {
            GameObject obj = pool.Dequeue();
            Destroy(obj);
        }

        Debug.Log($"[ObjectPooler] Cleared pool '{tag}'");
    }

    public int GetPoolSize(string tag)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            return 0;
        }

        return poolDictionary[tag].Count;
    }
}

/// <summary>
/// Interface for objects that need to respond to pooling events
/// </summary>
public interface IPoolable
{
    void OnSpawn();
    void OnDespawn();
}

/// <summary>
/// Component added to pooled objects for tracking
/// </summary>
public class PooledObject : MonoBehaviour
{
    public string poolTag;
}
