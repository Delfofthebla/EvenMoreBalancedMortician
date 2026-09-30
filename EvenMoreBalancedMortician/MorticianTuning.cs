using System;
using EvenMoreBalancedMortician.Presets;
using Morris;
using Morris.Components;
using RoR2;
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

        SwingShovel.damageCoefficient = AsCoefficient(settings.ShovelDamagePercent);
        BaseLaunchedState.damageCoefficient = AsCoefficient(settings.LaunchDamagePercent);

        GhoulMelee.damageCoefficient = AsCoefficient(settings.GhoulBiteDamagePercent);
        ClingState.damageCoefficient = AsCoefficient(settings.GhoulBiteDamagePercent);
        BileSpit.damageCoefficient = AsCoefficient(settings.GhoulSpitDamagePercent);
        ApplyToBodies(
            "Ghoul",
            MorrisPlugin.GhoulBodyPrefab,
            MorrisPlugin.GhoulBodyIndex,
            body => ApplyDamageStats(body, settings.GhoulBaseDamage, settings.GhoulDamagePerLevel)
        );

        GhoulDeath.sacrificedDamageCoefficient = AsCoefficient(settings.SacrificeDamagePercent);
        GhoulDeath.sacrificedRadius = settings.SacrificeRadius.Value;
        Sacrifice.sacrificePercentHealAmount = AsCoefficient(settings.SacrificeHealPercent);

        ApplyToBodies("Tombstone",
            MorrisPlugin.TombstoneBodyPrefab,
            MorrisPlugin.TombstoneBodyIndex,
            body => ApplyDamageStats(body, settings.TombstoneBaseDamage, settings.TombstoneDamagePerLevel)
        );
        TombstoneController.soulOrbDamage = AsCoefficient(settings.SoulOrbDamagePercent);
        TombstoneController.spawnTime = settings.TombstoneGhoulSpawnInterval.Value;
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

        if (bodyIndex == BodyIndex.None)
            return;

        foreach (var liveBody in CharacterBody.readOnlyInstancesList)
        {
            if (liveBody.bodyIndex != bodyIndex)
                continue;

            applyStats(liveBody);
            liveBody.MarkAllStatsDirty();
        }
    }

    private static float AsCoefficient(PresetSetting<float> percent) => percent.Value / 100f;
}
