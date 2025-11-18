#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;
using System.IO;

/// <summary>
/// Build automation tool for Thermal Hunt
/// Automates iOS and Android build processes
/// </summary>
public class BuildAutomation : EditorWindow
{
    private static string bundleIdentifier = "com.yourstudio.thermalhunter";
    private static string version = "1.0.0";
    private static int buildNumber = 1;
    private static string companyName = "Your Studio";
    private static string productName = "Thermal Hunt";

    [MenuItem("Thermal Hunt/Build Automation")]
    public static void ShowWindow()
    {
        GetWindow<BuildAutomation>("Build Automation");
    }

    private void OnGUI()
    {
        GUILayout.Label("Thermal Hunt - Build Automation", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        // Common settings
        GUILayout.Label("Common Settings", EditorStyles.boldLabel);
        bundleIdentifier = EditorGUILayout.TextField("Bundle Identifier", bundleIdentifier);
        version = EditorGUILayout.TextField("Version", version);
        buildNumber = EditorGUILayout.IntField("Build Number", buildNumber);
        companyName = EditorGUILayout.TextField("Company Name", companyName);
        productName = EditorGUILayout.TextField("Product Name", productName);

        EditorGUILayout.Space();

        // iOS Build
        GUILayout.Label("iOS Build", EditorStyles.boldLabel);
        if (GUILayout.Button("Configure iOS Settings", GUILayout.Height(30)))
        {
            ConfigureIOSSettings();
        }
        if (GUILayout.Button("Build iOS (Xcode Project)", GUILayout.Height(30)))
        {
            BuildIOS();
        }

        EditorGUILayout.Space();

        // Android Build
        GUILayout.Label("Android Build", EditorStyles.boldLabel);
        if (GUILayout.Button("Configure Android Settings", GUILayout.Height(30)))
        {
            ConfigureAndroidSettings();
        }
        if (GUILayout.Button("Build Android (.aab)", GUILayout.Height(30)))
        {
            BuildAndroid();
        }

        EditorGUILayout.Space();

        // Utilities
        GUILayout.Label("Utilities", EditorStyles.boldLabel);
        if (GUILayout.Button("Apply All Settings", GUILayout.Height(25)))
        {
            ApplyCommonSettings();
            ConfigureIOSSettings();
            ConfigureAndroidSettings();
        }
        if (GUILayout.Button("Generate App Icons", GUILayout.Height(25)))
        {
            IconGenerator.GenerateAllIcons();
        }
        if (GUILayout.Button("Open Build Folder", GUILayout.Height(25)))
        {
            string buildPath = Path.Combine(Application.dataPath, "..", "Builds");
            EditorUtility.RevealInFinder(buildPath);
        }
    }

    private static void ApplyCommonSettings()
    {
        PlayerSettings.companyName = companyName;
        PlayerSettings.productName = productName;
        PlayerSettings.bundleVersion = version;

        Debug.Log("[BuildAutomation] Common settings applied");
        EditorUtility.DisplayDialog("Settings Applied", "Common player settings have been configured.", "OK");
    }

    private static void ConfigureIOSSettings()
    {
        PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.iOS, bundleIdentifier);
        PlayerSettings.iOS.buildNumber = buildNumber.ToString();
        PlayerSettings.iOS.targetOSVersionString = "14.0";

        // Scripting
        PlayerSettings.SetScriptingBackend(BuildTargetGroup.iOS, ScriptingImplementation.IL2CPP);
        PlayerSettings.SetArchitecture(BuildTargetGroup.iOS, 1); // ARM64

        // Graphics
        PlayerSettings.iOS.targetDevice = iOSTargetDevice.iPhoneAndiPad;

        // Capabilities
        PlayerSettings.iOS.requiresPersistentWiFi = false;

        // Optimization
        PlayerSettings.stripEngineCode = true;
        PlayerSettings.SetManagedStrippingLevel(BuildTargetGroup.iOS, ManagedStrippingLevel.Medium);

        Debug.Log("[BuildAutomation] iOS settings configured");
        EditorUtility.DisplayDialog("iOS Settings", "iOS build settings have been configured.", "OK");
    }

    private static void ConfigureAndroidSettings()
    {
        PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, bundleIdentifier);
        PlayerSettings.Android.bundleVersionCode = buildNumber;
        PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel21;
        PlayerSettings.Android.targetSdkVersion = AndroidSdkVersions.AndroidApiLevel33;

        // Scripting
        PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);
        PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;

        // Build
        EditorUserBuildSettings.buildAppBundle = true; // .aab format

        // Optimization
        PlayerSettings.stripEngineCode = true;
        PlayerSettings.SetManagedStrippingLevel(BuildTargetGroup.Android, ManagedStrippingLevel.Medium);

        // Keystore (note: user must set up their own keystore)
        Debug.LogWarning("[BuildAutomation] Remember to configure your keystore in Publishing Settings!");

        Debug.Log("[BuildAutomation] Android settings configured");
        EditorUtility.DisplayDialog("Android Settings",
            "Android build settings have been configured.\n\nRemember to set up your keystore in Publishing Settings!",
            "OK");
    }

    [MenuItem("Thermal Hunt/Build/iOS")]
    public static void BuildIOS()
    {
        ApplyCommonSettings();
        ConfigureIOSSettings();

        string buildPath = Path.Combine(Application.dataPath, "..", "Builds", "iOS");

        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        buildPlayerOptions.scenes = GetEnabledScenes();
        buildPlayerOptions.locationPathName = buildPath;
        buildPlayerOptions.target = BuildTarget.iOS;
        buildPlayerOptions.options = BuildOptions.None;

        BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
        BuildSummary summary = report.summary;

        if (summary.result == BuildResult.Succeeded)
        {
            Debug.Log($"[BuildAutomation] iOS build succeeded: {summary.totalSize} bytes");
            EditorUtility.DisplayDialog("Build Successful",
                $"iOS build completed successfully!\n\nSize: {summary.totalSize / (1024 * 1024)} MB\nLocation: {buildPath}\n\nOpen in Xcode to continue.",
                "OK");
        }
        else if (summary.result == BuildResult.Failed)
        {
            Debug.LogError("[BuildAutomation] iOS build failed");
            EditorUtility.DisplayDialog("Build Failed", "iOS build failed. Check the console for errors.", "OK");
        }
    }

    [MenuItem("Thermal Hunt/Build/Android")]
    public static void BuildAndroid()
    {
        ApplyCommonSettings();
        ConfigureAndroidSettings();

        string buildPath = Path.Combine(Application.dataPath, "..", "Builds", "Android", $"ThermalHunt_v{version}_{buildNumber}.aab");

        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        buildPlayerOptions.scenes = GetEnabledScenes();
        buildPlayerOptions.locationPathName = buildPath;
        buildPlayerOptions.target = BuildTarget.Android;
        buildPlayerOptions.options = BuildOptions.None;

        BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
        BuildSummary summary = report.summary;

        if (summary.result == BuildResult.Succeeded)
        {
            Debug.Log($"[BuildAutomation] Android build succeeded: {summary.totalSize} bytes");
            EditorUtility.DisplayDialog("Build Successful",
                $"Android build completed successfully!\n\nSize: {summary.totalSize / (1024 * 1024)} MB\nLocation: {buildPath}\n\nUpload this .aab file to Google Play Console.",
                "OK");
        }
        else if (summary.result == BuildResult.Failed)
        {
            Debug.LogError("[BuildAutomation] Android build failed");
            EditorUtility.DisplayDialog("Build Failed", "Android build failed. Check the console for errors.", "OK");
        }
    }

    private static string[] GetEnabledScenes()
    {
        var scenes = new string[EditorBuildSettings.scenes.Length];
        for (int i = 0; i < scenes.Length; i++)
        {
            scenes[i] = EditorBuildSettings.scenes[i].path;
        }
        return scenes;
    }
}
#endif
