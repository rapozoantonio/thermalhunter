using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Utility extension methods for common operations
/// </summary>
public static class Extensions
{
    // === Vector3 Extensions ===

    /// <summary>
    /// Returns a Vector3 with only the XZ components (sets Y to 0)
    /// </summary>
    public static Vector3 Flatten(this Vector3 vector)
    {
        return new Vector3(vector.x, 0, vector.z);
    }

    /// <summary>
    /// Returns the horizontal distance to another Vector3 (ignoring Y)
    /// </summary>
    public static float HorizontalDistance(this Vector3 from, Vector3 to)
    {
        return Vector3.Distance(from.Flatten(), to.Flatten());
    }

    /// <summary>
    /// Returns a random point within a radius
    /// </summary>
    public static Vector3 RandomPointInRadius(this Vector3 center, float radius)
    {
        Vector2 randomCircle = Random.insideUnitCircle * radius;
        return center + new Vector3(randomCircle.x, 0, randomCircle.y);
    }

    // === Transform Extensions ===

    /// <summary>
    /// Resets the transform to default values
    /// </summary>
    public static void Reset(this Transform transform)
    {
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        transform.localScale = Vector3.one;
    }

    /// <summary>
    /// Destroys all children of this transform
    /// </summary>
    public static void DestroyChildren(this Transform transform)
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Object.Destroy(transform.GetChild(i).gameObject);
        }
    }

    /// <summary>
    /// Gets all children as a list
    /// </summary>
    public static List<Transform> GetChildren(this Transform transform)
    {
        List<Transform> children = new List<Transform>();
        for (int i = 0; i < transform.childCount; i++)
        {
            children.Add(transform.GetChild(i));
        }
        return children;
    }

    // === GameObject Extensions ===

    /// <summary>
    /// Gets or adds a component
    /// </summary>
    public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
    {
        T component = gameObject.GetComponent<T>();
        if (component == null)
        {
            component = gameObject.AddComponent<T>();
        }
        return component;
    }

    /// <summary>
    /// Sets the layer recursively for all children
    /// </summary>
    public static void SetLayerRecursive(this GameObject gameObject, int layer)
    {
        gameObject.layer = layer;
        foreach (Transform child in gameObject.transform)
        {
            child.gameObject.SetLayerRecursive(layer);
        }
    }

    // === List Extensions ===

    /// <summary>
    /// Returns a random element from the list
    /// </summary>
    public static T RandomElement<T>(this List<T> list)
    {
        if (list == null || list.Count == 0)
            return default(T);

        return list[Random.Range(0, list.Count)];
    }

    /// <summary>
    /// Shuffles the list in place
    /// </summary>
    public static void Shuffle<T>(this List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            T temp = list[i];
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }

    // === Float Extensions ===

    /// <summary>
    /// Remaps a value from one range to another
    /// </summary>
    public static float Remap(this float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }

    /// <summary>
    /// Clamps a value between 0 and 1
    /// </summary>
    public static float Clamp01(this float value)
    {
        return Mathf.Clamp01(value);
    }

    // === Color Extensions ===

    /// <summary>
    /// Returns a color with modified alpha
    /// </summary>
    public static Color WithAlpha(this Color color, float alpha)
    {
        return new Color(color.r, color.g, color.b, alpha);
    }

    // === LayerMask Extensions ===

    /// <summary>
    /// Checks if a LayerMask contains a specific layer
    /// </summary>
    public static bool Contains(this LayerMask layerMask, int layer)
    {
        return (layerMask.value & (1 << layer)) != 0;
    }

    // === String Extensions ===

    /// <summary>
    /// Truncates a string to a maximum length
    /// </summary>
    public static string Truncate(this string value, int maxLength)
    {
        if (string.IsNullOrEmpty(value)) return value;
        return value.Length <= maxLength ? value : value.Substring(0, maxLength);
    }

    // === Coroutine Utilities ===

    /// <summary>
    /// Invokes an action after a delay
    /// </summary>
    public static void InvokeDelayed(this MonoBehaviour monoBehaviour, System.Action action, float delay)
    {
        monoBehaviour.StartCoroutine(InvokeDelayedCoroutine(action, delay));
    }

    private static System.Collections.IEnumerator InvokeDelayedCoroutine(System.Action action, float delay)
    {
        yield return new WaitForSeconds(delay);
        action?.Invoke();
    }
}
