using UnityEngine;

/// <summary>
/// ScriptableObject for weapon configuration data
/// Contains all weapon stats and properties
/// </summary>
[CreateAssetMenu(fileName = "NewWeapon", menuName = "ThermalHunt/Weapon Data")]
public class WeaponData : ScriptableObject
{
    [Header("Weapon Info")]
    public string weaponID;
    public string weaponName;
    [TextArea(2, 4)] public string description;
    public Sprite icon;
    public GameObject weaponPrefab;

    [Header("Ballistics")]
    public int damage = 1;
    public float muzzleVelocity = 300f; // m/s
    public float maxRange = 100f;
    public float accuracy = 0.95f;
    public float recoilAmount = 0.1f;

    [Header("Fire Rate")]
    public float fireRate = 1f; // Shots per second
    public bool isAutomatic = false;

    [Header("Sound")]
    [Range(0f, 1f)] public float soundLevel = 0.3f; // How loud (0 = silent, 1 = very loud)
    public AudioClip fireSound;
    public AudioClip reloadSound;

    [Header("Score")]
    public int baseScore = 100;
    public int headshotBonus = 50;

    [Header("Unlock Requirements")]
    public int requiredLevel = 1;
    public bool isPremium = false;
    public float price = 0f; // For IAP

    // Calculated properties
    public float FireDelay => 1f / fireRate;

    // Validation
    private void OnValidate()
    {
        if (string.IsNullOrEmpty(weaponID))
        {
            weaponID = name;
        }
    }
}
