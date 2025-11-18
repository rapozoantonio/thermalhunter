using UnityEngine;

/// <summary>
/// Simple particle effect creator for MVP
/// Creates basic particle effects programmatically
/// </summary>
public class SimpleParticleEffect : MonoBehaviour
{
    public enum EffectType
    {
        MuzzleFlash,
        BloodSplatter,
        BulletTracer,
        ShellEjection,
        ImpactWood,
        ImpactMetal,
        ImpactConcrete,
        Sparks,
        Dust
    }

    /// <summary>
    /// Create a simple particle effect at a position
    /// </summary>
    public static GameObject CreateEffect(EffectType effectType, Vector3 position, Vector3 direction)
    {
        GameObject effectObject = new GameObject($"Effect_{effectType}");
        effectObject.transform.position = position;

        ParticleSystem ps = effectObject.AddComponent<ParticleSystem>();
        var main = ps.main;
        var emission = ps.emission;
        var shape = ps.shape;
        var colorOverLifetime = ps.colorOverLifetime;

        // Configure based on effect type
        switch (effectType)
        {
            case EffectType.MuzzleFlash:
                ConfigureMuzzleFlash(ps, direction);
                break;
            case EffectType.BloodSplatter:
                ConfigureBloodSplatter(ps, direction);
                break;
            case EffectType.BulletTracer:
                ConfigureBulletTracer(ps, direction);
                break;
            case EffectType.ShellEjection:
                ConfigureShellEjection(ps, direction);
                break;
            case EffectType.ImpactWood:
                ConfigureImpactWood(ps, direction);
                break;
            case EffectType.ImpactMetal:
                ConfigureImpactMetal(ps, direction);
                break;
            case EffectType.ImpactConcrete:
                ConfigureImpactConcrete(ps, direction);
                break;
            case EffectType.Sparks:
                ConfigureSparks(ps, direction);
                break;
            case EffectType.Dust:
                ConfigureDust(ps, direction);
                break;
        }

        // Auto-destroy after effect finishes
        Destroy(effectObject, main.duration + main.startLifetime.constantMax);

        return effectObject;
    }

    private static void ConfigureMuzzleFlash(ParticleSystem ps, Vector3 direction)
    {
        var main = ps.main;
        main.duration = 0.1f;
        main.startLifetime = 0.05f;
        main.startSpeed = 2f;
        main.startSize = 0.5f;
        main.startColor = new Color(1f, 0.8f, 0.3f, 1f); // Orange-yellow
        main.maxParticles = 10;

        var emission = ps.emission;
        emission.rateOverTime = 0;
        emission.SetBursts(new ParticleSystem.Burst[] { new ParticleSystem.Burst(0f, 10) });

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Cone;
        shape.angle = 15f;
        shape.rotation = Quaternion.LookRotation(direction).eulerAngles;
    }

    private static void ConfigureBloodSplatter(ParticleSystem ps, Vector3 direction)
    {
        var main = ps.main;
        main.duration = 0.5f;
        main.startLifetime = 0.3f;
        main.startSpeed = 3f;
        main.startSize = 0.1f;
        main.startColor = new Color(0.6f, 0f, 0f, 0.8f); // Dark red
        main.maxParticles = 20;
        main.gravityModifier = 1f;

        var emission = ps.emission;
        emission.rateOverTime = 0;
        emission.SetBursts(new ParticleSystem.Burst[] { new ParticleSystem.Burst(0f, 20) });

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Cone;
        shape.angle = 30f;
        shape.rotation = Quaternion.LookRotation(direction).eulerAngles;
    }

    private static void ConfigureBulletTracer(ParticleSystem ps, Vector3 direction)
    {
        var main = ps.main;
        main.duration = 0.1f;
        main.startLifetime = 0.05f;
        main.startSpeed = 100f;
        main.startSize = 0.05f;
        main.startColor = new Color(1f, 1f, 0.5f, 0.5f); // Yellow
        main.maxParticles = 5;

        var emission = ps.emission;
        emission.rateOverTime = 0;
        emission.SetBursts(new ParticleSystem.Burst[] { new ParticleSystem.Burst(0f, 1) });

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Cone;
        shape.angle = 0f;
        shape.rotation = Quaternion.LookRotation(direction).eulerAngles;
    }

    private static void ConfigureShellEjection(ParticleSystem ps, Vector3 direction)
    {
        var main = ps.main;
        main.duration = 0.5f;
        main.startLifetime = 1f;
        main.startSpeed = 2f;
        main.startSize = 0.05f;
        main.startColor = new Color(0.8f, 0.7f, 0.3f, 1f); // Brass
        main.maxParticles = 1;
        main.gravityModifier = 1f;

        var emission = ps.emission;
        emission.rateOverTime = 0;
        emission.SetBursts(new ParticleSystem.Burst[] { new ParticleSystem.Burst(0f, 1) });

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Cone;
        shape.angle = 45f;
    }

    private static void ConfigureImpactWood(ParticleSystem ps, Vector3 direction)
    {
        var main = ps.main;
        main.duration = 0.3f;
        main.startLifetime = 0.5f;
        main.startSpeed = 2f;
        main.startSize = 0.05f;
        main.startColor = new Color(0.6f, 0.4f, 0.2f, 0.8f); // Brown
        main.maxParticles = 15;
        main.gravityModifier = 0.5f;

        var emission = ps.emission;
        emission.rateOverTime = 0;
        emission.SetBursts(new ParticleSystem.Burst[] { new ParticleSystem.Burst(0f, 15) });

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Cone;
        shape.angle = 45f;
        shape.rotation = Quaternion.LookRotation(direction).eulerAngles;
    }

    private static void ConfigureImpactMetal(ParticleSystem ps, Vector3 direction)
    {
        var main = ps.main;
        main.duration = 0.2f;
        main.startLifetime = 0.3f;
        main.startSpeed = 3f;
        main.startSize = 0.03f;
        main.startColor = new Color(0.7f, 0.7f, 0.7f, 1f); // Gray
        main.maxParticles = 10;
        main.gravityModifier = 1f;

        var emission = ps.emission;
        emission.rateOverTime = 0;
        emission.SetBursts(new ParticleSystem.Burst[] { new ParticleSystem.Burst(0f, 10) });

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Cone;
        shape.angle = 30f;
        shape.rotation = Quaternion.LookRotation(direction).eulerAngles;
    }

    private static void ConfigureImpactConcrete(ParticleSystem ps, Vector3 direction)
    {
        var main = ps.main;
        main.duration = 0.4f;
        main.startLifetime = 0.6f;
        main.startSpeed = 2.5f;
        main.startSize = 0.04f;
        main.startColor = new Color(0.5f, 0.5f, 0.5f, 0.7f); // Light gray
        main.maxParticles = 20;
        main.gravityModifier = 0.8f;

        var emission = ps.emission;
        emission.rateOverTime = 0;
        emission.SetBursts(new ParticleSystem.Burst[] { new ParticleSystem.Burst(0f, 20) });

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Cone;
        shape.angle = 40f;
        shape.rotation = Quaternion.LookRotation(direction).eulerAngles;
    }

    private static void ConfigureSparks(ParticleSystem ps, Vector3 direction)
    {
        var main = ps.main;
        main.duration = 0.3f;
        main.startLifetime = 0.4f;
        main.startSpeed = 4f;
        main.startSize = 0.02f;
        main.startColor = new Color(1f, 0.8f, 0.3f, 1f); // Orange-yellow
        main.maxParticles = 25;
        main.gravityModifier = 1.5f;

        var emission = ps.emission;
        emission.rateOverTime = 0;
        emission.SetBursts(new ParticleSystem.Burst[] { new ParticleSystem.Burst(0f, 25) });

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Cone;
        shape.angle = 50f;
        shape.rotation = Quaternion.LookRotation(direction).eulerAngles;
    }

    private static void ConfigureDust(ParticleSystem ps, Vector3 direction)
    {
        var main = ps.main;
        main.duration = 1f;
        main.startLifetime = 1.5f;
        main.startSpeed = 0.5f;
        main.startSize = 0.1f;
        main.startColor = new Color(0.6f, 0.6f, 0.5f, 0.3f); // Beige, semi-transparent
        main.maxParticles = 30;
        main.gravityModifier = -0.1f; // Slight upward drift

        var emission = ps.emission;
        emission.rateOverTime = 0;
        emission.SetBursts(new ParticleSystem.Burst[] { new ParticleSystem.Burst(0f, 30) });

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Sphere;
        shape.radius = 0.5f;
    }
}
