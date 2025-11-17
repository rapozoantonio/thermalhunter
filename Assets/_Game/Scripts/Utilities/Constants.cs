/// <summary>
/// Global constants for the game
/// Centralizes magic numbers and configuration values
/// </summary>
public static class Constants
{
    // === Layer Names ===
    public const string LAYER_PLAYER = "Player";
    public const string LAYER_TARGET = "Target";
    public const string LAYER_THERMAL_VISIBLE = "ThermalVisible";
    public const string LAYER_ENVIRONMENT = "Environment";
    public const string LAYER_UI = "UI";

    // === Tag Names ===
    public const string TAG_PLAYER = "Player";
    public const string TAG_RAT = "Rat";
    public const string TAG_WEAK_POINT = "WeakPoint";
    public const string TAG_COVER = "Cover";

    // === Scene Names ===
    public const string SCENE_BOOT = "Boot";
    public const string SCENE_MAIN_MENU = "MainMenu";
    public const string SCENE_GAME = "Game";
    public const string SCENE_LOADING = "Loading";

    // === Game Balance ===
    public const int DEFAULT_AMMO = 15;
    public const float DEFAULT_BATTERY_LIFE = 180f; // 3 minutes
    public const float DEFAULT_MISSION_TIME = 300f; // 5 minutes
    public const int BASE_SCORE_PER_KILL = 100;
    public const int HEADSHOT_BONUS = 50;
    public const float SOUND_PROPAGATION_SPEED = 343f; // m/s

    // === Thermal Settings ===
    public const float THERMAL_NOISE_INTENSITY = 0.02f;
    public const float THERMAL_SCAN_LINE_SPEED = 2f;
    public const int THERMAL_MOBILE_RESOLUTION = 512;
    public const int THERMAL_PC_RESOLUTION = 1024;

    // === AI Settings ===
    public const float AI_HEARING_RANGE = 8f;
    public const float AI_DETECTION_ANGLE = 120f;
    public const float AI_ALERT_RADIUS = 5f;
    public const float AI_PATROL_RADIUS = 10f;

    // === Performance ===
    public const int OBJECT_POOL_INITIAL_SIZE = 20;
    public const int MAX_CONCURRENT_RATS = 20;
    public const float AI_LOD_HIGH_DISTANCE = 20f;
    public const float AI_LOD_MEDIUM_DISTANCE = 40f;
    public const float AI_LOD_LOW_DISTANCE = 60f;

    // === Monetization ===
    public const int MAX_ADS_PER_SESSION = 5;
    public const int SESSIONS_UNTIL_INTERSTITIAL = 3;
    public const float IAP_REMOVE_ADS_PRICE = 4.99f;
    public const float IAP_PREMIUM_SCOPES_PRICE = 2.99f;
    public const float IAP_STARTER_PACK_PRICE = 9.99f;

    // === Save Data ===
    public const string SAVE_FILE_NAME = "ThermalHuntSave.json";
    public const int SAVE_VERSION = 1;

    // === Analytics Events ===
    public const string EVENT_MISSION_START = "mission_start";
    public const string EVENT_MISSION_COMPLETE = "mission_complete";
    public const string EVENT_MISSION_FAIL = "mission_fail";
    public const string EVENT_SHOT_FIRED = "shot_fired";
    public const string EVENT_TARGET_HIT = "target_hit";
    public const string EVENT_AD_WATCHED = "ad_watched";
    public const string EVENT_PURCHASE = "purchase";
    public const string EVENT_LEVEL_UP = "level_up";

    // === Platform IDs ===
#if UNITY_IOS
    public const string UNITY_GAME_ID = "YOUR_IOS_ID";
    public const string REWARDED_AD_UNIT = "Rewarded_iOS";
    public const string INTERSTITIAL_AD_UNIT = "Interstitial_iOS";
#elif UNITY_ANDROID
    public const string UNITY_GAME_ID = "YOUR_ANDROID_ID";
    public const string REWARDED_AD_UNIT = "Rewarded_Android";
    public const string INTERSTITIAL_AD_UNIT = "Interstitial_Android";
#else
    public const string UNITY_GAME_ID = "NOT_CONFIGURED";
    public const string REWARDED_AD_UNIT = "NOT_CONFIGURED";
    public const string INTERSTITIAL_AD_UNIT = "NOT_CONFIGURED";
#endif

    // === IAP Product IDs ===
    public const string IAP_REMOVE_ADS = "com.thermalhunt.removeads";
    public const string IAP_PREMIUM_SCOPE_PACK = "com.thermalhunt.premiumscopes";
    public const string IAP_STARTER_PACK = "com.thermalhunt.starterpack";
}
