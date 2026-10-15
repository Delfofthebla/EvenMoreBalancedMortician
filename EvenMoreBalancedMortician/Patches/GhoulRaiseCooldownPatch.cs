using System;
using EvenMoreBalancedMortician.Presets;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using Morris.Components;
using RoR2;

namespace EvenMoreBalancedMortician.Patches;

internal static class GhoulRaiseCooldownPatch
{
    private static Hook _tombstoneStartHook;
    private static ILHook _tombstoneUpdateHook;
    private static PresetSetting<bool> _usesCooldownReduction;

    public static void Install(PresetSetting<bool> usesCooldownReduction)
    {
        _usesCooldownReduction = usesCooldownReduction;

        if (!TombstoneRaiseSkills.Register())
            return;

        _tombstoneStartHook = new Hook(MorticianMethods.TombstoneStart, FollowTombstoneStats);
        _tombstoneUpdateHook = new ILHook(MorticianMethods.TombstoneFixedUpdate, SpawnGhoulsFromSkillStock);
    }

    private static void FollowTombstoneStats(Action<TombstoneController> orig, TombstoneController self)
    {
        orig(self);

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

    private static void SpawnGhoulsFromSkillStock(ILContext il)
    {
        var cursor = new ILCursor(il);
        ILLabel skipSpawn = null;
        var foundSpawnTimer = cursor.TryGotoNext(
            instruction => instruction.MatchLdarg(0),
            instruction => instruction.MatchLdfld<TombstoneController>(nameof(TombstoneController.spawnStopwatch)),
            instruction => instruction.MatchLdsfld<TombstoneController>(nameof(TombstoneController.spawnTime)),
            instruction => instruction.MatchBltUn(out skipSpawn));

        if (!foundSpawnTimer)
        {
            EvenMoreBalancedMorticianPlugin.Log.LogError("Could not find the tombstone's ghoul spawn timer; cooldown reduction will not speed up its ghoul spawns.");
            return;
        }

        cursor.Index++;
        cursor.RemoveRange(3);
        cursor.EmitDelegate(TryConsumeGhoulSpawn);
        cursor.Emit(OpCodes.Brfalse, skipSpawn);
    }

    private static bool TryConsumeGhoulSpawn(TombstoneController tombstone) => TombstoneRaiseSkills.TryConsume(TombstoneRaiseSkills.GhoulSpawnSlotOf(tombstone.characterBody));
}
