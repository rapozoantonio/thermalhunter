using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Procedural mesh generator for creating Minecraft-style blocky 3D models
/// Generates simple geometric shapes programmatically without external assets
/// </summary>
public static class ProceduralMeshGenerator
{
    /// <summary>
    /// Creates a simple box mesh
    /// </summary>
    public static Mesh CreateBox(Vector3 size)
    {
        Mesh mesh = new Mesh { name = "ProceduralBox" };

        Vector3 halfSize = size / 2f;

        // Define vertices for a cube
        Vector3[] vertices = new Vector3[]
        {
            // Front face
            new Vector3(-halfSize.x, -halfSize.y, halfSize.z),
            new Vector3(halfSize.x, -halfSize.y, halfSize.z),
            new Vector3(halfSize.x, halfSize.y, halfSize.z),
            new Vector3(-halfSize.x, halfSize.y, halfSize.z),
            // Back face
            new Vector3(-halfSize.x, -halfSize.y, -halfSize.z),
            new Vector3(halfSize.x, -halfSize.y, -halfSize.z),
            new Vector3(halfSize.x, halfSize.y, -halfSize.z),
            new Vector3(-halfSize.x, halfSize.y, -halfSize.z)
        };

        int[] triangles = new int[]
        {
            // Front
            0, 2, 1, 0, 3, 2,
            // Right
            1, 2, 6, 1, 6, 5,
            // Back
            5, 6, 7, 5, 7, 4,
            // Left
            4, 7, 3, 4, 3, 0,
            // Top
            3, 7, 6, 3, 6, 2,
            // Bottom
            4, 0, 1, 4, 1, 5
        };

        Vector2[] uvs = new Vector2[]
        {
            new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1),
            new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1)
        };

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        return mesh;
    }

    /// <summary>
    /// Creates a blocky rat model (Minecraft-style)
    /// </summary>
    public static Mesh CreateBlockyRat(float size = 1f)
    {
        CombineInstance[] combines = new CombineInstance[4];

        // Body (larger box)
        Mesh body = CreateBox(new Vector3(0.4f * size, 0.2f * size, 0.6f * size));
        Matrix4x4 bodyMatrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.one);
        combines[0].mesh = body;
        combines[0].transform = bodyMatrix;

        // Head (smaller box at front)
        Mesh head = CreateBox(new Vector3(0.25f * size, 0.2f * size, 0.25f * size));
        Matrix4x4 headMatrix = Matrix4x4.TRS(new Vector3(0, 0, 0.35f * size), Quaternion.identity, Vector3.one);
        combines[1].mesh = head;
        combines[1].transform = headMatrix;

        // Tail (thin box at back)
        Mesh tail = CreateBox(new Vector3(0.05f * size, 0.05f * size, 0.4f * size));
        Matrix4x4 tailMatrix = Matrix4x4.TRS(new Vector3(0, 0, -0.5f * size), Quaternion.Euler(30, 0, 0), Vector3.one);
        combines[2].mesh = tail;
        combines[2].transform = tailMatrix;

        // Ears (tiny boxes)
        Mesh ears = CreateBox(new Vector3(0.15f * size, 0.15f * size, 0.05f * size));
        Matrix4x4 earsMatrix = Matrix4x4.TRS(new Vector3(0, 0.15f * size, 0.45f * size), Quaternion.identity, Vector3.one);
        combines[3].mesh = ears;
        combines[3].transform = earsMatrix;

        Mesh combinedMesh = new Mesh { name = "BlockyRat" };
        combinedMesh.CombineMeshes(combines, true, true);
        combinedMesh.RecalculateNormals();
        combinedMesh.RecalculateBounds();

        return combinedMesh;
    }

    /// <summary>
    /// Creates a blocky rifle model (Minecraft-style)
    /// </summary>
    public static Mesh CreateBlockyRifle()
    {
        CombineInstance[] combines = new CombineInstance[5];

        // Stock (back part)
        Mesh stock = CreateBox(new Vector3(0.05f, 0.15f, 0.3f));
        combines[0].mesh = stock;
        combines[0].transform = Matrix4x4.TRS(new Vector3(0, -0.05f, -0.4f), Quaternion.identity, Vector3.one);

        // Receiver (middle part)
        Mesh receiver = CreateBox(new Vector3(0.06f, 0.08f, 0.2f));
        combines[1].mesh = receiver;
        combines[1].transform = Matrix4x4.TRS(new Vector3(0, 0, -0.1f), Quaternion.identity, Vector3.one);

        // Barrel (long front part)
        Mesh barrel = CreateBox(new Vector3(0.03f, 0.03f, 0.5f));
        combines[2].mesh = barrel;
        combines[2].transform = Matrix4x4.TRS(new Vector3(0, 0, 0.3f), Quaternion.identity, Vector3.one);

        // Scope mount
        Mesh scopeMount = CreateBox(new Vector3(0.04f, 0.04f, 0.15f));
        combines[3].mesh = scopeMount;
        combines[3].transform = Matrix4x4.TRS(new Vector3(0, 0.06f, 0f), Quaternion.identity, Vector3.one);

        // Magazine
        Mesh magazine = CreateBox(new Vector3(0.04f, 0.12f, 0.08f));
        combines[4].mesh = magazine;
        combines[4].transform = Matrix4x4.TRS(new Vector3(0, -0.08f, 0f), Quaternion.identity, Vector3.one);

        Mesh combinedMesh = new Mesh { name = "BlockyRifle" };
        combinedMesh.CombineMeshes(combines, true, true);
        combinedMesh.RecalculateNormals();
        combinedMesh.RecalculateBounds();

        return combinedMesh;
    }

    /// <summary>
    /// Creates a blocky thermal scope model
    /// </summary>
    public static Mesh CreateBlockyScope()
    {
        CombineInstance[] combines = new CombineInstance[3];

        // Main tube
        Mesh tube = CreateBox(new Vector3(0.06f, 0.06f, 0.15f));
        combines[0].mesh = tube;
        combines[0].transform = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.one);

        // Front lens
        Mesh frontLens = CreateBox(new Vector3(0.07f, 0.07f, 0.02f));
        combines[1].mesh = frontLens;
        combines[1].transform = Matrix4x4.TRS(new Vector3(0, 0, 0.08f), Quaternion.identity, Vector3.one);

        // Rear lens
        Mesh rearLens = CreateBox(new Vector3(0.05f, 0.05f, 0.02f));
        combines[2].mesh = rearLens;
        combines[2].transform = Matrix4x4.TRS(new Vector3(0, 0, -0.08f), Quaternion.identity, Vector3.one);

        Mesh combinedMesh = new Mesh { name = "BlockyScope" };
        combinedMesh.CombineMeshes(combines, true, true);
        combinedMesh.RecalculateNormals();
        combinedMesh.RecalculateBounds();

        return combinedMesh;
    }

    /// <summary>
    /// Creates a blocky building block (for environment)
    /// </summary>
    public static Mesh CreateBuildingBlock(Vector3 size)
    {
        return CreateBox(size);
    }

    /// <summary>
    /// Creates a blocky barrel/crate prop
    /// </summary>
    public static Mesh CreateBarrel(float height = 1f, float radius = 0.5f)
    {
        // Simplified cylinder using boxes stacked
        int segments = 8;
        CombineInstance[] combines = new CombineInstance[segments];

        for (int i = 0; i < segments; i++)
        {
            float angle = (i * 360f / segments) * Mathf.Deg2Rad;
            float x = Mathf.Cos(angle) * radius * 0.7f;
            float z = Mathf.Sin(angle) * radius * 0.7f;

            Mesh segment = CreateBox(new Vector3(radius * 0.4f, height, radius * 0.2f));
            combines[i].mesh = segment;
            combines[i].transform = Matrix4x4.TRS(
                new Vector3(x, 0, z),
                Quaternion.Euler(0, i * 360f / segments, 0),
                Vector3.one
            );
        }

        Mesh combinedMesh = new Mesh { name = "BlockyBarrel" };
        combinedMesh.CombineMeshes(combines, true, true);
        combinedMesh.RecalculateNormals();
        combinedMesh.RecalculateBounds();

        return combinedMesh;
    }

    /// <summary>
    /// Creates a simple blocky tree
    /// </summary>
    public static Mesh CreateBlockyTree(float height = 3f)
    {
        CombineInstance[] combines = new CombineInstance[2];

        // Trunk
        Mesh trunk = CreateBox(new Vector3(0.3f, height, 0.3f));
        combines[0].mesh = trunk;
        combines[0].transform = Matrix4x4.TRS(new Vector3(0, height / 2f, 0), Quaternion.identity, Vector3.one);

        // Leaves (blocky crown)
        Mesh leaves = CreateBox(new Vector3(1.5f, 1f, 1.5f));
        combines[1].mesh = leaves;
        combines[1].transform = Matrix4x4.TRS(new Vector3(0, height + 0.5f, 0), Quaternion.identity, Vector3.one);

        Mesh combinedMesh = new Mesh { name = "BlockyTree" };
        combinedMesh.CombineMeshes(combines, true, true);
        combinedMesh.RecalculateNormals();
        combinedMesh.RecalculateBounds();

        return combinedMesh;
    }

    /// <summary>
    /// Creates a simple flat plane for ground
    /// </summary>
    public static Mesh CreateGround(float width, float depth)
    {
        Mesh mesh = new Mesh { name = "Ground" };

        Vector3[] vertices = new Vector3[]
        {
            new Vector3(-width/2f, 0, -depth/2f),
            new Vector3(width/2f, 0, -depth/2f),
            new Vector3(width/2f, 0, depth/2f),
            new Vector3(-width/2f, 0, depth/2f)
        };

        int[] triangles = new int[] { 0, 2, 1, 0, 3, 2 };

        Vector2[] uvs = new Vector2[]
        {
            new Vector2(0, 0),
            new Vector2(width, 0),
            new Vector2(width, depth),
            new Vector2(0, depth)
        };

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        return mesh;
    }
}
