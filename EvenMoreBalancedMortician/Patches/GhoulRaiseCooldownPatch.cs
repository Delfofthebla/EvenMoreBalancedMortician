using System;
using EvenMoreBalancedMortician.Presets;
using MonoMod.RuntimeDetour;
using Morris.Components;
using RoR2;
using UnityEngine.Networking;

namespace EvenMoreBalancedMortician.Patches;

internal static class GhoulRaiseCooldownPatch
{
    private static Hook _tombstoneStartHook;
    private static Hook _tombstoneUpdateHook;
    private static PresetSetting<bool> _usesCooldownReduction;

    public static void Install(PresetSetting<bool> usesCooldownReduction)
    {
        _usesCooldownReduction = usesCooldownReduction;

        if (!TombstoneRaiseSkills.Register())
            return;

        _tombstoneStartHook = new Hook(MorticianMethods.TombstoneStart, SwitchToSkillStockSpawning);
        _tombstoneUpdateHook = new Hook(MorticianMethods.TombstoneFixedUpdate, SpawnGhoulsFromSkillStock);
    }

    private static void SwitchToSkillStockSpawning(Action<TombstoneController> orig, TombstoneController self)
    {
        orig(self);
        DisableMorticianSpawnTimer(self);

        self.characterBody.onRecalculateStats += ApplyCooldownReductionSetting;
        self.characterBody.MarkAllStatsDirty();
    }

    private static void ApplyCooldownReductionSetting(CharacterBody tombstone)
    {
        var ghoulSpawn = TombstoneRaiseSkills.GhoulSpawnSlotOf(tombstone);
        var ghoulRaise = TombstoneRaiseSkills.GhoulRaiseSlotOf(tombstone);
        if (!ghoulSpawn || !ghoulRaise)
            return;

        if (!_usesCooldownReduction.Value)
        {
            ghoulSpawn.cooldownScale = 1f;
            ghoulSpawn.flatCooldownReduction = 0f;
        }

        ghoulRaise.cooldownScale = ghoulSpawn.cooldownScale;
        ghoulRaise.flatCooldownReduction = ghoulSpawn.flatCooldownReduction;
    }

    private static void SpawnGhoulsFromSkillStock(Action<TombstoneController> orig, TombstoneController self)
    {
        orig(self);

        if (NetworkServer.active && self.nodeGraph && TryConsumeGhoulSpawn(self))
            self.SpawnGhoulAtClosestNode();
    }

    // Mortician's FixedUpdate adds to spawnStopwatch and spawns a ghoul once it reaches spawnTime; from negative infinity it never does.
    private static void DisableMorticianSpawnTimer(TombstoneController tombstone) => tombstone.spawnStopwatch = float.NegativeInfinity;

    private static bool TryConsumeGhoulSpawn(TombstoneController tombstone) => TombstoneRaiseSkills.TryConsume(TombstoneRaiseSkills.GhoulSpawnSlotOf(tombstone.characterBody));
}
