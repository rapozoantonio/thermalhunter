using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

/// <summary>
/// Generates procedural farm environment with Minecraft-style aesthetics
/// Creates buildings, props, and spawns rats
/// </summary>
public class ProceduralEnvironmentManager : MonoBehaviour
{
    [Header("Environment Settings")]
    [SerializeField] private float environmentSize = 50f;
    [SerializeField] private int numberOfBuildings = 3;
    [SerializeField] private int numberOfProps = 20;
    [SerializeField] private int initialRatCount = 20;

    [Header("Building Settings")]
    [SerializeField] private Vector2 buildingSizeRange = new Vector2(8f, 15f);
    [SerializeField] private Vector2 buildingHeightRange = new Vector2(3f, 5f);

    [Header("NavMesh")]
    [SerializeField] private bool bakeNavMeshOnGenerate = true;

    private ProceduralAssetFactory assetFactory;
    private List<GameObject> spawnedObjects = new List<GameObject>();
    private List<GameObject> rats = new List<GameObject>();

    private void Awake()
    {
        assetFactory = ProceduralAssetFactory.Instance;
    }

    /// <summary>
    /// Generates the complete environment
    /// </summary>
    public void GenerateEnvironment(float size = 0)
    {
        if (size > 0)
        {
            environmentSize = size;
        }

        Debug.Log("[ProceduralEnvironmentManager] Generating environment...");

        // Clear existing environment
        ClearEnvironment();

        // Create ground
        CreateGround();

        // Create buildings
        CreateBuildings();

        // Create props (barrels, crates, etc.)
        CreateProps();

        // Bake NavMesh
        if (bakeNavMeshOnGenerate)
        {
            BakeNavMesh();
        }

        // Spawn rats
        SpawnRats();

        Debug.Log($"[ProceduralEnvironmentManager] Environment generated with {spawnedObjects.Count} objects");
    }

    private void ClearEnvironment()
    {
        foreach (GameObject obj in spawnedObjects)
        {
            if (obj != null)
            {
                Destroy(obj);
            }
        }
        spawnedObjects.Clear();

        foreach (GameObject rat in rats)
        {
            if (rat != null)
            {
                Destroy(rat);
            }
        }
        rats.Clear();
    }

    private void CreateGround()
    {
        GameObject ground = assetFactory.CreateGround(environmentSize, environmentSize);
        ground.transform.parent = transform;
        spawnedObjects.Add(ground);

        Debug.Log("[ProceduralEnvironmentManager] Created ground");
    }

    private void CreateBuildings()
    {
        for (int i = 0; i < numberOfBuildings; i++)
        {
            // Random position within environment bounds
            float x = Random.Range(-environmentSize / 3f, environmentSize / 3f);
            float z = Random.Range(-environmentSize / 3f, environmentSize / 3f);
            Vector3 position = new Vector3(x, 0, z);

            // Random size
            float width = Random.Range(buildingSizeRange.x, buildingSizeRange.y);
            float depth = Random.Range(buildingSizeRange.x, buildingSizeRange.y);
            float height = Random.Range(buildingHeightRange.x, buildingHeightRange.y);
            Vector3 size = new Vector3(width, height, depth);

            // Create building
            GameObject building = assetFactory.CreateBuildingStructure(position, size);
            building.transform.parent = transform;
            building.name = $"Building_{i + 1}";
            spawnedObjects.Add(building);
        }

        Debug.Log($"[ProceduralEnvironmentManager] Created {numberOfBuildings} buildings");
    }

    private void CreateProps()
    {
        string[] propTypes = { "Barrel", "Crate", "Barrel", "Crate" };

        for (int i = 0; i < numberOfProps; i++)
        {
            // Random position
            float x = Random.Range(-environmentSize / 2.5f, environmentSize / 2.5f);
            float z = Random.Range(-environmentSize / 2.5f, environmentSize / 2.5f);
            Vector3 position = new Vector3(x, 0, z);

            // Random prop type
            string propType = propTypes[Random.Range(0, propTypes.Length)];

            // Create prop
            GameObject prop = assetFactory.CreateEnvironmentProp(propType, position);
            prop.transform.parent = transform;
            prop.name = $"{propType}_{i + 1}";

            // Random rotation
            prop.transform.rotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0);

            spawnedObjects.Add(prop);
        }

        Debug.Log($"[ProceduralEnvironmentManager] Created {numberOfProps} props");
    }

    private void BakeNavMesh()
    {
        // Check if NavMesh is available
        NavMeshSurface surface = gameObject.GetComponent<NavMeshSurface>();

        if (surface == null)
        {
            // Try to add NavMeshSurface component (requires AI Navigation package)
            Debug.LogWarning("[ProceduralEnvironmentManager] NavMeshSurface not found. Add AI Navigation package and NavMeshSurface component for pathfinding.");

            // Create a simple NavMesh manually using NavMesh.AddNavMeshData
            // For production, install Unity's AI Navigation package
        }
        else
        {
            surface.BuildNavMesh();
            Debug.Log("[ProceduralEnvironmentManager] NavMesh baked");
        }
    }

    private void SpawnRats()
    {
        for (int i = 0; i < initialRatCount; i++)
        {
            // Random position
            float x = Random.Range(-environmentSize / 2.5f, environmentSize / 2.5f);
            float z = Random.Range(-environmentSize / 2.5f, environmentSize / 2.5f);
            Vector3 position = new Vector3(x, 0.2f, z);

            // Random rat type and size
            RatAI.RatSize size = (RatAI.RatSize)Random.Range(0, 3);
            RatAI.RatType type = RatAI.RatType.Drone;

            // Occasionally spawn special rats
            float rand = Random.value;
            if (rand < 0.05f)
            {
                type = RatAI.RatType.Alpha;
                size = RatAI.RatSize.Large;
            }
            else if (rand < 0.1f)
            {
                type = RatAI.RatType.NestMother;
                size = RatAI.RatSize.Large;
            }

            // Create rat
            GameObject rat = assetFactory.CreateRat(size, type);
            rat.transform.position = position;
            rat.transform.parent = transform;
            rat.name = $"Rat_{type}_{i + 1}";

            rats.Add(rat);
        }

        Debug.Log($"[ProceduralEnvironmentManager] Spawned {initialRatCount} rats");
    }

    /// <summary>
    /// Spawns a single rat at a random location
    /// </summary>
    public GameObject SpawnRat()
    {
        float x = Random.Range(-environmentSize / 2.5f, environmentSize / 2.5f);
        float z = Random.Range(-environmentSize / 2.5f, environmentSize / 2.5f);
        Vector3 position = new Vector3(x, 0.2f, z);

        RatAI.RatSize size = (RatAI.RatSize)Random.Range(0, 3);
        RatAI.RatType type = RatAI.RatType.Drone;

        GameObject rat = assetFactory.CreateRat(size, type);
        rat.transform.position = position;
        rat.transform.parent = transform;

        rats.Add(rat);

        return rat;
    }

    /// <summary>
    /// Removes a rat from the list
    /// </summary>
    public void RemoveRat(GameObject rat)
    {
        rats.Remove(rat);
    }

    public int RatCount => rats.Count;
    public List<GameObject> Rats => rats;
}
