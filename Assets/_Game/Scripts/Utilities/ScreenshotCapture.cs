using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Screenshot capture tool for creating app store screenshots
/// Use this to capture high-quality screenshots during gameplay
/// </summary>
public class ScreenshotCapture : MonoBehaviour
{
    [Header("Screenshot Settings")]
    [SerializeField] private KeyCode screenshotKey = KeyCode.F12;
    [SerializeField] private int superSize = 2; // 2x or 4x for higher quality
    [SerializeField] private string screenshotFolder = "Screenshots";

    [Header("Presets")]
    [SerializeField] private ScreenshotPreset currentPreset = ScreenshotPreset.Custom;

    private string folderPath;
    private int screenshotCount = 0;

    private void Start()
    {
        folderPath = System.IO.Path.Combine(Application.dataPath, "..", screenshotFolder);

        if (!System.IO.Directory.Exists(folderPath))
        {
            System.IO.Directory.CreateDirectory(folderPath);
        }

        Debug.Log($"[ScreenshotCapture] Screenshots will be saved to: {folderPath}");
        Debug.Log($"[ScreenshotCapture] Press {screenshotKey} to capture screenshot");
    }

    private void Update()
    {
        if (Input.GetKeyDown(screenshotKey))
        {
            CaptureScreenshot();
        }

        // Quick presets
        if (Input.GetKeyDown(KeyCode.F9))
        {
            CapturePreset(ScreenshotPreset.iPhone14ProMax);
        }
        if (Input.GetKeyDown(KeyCode.F10))
        {
            CapturePreset(ScreenshotPreset.AndroidPhone);
        }
    }

    public void CaptureScreenshot()
    {
        string timestamp = System.DateTime.Now.ToString("yyyyMMdd_HHmmss");
        string filename = $"Screenshot_{timestamp}_{screenshotCount:D3}.png";
        string fullPath = System.IO.Path.Combine(folderPath, filename);

        ScreenCapture.CaptureScreenshot(fullPath, superSize);
        screenshotCount++;

        Debug.Log($"[ScreenshotCapture] Screenshot saved: {filename} (supersize: {superSize}x)");

        #if UNITY_EDITOR
        EditorUtility.DisplayDialog("Screenshot Captured",
            $"Screenshot saved to:\n{fullPath}\n\nSupersize: {superSize}x", "OK");
        #endif
    }

    public void CapturePreset(ScreenshotPreset preset)
    {
        int width = 1920;
        int height = 1080;

        switch (preset)
        {
            case ScreenshotPreset.iPhone14ProMax:
                width = 1290;
                height = 2796;
                break;
            case ScreenshotPreset.iPhone11ProMax:
                width = 1242;
                height = 2688;
                break;
            case ScreenshotPreset.iPhone8Plus:
                width = 1242;
                height = 2208;
                break;
            case ScreenshotPreset.AndroidPhone:
                width = 1080;
                height = 1920;
                break;
            case ScreenshotPreset.AndroidPhone_19_9:
                width = 1080;
                height = 2340;
                break;
        }

        CaptureResolution(width, height, preset.ToString());
    }

    private void CaptureResolution(int width, int height, string presetName)
    {
        string timestamp = System.DateTime.Now.ToString("yyyyMMdd_HHmmss");
        string filename = $"Screenshot_{presetName}_{width}x{height}_{timestamp}.png";
        string fullPath = System.IO.Path.Combine(folderPath, filename);

        // Render to texture at specific resolution
        RenderTexture rt = new RenderTexture(width, height, 24);
        Camera cam = Camera.main;

        if (cam == null)
        {
            Debug.LogError("[ScreenshotCapture] No main camera found!");
            return;
        }

        RenderTexture currentRT = cam.targetTexture;
        cam.targetTexture = rt;

        Texture2D screenshot = new Texture2D(width, height, TextureFormat.RGB24, false);
        cam.Render();

        RenderTexture.active = rt;
        screenshot.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        screenshot.Apply();

        cam.targetTexture = currentRT;
        RenderTexture.active = null;
        Destroy(rt);

        byte[] bytes = screenshot.EncodeToPNG();
        System.IO.File.WriteAllBytes(fullPath, bytes);

        Debug.Log($"[ScreenshotCapture] Screenshot saved: {filename} ({width}×{height})");

        #if UNITY_EDITOR
        EditorUtility.DisplayDialog("Screenshot Captured",
            $"Screenshot saved:\n{filename}\n\nResolution: {width}×{height}", "OK");
        #endif
    }

    public enum ScreenshotPreset
    {
        Custom,
        iPhone14ProMax,     // 1290×2796
        iPhone11ProMax,     // 1242×2688
        iPhone8Plus,        // 1242×2208
        AndroidPhone,       // 1080×1920 (16:9)
        AndroidPhone_19_9   // 1080×2340 (19.5:9)
    }
}
