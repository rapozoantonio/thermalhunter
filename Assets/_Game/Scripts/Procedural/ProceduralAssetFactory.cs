using UnityEngine;

/// <summary>
/// Factory for creating procedural game assets with Minecraft-style aesthetics
/// Generates GameObjects with meshes, materials, and components at runtime
/// </summary>
public class ProceduralAssetFactory : MonoBehaviour
{
    [Header("Material References")]
    [SerializeField] private Material ratMaterial;
    [SerializeField] private Material weaponMaterial;
    [SerializeField] private Material environmentMaterial;
    [SerializeField] private Material groundMaterial;

    [Header("Color Palettes (Minecraft-style)")]
    [SerializeField] private Color[] ratColors = new Color[]
    {
        new Color(0.4f, 0.3f, 0.2f), // Brown
        new Color(0.3f, 0.3f, 0.3f), // Dark gray
        new Color(0.5f, 0.4f, 0.3f)  // Light brown
    };

    [SerializeField] private Color weaponColor = new Color(0.2f, 0.2f, 0.2f); // Dark metal
    [SerializeField] private Color scopeColor = new Color(0.1f, 0.1f, 0.1f); // Black
    [SerializeField] private Color woodColor = new Color(0.5f, 0.3f, 0.1f); // Brown wood
    [SerializeField] private Color concreteColor = new Color(0.6f, 0.6f, 0.6f); // Concrete

    private static ProceduralAssetFactory instance;

    public static ProceduralAssetFactory Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<ProceduralAssetFactory>();
                if (instance == null)
                {
                    GameObject go = new GameObject("ProceduralAssetFactory");
                    instance = go.AddComponent<ProceduralAssetFactory>();
                }
            }
            return instance;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeMaterials();
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void InitializeMaterials()
    {
        // Create simple colored materials if not assigned
        if (ratMaterial == null)
        {
            ratMaterial = CreateColoredMaterial(ratColors[0], "RatMaterial");
        }

        if (weaponMaterial == null)
        {
            weaponMaterial = CreateColoredMaterial(weaponColor, "WeaponMaterial");
        }

        if (environmentMaterial == null)
        {
            environmentMaterial = CreateColoredMaterial(concreteColor, "EnvironmentMaterial");
        }

        if (groundMaterial == null)
        {
            groundMaterial = CreateColoredMaterial(new Color(0.2f, 0.3f, 0.2f), "GroundMaterial");
        }
    }

    private Material CreateColoredMaterial(Color color, string name)
    {
        Material mat = new Material(Shader.Find("Standard"))
        {
            name = name,
            color = color
        };
        mat.SetFloat("_Glossiness", 0.1f); // Low shine for blocky look
        mat.SetFloat("_Metallic", 0f);
        return mat;
    }

    /// <summary>
    /// Creates a procedural rat GameObject
    /// </summary>
    public GameObject CreateRat(RatAI.RatSize size = RatAI.RatSize.Medium, RatAI.RatType type = RatAI.RatType.Drone)
    {
        GameObject ratObj = new GameObject($"Rat_{type}_{size}");

        // Add mesh
        MeshFilter meshFilter = ratObj.AddComponent<MeshFilter>();
        float sizeMultiplier = size == RatAI.RatSize.Small ? 0.7f : (size == RatAI.RatSize.Large ? 1.3f : 1.0f);
        meshFilter.mesh = ProceduralMeshGenerator.CreateBlockyRat(sizeMultiplier);

        // Add renderer with random rat color
        MeshRenderer renderer = ratObj.AddComponent<MeshRenderer>();
        Color randomColor = ratColors[Random.Range(0, ratColors.Length)];
        renderer.material = CreateColoredMaterial(randomColor, "RatMaterial_Instance");

        // Add collider
        BoxCollider collider = ratObj.AddComponent<BoxCollider>();
        collider.size = new Vector3(0.4f, 0.2f, 0.6f) * sizeMultiplier;

        // Add NavMeshAgent
        UnityEngine.AI.NavMeshAgent navAgent = ratObj.AddComponent<UnityEngine.AI.NavMeshAgent>();
        navAgent.speed = 2f * sizeMultiplier;
        navAgent.acceleration = 8f;
        navAgent.angularSpeed = 180f;
        navAgent.stoppingDistance = 0.5f;
        navAgent.radius = 0.2f * sizeMultiplier;
        navAgent.height = 0.2f * sizeMultiplier;

        // Add HeatSignature component
        HeatSignature heatSig = ratObj.AddComponent<HeatSignature>();

        // Add RatAI component
        RatAI ai = ratObj.AddComponent<RatAI>();

        // Add Rigidbody for physics
        Rigidbody rb = ratObj.AddComponent<Rigidbody>();
        rb.mass = size == RatAI.RatSize.Small ? 0.3f : (size == RatAI.RatSize.Large ? 1.5f : 0.8f);
        rb.constraints = RigidbodyConstraints.FreezeRotation;

        // Set layer and tag
        ratObj.layer = LayerMask.NameToLayer(Constants.LAYER_TARGET);
        ratObj.tag = Constants.TAG_RAT;

        return ratObj;
    }

    /// <summary>
    /// Creates a procedural weapon GameObject
    /// </summary>
    public GameObject CreateWeapon(string weaponName = "Rifle")
    {
        GameObject weaponObj = new GameObject($"Weapon_{weaponName}");

        // Add mesh
        MeshFilter meshFilter = weaponObj.AddComponent<MeshFilter>();
        meshFilter.mesh = ProceduralMeshGenerator.CreateBlockyRifle();

        // Add renderer
        MeshRenderer renderer = weaponObj.AddComponent<MeshRenderer>();
        renderer.material = weaponMaterial;

        return weaponObj;
    }

    /// <summary>
    /// Creates a procedural scope GameObject
    /// </summary>
    public GameObject CreateScope(string scopeName = "ThermalScope")
    {
        GameObject scopeObj = new GameObject($"Scope_{scopeName}");

        // Add mesh
        MeshFilter meshFilter = scopeObj.AddComponent<MeshFilter>();
        meshFilter.mesh = ProceduralMeshGenerator.CreateBlockyScope();

        // Add renderer
        MeshRenderer renderer = scopeObj.AddComponent<MeshRenderer>();
        renderer.material = CreateColoredMaterial(scopeColor, "ScopeMaterial");

        return scopeObj;
    }

    /// <summary>
    /// Creates a procedural environment object (crate, barrel, etc.)
    /// </summary>
    public GameObject CreateEnvironmentProp(string propType = "Barrel", Vector3 position = default)
    {
        GameObject propObj = new GameObject($"Prop_{propType}");
        propObj.transform.position = position;

        MeshFilter meshFilter = propObj.AddComponent<MeshFilter>();
        MeshRenderer renderer = propObj.AddComponent<MeshRenderer>();

        switch (propType.ToLower())
        {
            case "barrel":
                meshFilter.mesh = ProceduralMeshGenerator.CreateBarrel(1f, 0.5f);
                renderer.material = CreateColoredMaterial(new Color(0.6f, 0.4f, 0.2f), "BarrelMaterial");
                break;

            case "crate":
                meshFilter.mesh = ProceduralMeshGenerator.CreateBox(new Vector3(1f, 1f, 1f));
                renderer.material = CreateColoredMaterial(woodColor, "CrateMaterial");
                break;

            case "wall":
                meshFilter.mesh = ProceduralMeshGenerator.CreateBox(new Vector3(0.3f, 3f, 5f));
                renderer.material = environmentMaterial;
                break;

            default:
                meshFilter.mesh = ProceduralMeshGenerator.CreateBox(Vector3.one);
                renderer.material = environmentMaterial;
                break;
        }

        // Add collider
        BoxCollider collider = propObj.AddComponent<BoxCollider>();

        // Set layer
        propObj.layer = LayerMask.NameToLayer(Constants.LAYER_ENVIRONMENT);

        return propObj;
    }

    /// <summary>
    /// Creates a simple farm building
    /// </summary>
    public GameObject CreateBuildingStructure(Vector3 position, Vector3 size)
    {
        GameObject building = new GameObject("FarmBuilding");
        building.transform.position = position;

        // Floor
        GameObject floor = new GameObject("Floor");
        floor.transform.parent = building.transform;
        MeshFilter floorMesh = floor.AddComponent<MeshFilter>();
        floorMesh.mesh = ProceduralMeshGenerator.CreateBox(new Vector3(size.x, 0.2f, size.z));
        MeshRenderer floorRenderer = floor.AddComponent<MeshRenderer>();
        floorRenderer.material = CreateColoredMaterial(concreteColor, "FloorMaterial");
        BoxCollider floorCollider = floor.AddComponent<BoxCollider>();

        // Walls
        CreateWall(building.transform, new Vector3(0, size.y / 2f, size.z / 2f), new Vector3(size.x, size.y, 0.3f)); // North
        CreateWall(building.transform, new Vector3(0, size.y / 2f, -size.z / 2f), new Vector3(size.x, size.y, 0.3f)); // South
        CreateWall(building.transform, new Vector3(size.x / 2f, size.y / 2f, 0), new Vector3(0.3f, size.y, size.z)); // East
        CreateWall(building.transform, new Vector3(-size.x / 2f, size.y / 2f, 0), new Vector3(0.3f, size.y, size.z)); // West

        // Roof
        GameObject roof = new GameObject("Roof");
        roof.transform.parent = building.transform;
        roof.transform.localPosition = new Vector3(0, size.y, 0);
        MeshFilter roofMesh = roof.AddComponent<MeshFilter>();
        roofMesh.mesh = ProceduralMeshGenerator.CreateBox(new Vector3(size.x + 0.5f, 0.2f, size.z + 0.5f));
        MeshRenderer roofRenderer = roof.AddComponent<MeshRenderer>();
        roofRenderer.material = CreateColoredMaterial(new Color(0.5f, 0.2f, 0.1f), "RoofMaterial");
        BoxCollider roofCollider = roof.AddComponent<BoxCollider>();

        building.layer = LayerMask.NameToLayer(Constants.LAYER_ENVIRONMENT);

        return building;
    }

    private void CreateWall(Transform parent, Vector3 localPos, Vector3 size)
    {
        GameObject wall = new GameObject("Wall");
        wall.transform.parent = parent;
        wall.transform.localPosition = localPos;

        MeshFilter meshFilter = wall.AddComponent<MeshFilter>();
        meshFilter.mesh = ProceduralMeshGenerator.CreateBox(size);

        MeshRenderer renderer = wall.AddComponent<MeshRenderer>();
        renderer.material = CreateColoredMaterial(new Color(0.7f, 0.6f, 0.5f), "WallMaterial");

        BoxCollider collider = wall.AddComponent<BoxCollider>();

        wall.layer = LayerMask.NameToLayer(Constants.LAYER_ENVIRONMENT);
    }

    /// <summary>
    /// Creates ground plane
    /// </summary>
    public GameObject CreateGround(float width, float depth)
    {
        GameObject ground = new GameObject("Ground");

        MeshFilter meshFilter = ground.AddComponent<MeshFilter>();
        meshFilter.mesh = ProceduralMeshGenerator.CreateGround(width, depth);

        MeshRenderer renderer = ground.AddComponent<MeshRenderer>();
        renderer.material = groundMaterial;

        MeshCollider collider = ground.AddComponent<MeshCollider>();

        ground.layer = LayerMask.NameToLayer(Constants.LAYER_ENVIRONMENT);

        return ground;
    }
}
