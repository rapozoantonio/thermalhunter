using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Manages enemy spawning for missions
/// Handles rat distribution and spawn points
/// </summary>
public class SpawnManager : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private List<Transform> spawnPoints = new List<Transform>();
    [SerializeField] private float spawnRadius = 5f;
    [SerializeField] private float minSpawnDistance = 2f;

    [Header("Prefabs")]
    [SerializeField] private GameObject ratSmallPrefab;
    [SerializeField] private GameObject ratMediumPrefab;
    [SerializeField] private GameObject ratLargePrefab;

    private ObjectPooler objectPooler;
    private List<GameObject> spawnedRats = new List<GameObject>();
    private ContractData currentContract;

    private void Awake()
    {
        objectPooler = ObjectPooler.Instance;
    }

    private void Start()
    {
        // Find all spawn points if not manually assigned
        if (spawnPoints.Count == 0)
        {
            GameObject[] spawnObjects = GameObject.FindGameObjectsWithTag("SpawnPoint");
            foreach (var obj in spawnObjects)
            {
                spawnPoints.Add(obj.transform);
            }
        }

        Debug.Log($"[SpawnManager] Found {spawnPoints.Count} spawn points");
    }

    /// <summary>
    /// Spawn rats for the current contract
    /// </summary>
    public void SpawnRatsForContract(ContractData contract)
    {
        if (contract == null)
        {
            Debug.LogError("[SpawnManager] Cannot spawn - contract is null!");
            return;
        }

        currentContract = contract;

        // Clear any existing rats
        DespawnAllRats();

        // Spawn based on contract distribution
        SpawnRatType(RatAI.RatType.Drone, RatAI.RatSize.Small, contract.droneRatCount);
        SpawnRatType(RatAI.RatType.NestMother, RatAI.RatSize.Large, contract.nestMotherCount);
        SpawnRatType(RatAI.RatType.Alpha, RatAI.RatSize.Medium, contract.alphaRatCount);

        Debug.Log($"[SpawnManager] Spawned {spawnedRats.Count} rats for contract '{contract.contractName}'");
    }

    private void SpawnRatType(RatAI.RatType type, RatAI.RatSize size, int count)
    {
        for (int i = 0; i < count; i++)
        {
            Vector3 spawnPosition = GetRandomSpawnPosition();
            GameObject ratPrefab = GetRatPrefab(size);

            if (ratPrefab == null)
            {
                Debug.LogWarning($"[SpawnManager] No prefab found for size {size}");
                continue;
            }

            GameObject rat = Instantiate(ratPrefab, spawnPosition, Quaternion.identity);
            rat.name = $"{type}Rat_{i}";

            // Configure rat
            RatAI ratAI = rat.GetComponent<RatAI>();
            if (ratAI != null)
            {
                // Set rat type via reflection or public setter
                // ratAI.SetType(type);
                // ratAI.SetSize(size);
            }

            spawnedRats.Add(rat);
        }
    }

    private GameObject GetRatPrefab(RatAI.RatSize size)
    {
        return size switch
        {
            RatAI.RatSize.Small => ratSmallPrefab,
            RatAI.RatSize.Medium => ratMediumPrefab,
            RatAI.RatSize.Large => ratLargePrefab,
            _ => ratMediumPrefab
        };
    }

    private Vector3 GetRandomSpawnPosition()
    {
        if (spawnPoints.Count == 0)
        {
            // Fallback to random position around origin
            return Random.insideUnitSphere * spawnRadius;
        }

        // Select random spawn point
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Count)];

        // Add random offset
        Vector3 randomOffset = Random.insideUnitSphere * spawnRadius;
        randomOffset.y = 0; // Keep on ground

        Vector3 position = spawnPoint.position + randomOffset;

        // Ensure not too close to other rats
        int attempts = 0;
        while (IsTooCloseToOtherRats(position) && attempts < 10)
        {
            randomOffset = Random.insideUnitSphere * spawnRadius;
            randomOffset.y = 0;
            position = spawnPoint.position + randomOffset;
            attempts++;
        }

        return position;
    }

    private bool IsTooCloseToOtherRats(Vector3 position)
    {
        foreach (var rat in spawnedRats)
        {
            if (rat == null) continue;

            float distance = Vector3.Distance(position, rat.transform.position);
            if (distance < minSpawnDistance)
            {
                return true;
            }
        }

        return false;
    }

    public void DespawnAllRats()
    {
        foreach (var rat in spawnedRats)
        {
            if (rat != null)
            {
                Destroy(rat);
            }
        }

        spawnedRats.Clear();
        Debug.Log("[SpawnManager] All rats despawned");
    }

    public int GetActiveRatCount()
    {
        // Remove null entries
        spawnedRats.RemoveAll(r => r == null);

        // Count alive rats
        int aliveCount = 0;
        foreach (var rat in spawnedRats)
        {
            RatAI ratAI = rat.GetComponent<RatAI>();
            if (ratAI != null && !ratAI.IsDead)
            {
                aliveCount++;
            }
        }

        return aliveCount;
    }

    public List<GameObject> GetAllRats()
    {
        return new List<GameObject>(spawnedRats);
    }

    private void OnDestroy()
    {
        DespawnAllRats();
    }

    private void OnDrawGizmosSelected()
    {
        // Visualize spawn points
        Gizmos.color = Color.yellow;
        foreach (var point in spawnPoints)
        {
            if (point != null)
            {
                Gizmos.DrawWireSphere(point.position, spawnRadius);
            }
        }
    }
}
