using UnityEngine;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Icon generator for Thermal Hunt
/// Generates basic app icons programmatically for MVP
/// Replace with professional designs for final release
/// </summary>
public class IconGenerator : MonoBehaviour
{
    public static Texture2D GenerateAppIcon(int size)
    {
        Texture2D icon = new Texture2D(size, size, TextureFormat.RGBA32, false);

        // Background - dark blue/black gradient
        Color darkBlue = new Color(0.1f, 0.1f, 0.2f, 1f);
        Color black = new Color(0f, 0f, 0f, 1f);

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float gradient = (float)y / size;
                Color pixelColor = Color.Lerp(darkBlue, black, gradient);
                icon.SetPixel(x, y, pixelColor);
            }
        }

        // Draw thermal scope circle (center)
        int centerX = size / 2;
        int centerY = size / 2;
        int radius = (int)(size * 0.35f);

        Color scopeEdge = new Color(0.3f, 0.3f, 0.4f, 1f);
        DrawCircle(icon, centerX, centerY, radius, scopeEdge, 3);

        // Draw crosshair
        Color crosshairColor = new Color(1f, 0.2f, 0.2f, 1f); // Red
        int crosshairLength = radius / 2;
        int crosshairThickness = 2;

        // Horizontal line
        DrawLine(icon, centerX - crosshairLength, centerY, centerX + crosshairLength, centerY, crosshairColor, crosshairThickness);
        // Vertical line
        DrawLine(icon, centerX, centerY - crosshairLength, centerX, centerY + crosshairLength, crosshairColor, crosshairThickness);

        // Draw rat silhouette (hot signature)
        int ratX = centerX + (int)(radius * 0.3f);
        int ratY = centerY - (int)(radius * 0.2f);
        int ratSize = radius / 3;

        Color hotColor = new Color(1f, 1f, 0.7f, 1f); // Bright yellow-white
        DrawRatSilhouette(icon, ratX, ratY, ratSize, hotColor);

        // Add text "TH" at bottom (optional)
        // (Text rendering would require a font texture - skip for MVP)

        icon.Apply();
        return icon;
    }

    private static void DrawCircle(Texture2D texture, int centerX, int centerY, int radius, Color color, int thickness)
    {
        for (int angle = 0; angle < 360; angle++)
        {
            float rad = angle * Mathf.Deg2Rad;
            int x = centerX + (int)(Mathf.Cos(rad) * radius);
            int y = centerY + (int)(Mathf.Sin(rad) * radius);

            for (int t = -thickness/2; t <= thickness/2; t++)
            {
                int tx = x + (int)(Mathf.Cos(rad) * t);
                int ty = y + (int)(Mathf.Sin(rad) * t);
                if (tx >= 0 && tx < texture.width && ty >= 0 && ty < texture.height)
                {
                    texture.SetPixel(tx, ty, color);
                }
            }
        }
    }

    private static void DrawLine(Texture2D texture, int x0, int y0, int x1, int y1, Color color, int thickness)
    {
        int dx = Mathf.Abs(x1 - x0);
        int dy = Mathf.Abs(y1 - y0);
        int sx = x0 < x1 ? 1 : -1;
        int sy = y0 < y1 ? 1 : -1;
        int err = dx - dy;

        while (true)
        {
            // Draw thick line
            for (int tx = -thickness/2; tx <= thickness/2; tx++)
            {
                for (int ty = -thickness/2; ty <= thickness/2; ty++)
                {
                    int px = x0 + tx;
                    int py = y0 + ty;
                    if (px >= 0 && px < texture.width && py >= 0 && py < texture.height)
                    {
                        texture.SetPixel(px, py, color);
                    }
                }
            }

            if (x0 == x1 && y0 == y1) break;

            int e2 = 2 * err;
            if (e2 > -dy)
            {
                err -= dy;
                x0 += sx;
            }
            if (e2 < dx)
            {
                err += dx;
                y0 += sy;
            }
        }
    }

    private static void DrawRatSilhouette(Texture2D texture, int x, int y, int size, Color color)
    {
        // Simple rat shape: body (ellipse) + head (circle) + tail (line)

        // Body
        DrawFilledEllipse(texture, x, y, size, size/2, color);

        // Head
        DrawFilledCircle(texture, x + size/2, y, size/3, color);

        // Ear
        DrawFilledCircle(texture, x + size/2 + size/6, y + size/6, size/6, color);
    }

    private static void DrawFilledCircle(Texture2D texture, int centerX, int centerY, int radius, Color color)
    {
        for (int y = -radius; y <= radius; y++)
        {
            for (int x = -radius; x <= radius; x++)
            {
                if (x * x + y * y <= radius * radius)
                {
                    int px = centerX + x;
                    int py = centerY + y;
                    if (px >= 0 && px < texture.width && py >= 0 && py < texture.height)
                    {
                        texture.SetPixel(px, py, color);
                    }
                }
            }
        }
    }

    private static void DrawFilledEllipse(Texture2D texture, int centerX, int centerY, int radiusX, int radiusY, Color color)
    {
        for (int y = -radiusY; y <= radiusY; y++)
        {
            for (int x = -radiusX; x <= radiusX; x++)
            {
                float nx = (float)x / radiusX;
                float ny = (float)y / radiusY;
                if (nx * nx + ny * ny <= 1f)
                {
                    int px = centerX + x;
                    int py = centerY + y;
                    if (px >= 0 && px < texture.width && py >= 0 && py < texture.height)
                    {
                        texture.SetPixel(px, py, color);
                    }
                }
            }
        }
    }

    #if UNITY_EDITOR
    [MenuItem("Thermal Hunt/Generate App Icons")]
    public static void GenerateAllIcons()
    {
        string iconsPath = Path.Combine(Application.dataPath, "_Game", "Icons");
        if (!Directory.Exists(iconsPath))
        {
            Directory.CreateDirectory(iconsPath);
        }

        // iOS icon sizes
        int[] iosSizes = { 20, 29, 40, 58, 60, 76, 80, 87, 120, 152, 167, 180, 1024 };

        // Android icon sizes
        int[] androidSizes = { 48, 72, 96, 144, 192, 512 };

        Debug.Log("[IconGenerator] Generating iOS icons...");
        foreach (int size in iosSizes)
        {
            Texture2D icon = GenerateAppIcon(size);
            byte[] bytes = icon.EncodeToPNG();
            string filename = Path.Combine(iconsPath, $"icon_ios_{size}x{size}.png");
            File.WriteAllBytes(filename, bytes);
            Debug.Log($"Generated: icon_ios_{size}x{size}.png");
        }

        Debug.Log("[IconGenerator] Generating Android icons...");
        foreach (int size in androidSizes)
        {
            Texture2D icon = GenerateAppIcon(size);
            byte[] bytes = icon.EncodeToPNG();
            string filename = Path.Combine(iconsPath, $"icon_android_{size}x{size}.png");
            File.WriteAllBytes(filename, bytes);
            Debug.Log($"Generated: icon_android_{size}x{size}.png");
        }

        AssetDatabase.Refresh();
        EditorUtility.DisplayDialog("Icon Generation Complete",
            $"Generated {iosSizes.Length + androidSizes.Length} app icons!\n\nLocation: Assets/_Game/Icons/",
            "OK");
    }
    #endif
}
