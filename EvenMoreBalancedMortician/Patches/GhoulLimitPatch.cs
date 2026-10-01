using System;
using System.Reflection;
using EvenMoreBalancedMortician.Presets;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using Morris.Components;
using Morris.Modules.NPC;
using R2API;
using RoR2;
using MorticianConfig = Morris.Modules.Config;
using MorticianSurvivor = Morris.Modules.Survivors.Morris;

namespace EvenMoreBalancedMortician.Patches;

internal static class GhoulLimitPatch
{
    private const int AnyPositiveLimit = 1;

    private static ILHook _minionStartHook;
    private static PresetSetting<int> _ghoulLimit;

    public static void Install(PresetSetting<int> limit)
    {
        _ghoulLimit = limit;
        MorticianSurvivor.ghoulSlot = DeployableAPI.RegisterDeployableSlot(GetGhoulLimit);
        EnsureGhoulsAreDeployable();

        var minionStart = typeof(MorrisMinionController).GetMethod("Start", BindingFlags.Instance | BindingFlags.NonPublic);
        _minionStartHook = new ILHook(minionStart, AlwaysTrackGhoulsAsDeployables);
    }

    // Mortician only gives the ghoul master a Deployable when its own config limit was positive at load, and the forced tracking below needs one.
    private static void EnsureGhoulsAreDeployable()
    {
        var ghoulMaster = GhoulMinion.ghoulMasterPrefab;
        if (!ghoulMaster)
        {
            EvenMoreBalancedMorticianPlugin.Log.LogError("Ghoul master prefab not found; the Ghoul Limit setting will not work.");
            return;
        }

        if (!ghoulMaster.GetComponent<Deployable>())
            ghoulMaster.AddComponent<Deployable>();
    }

    private static int GetGhoulLimit(CharacterMaster master, int deployableCountMultiplier)
        => _ghoulLimit.Value > 0 ? _ghoulLimit.Value : int.MaxValue;

    // Mortician only tracks ghouls in its deployable slot when its own config limit is positive, so that check is forced true and our slot's limit decides instead.
    private static void AlwaysTrackGhoulsAsDeployables(ILContext il)
    {
        var cursor = new ILCursor(il);
        var foundLimitCheck = cursor.TryGotoNext(MoveType.After,
            instruction => instruction.MatchLdsfld(typeof(MorticianConfig), nameof(MorticianConfig.ghoulLimit)),
            instruction => instruction.MatchCallvirt(out var method) && method.Name == "get_Value");

        if (!foundLimitCheck)
        {
            EvenMoreBalancedMorticianPlugin.Log.LogError("Could not find Mortician's ghoul limit check; the Ghoul Limit setting will not work.");
            return;
        }

        cursor.EmitDelegate<Func<int, int>>(_ => AnyPositiveLimit);
    }
}
