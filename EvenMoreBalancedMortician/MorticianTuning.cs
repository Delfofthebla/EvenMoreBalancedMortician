using System;
using EvenMoreBalancedMortician.Presets;
using EvenMoreBalancedMortician.Scepter;
using Morris;
using Morris.Components;
using RoR2;
using RoR2.Projectile;
using RoR2.Skills;
using SkillStates.Ghoul;
using SkillStates.Morris;
using SkillStates.SharedStates;
using UnityEngine;

namespace EvenMoreBalancedMortician;

internal static class MorticianTuning
{
    public static void Apply(MorticianSettings settings)
    {
        ApplyToBodies("Mortician", MorrisPlugin.MorrisBodyPrefab, MorrisPlugin.MorrisBodyIndex, body => ApplyMorticianStats(body, settings));
        ApplyCooldowns(settings);
        ApplyShovel(settings);
        ApplyGhouls(settings);
        ApplySacrifice(settings);
        ApplyTombstone(settings);
    }

    private static void ApplyShovel(MorticianSettings settings)
    {
        SwingShovel.damageCoefficient = AsCoefficient(settings.ShovelDamagePercent);
        BaseLaunchedState.damageCoefficient = AsCoefficient(settings.LaunchDamagePercent);
    }

    private static void ApplyGhouls(MorticianSettings settings)
    {
        GhoulMelee.damageCoefficient = AsCoefficient(settings.GhoulBiteDamagePercent);
        ClingState.damageCoefficient = AsCoefficient(settings.GhoulBiteDamagePercent);
        BileSpit.damageCoefficient = AsCoefficient(settings.GhoulSpitDamagePercent);
        ApplySpitProcCoefficient(settings);

        ApplyToBodies("Ghoul", MorrisPlugin.GhoulBodyPrefab, MorrisPlugin.GhoulBodyIndex, body => ApplyGhoulStats(body, settings));
    }

    private static void ApplySacrifice(MorticianSettings settings)
    {
        GhoulDeath.sacrificedDamageCoefficient = AsCoefficient(settings.SacrificeDamagePercent);
        GhoulDeath.sacrificedRadius = settings.SacrificeRadius.Value;
        Sacrifice.sacrificePercentHealAmount = AsCoefficient(settings.SacrificeHealPercent);
    }

    private static void ApplyTombstone(MorticianSettings settings)
    {
        TombstoneController.soulOrbDamage = AsCoefficient(settings.SoulOrbDamagePercent);
        TombstoneController.spawnTime = settings.TombstoneGhoulSpawnInterval.Value;

        ApplyToBodies("Tombstone", MorrisPlugin.TombstoneBodyPrefab, MorrisPlugin.TombstoneBodyIndex, body => ApplyDamageStats(body, settings.TombstoneBaseDamage, settings.TombstoneDamagePerLevel));
    }

    private static void ApplyCooldowns(MorticianSettings settings)
    {
        var skills = MorrisPlugin.MorrisBodyPrefab ? MorrisPlugin.MorrisBodyPrefab.GetComponent<SkillLocator>() : null;
        if (!skills)
        {
            EvenMoreBalancedMorticianPlugin.Log.LogError("Mortician skill locator not found; cooldowns were not applied.");
            return;
        }

        SetCooldown(skills.secondary, settings.RaiseDeadCooldown);
        SetCooldown(skills.utility, settings.SacrificeCooldown);
        SetCooldown(skills.special, settings.TombstoneCooldown);

        if (RestlessGraveSkill.Definition)
            RestlessGraveSkill.Definition.baseRechargeInterval = settings.TombstoneCooldown.Value;

        foreach (var body in LiveBodies.Of(MorrisPlugin.MorrisBodyIndex))
            RefreshCooldowns(body.skillLocator);
    }

    private static void SetCooldown(GenericSkill skillSlot, PresetSetting<float> cooldown)
    {
        foreach (var variant in skillSlot.skillFamily.variants)
            variant.skillDef.baseRechargeInterval = cooldown.Value;
    }

    private static void RefreshCooldowns(SkillLocator skills)
    {
        skills.secondary.RecalculateValues();
        skills.utility.RecalculateValues();
        skills.special.RecalculateValues();
    }

    private static void ApplySpitProcCoefficient(MorticianSettings settings)
    {
        var spitProjectile = BileSpit.spitPrefab ? BileSpit.spitPrefab.GetComponent<ProjectileController>() : null;
        if (!spitProjectile)
        {
            EvenMoreBalancedMorticianPlugin.Log.LogError("Ghoul spit projectile not found; its proc coefficient was not applied.");
            return;
        }

        spitProjectile.procCoefficient = settings.GhoulSpitProcCoefficient.Value;
    }

    private static void ApplyMorticianStats(CharacterBody body, MorticianSettings settings)
    {
        body.baseMaxHealth = settings.BaseHealth.Value;
        body.levelMaxHealth = settings.HealthPerLevel.Value;
        body.baseRegen = settings.BaseRegen.Value;
        body.levelRegen = settings.RegenPerLevel.Value;
        body.baseArmor = settings.BaseArmor.Value;
        body.levelArmor = settings.ArmorPerLevel.Value;
        body.baseDamage = settings.BaseDamage.Value;
        body.levelDamage = settings.DamagePerLevel.Value;
    }

    private static void ApplyGhoulStats(CharacterBody body, MorticianSettings settings)
    {
        ApplyDamageStats(body, settings.GhoulBaseDamage, settings.GhoulDamagePerLevel);
        body.baseMaxHealth = settings.GhoulBaseHealth.Value;
        body.levelMaxHealth = settings.GhoulHealthPerLevel.Value;
        body.baseRegen = -settings.GhoulDegen.Value;
        body.levelRegen = -settings.GhoulDegenPerLevel.Value;
    }

    private static void ApplyDamageStats(CharacterBody body, PresetSetting<float> baseDamage, PresetSetting<float> damagePerLevel)
    {
        body.baseDamage = baseDamage.Value;
        body.levelDamage = damagePerLevel.Value;
    }

    private static void ApplyToBodies(string bodyName, GameObject bodyPrefab, BodyIndex bodyIndex, Action<CharacterBody> applyStats)
    {
        var prefabBody = bodyPrefab ? bodyPrefab.GetComponent<CharacterBody>() : null;
        if (!prefabBody)
        {
            EvenMoreBalancedMorticianPlugin.Log.LogError($"{bodyName} body prefab not found; its stats were not applied.");
            return;
        }

        applyStats(prefabBody);

        foreach (var liveBody in LiveBodies.Of(bodyIndex))
        {
            applyStats(liveBody);
            liveBody.MarkAllStatsDirty();
        }
    }

    private static float AsCoefficient(PresetSetting<float> percent) => percent.Value / 100f;
}
