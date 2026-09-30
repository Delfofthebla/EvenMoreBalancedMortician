using System;
using System.Collections.Generic;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using Morris.Components;
using RoR2;
using RoR2.Orbs;
using SkillStates.Ghoul;
using SkillStates.SharedStates;

namespace EvenMoreBalancedMortician.Patches;

internal static class ProcCoefficientPatch
{
    private const string ProcCoefficientField = "procCoefficient";

    private static readonly List<Hook> _hooks = [];
    private static readonly List<ILHook> _ilHooks = [];
    private static MorticianSettings _settings;

    public static void Install(MorticianSettings settings)
    {
        _settings = settings;

        _hooks.Add(new Hook(typeof(GhoulMelee).GetMethod(nameof(GhoulMelee.OnEnter)), (Action<Action<GhoulMelee>, GhoulMelee>)SetBiteProcCoefficient));
        _hooks.Add(new Hook(typeof(BaseLaunchedState).GetMethod(nameof(BaseLaunchedState.OnEnter)), (Action<Action<BaseLaunchedState>, BaseLaunchedState>)SetLaunchProcCoefficient));

        _ilHooks.Add(new ILHook(typeof(ClingState).GetMethod(nameof(ClingState.Bite)), ReplaceClingBiteProcCoefficient));
        _ilHooks.Add(new ILHook(typeof(GhoulDeath).GetMethod(nameof(GhoulDeath.Explode)), ReplaceDetonationProcCoefficient));
        _ilHooks.Add(new ILHook(typeof(TombstoneController).GetMethod(nameof(TombstoneController.FireSoulOrb)), SetSoulOrbProcCoefficient));
    }

    private static void SetBiteProcCoefficient(Action<GhoulMelee> orig, GhoulMelee self)
    {
        orig(self);
        self.attack.procCoefficient = _settings.GhoulBiteProcCoefficient.Value;
    }

    private static void SetLaunchProcCoefficient(Action<BaseLaunchedState> orig, BaseLaunchedState self)
    {
        orig(self);
        self.attack.procCoefficient = _settings.LaunchProcCoefficient.Value;
    }

    private static void ReplaceClingBiteProcCoefficient(ILContext il) =>
        ReplaceProcCoefficientConstant<DamageInfo>(il, ClingBiteProcCoefficient);

    private static void ReplaceDetonationProcCoefficient(ILContext il) =>
        ReplaceProcCoefficientConstant<BlastAttack>(il, DetonationProcCoefficient);

    private static void ReplaceProcCoefficientConstant<TAttack>(ILContext il, Func<float> procCoefficient)
    {
        var cursor = new ILCursor(il);
        var foundConstant = cursor.TryGotoNext(
            instruction => instruction.MatchLdcR4(out _),
            instruction => instruction.MatchStfld<TAttack>(ProcCoefficientField));

        if (!foundConstant)
        {
            EvenMoreBalancedMorticianPlugin.Log.LogError($"Could not find the proc coefficient in {il.Method.Name}; it keeps Mortician's value.");
            return;
        }

        cursor.Remove();
        cursor.EmitDelegate(procCoefficient);
    }

    private static void SetSoulOrbProcCoefficient(ILContext il)
    {
        var cursor = new ILCursor(il);
        if (!cursor.TryGotoNext(instruction => instruction.MatchCallvirt<OrbManager>(nameof(OrbManager.AddOrb))))
        {
            EvenMoreBalancedMorticianPlugin.Log.LogError("Could not find where the tombstone fires its soul orb; it keeps Mortician's proc coefficient.");
            return;
        }

        cursor.EmitDelegate<Func<Orb, Orb>>(WithSoulOrbProcCoefficient);
    }

    private static Orb WithSoulOrbProcCoefficient(Orb soulOrb)
    {
        if (soulOrb is GenericDamageOrb damageOrb)
            damageOrb.procCoefficient = _settings.SoulOrbProcCoefficient.Value;

        return soulOrb;
    }

    private static float ClingBiteProcCoefficient() => _settings.GhoulClingBiteProcCoefficient.Value;

    private static float DetonationProcCoefficient() => _settings.SacrificeProcCoefficient.Value;
}
