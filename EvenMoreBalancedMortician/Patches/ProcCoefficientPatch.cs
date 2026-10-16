using System;
using System.Collections.Generic;
using MonoMod.RuntimeDetour;
using Morris.Content;
using RoR2;
using SkillStates.Ghoul;
using SkillStates.SharedStates;

namespace EvenMoreBalancedMortician.Patches;

internal static class ProcCoefficientPatch
{
    private static readonly ScopedValue<ClingState> _bitingGhoul = new();
    private static readonly ScopedValue<GhoulDeath> _detonatingGhoul = new();
    private static readonly List<Hook> _hooks = [];

    private static MorticianSettings _settings;

    public static void Install(MorticianSettings settings)
    {
        _settings = settings;

        _hooks.Add(new Hook(typeof(GhoulMelee).GetMethod(nameof(GhoulMelee.OnEnter)), SetBiteProcCoefficient));
        _hooks.Add(new Hook(typeof(BaseLaunchedState).GetMethod(nameof(BaseLaunchedState.OnEnter)), SetLaunchProcCoefficient));
        _hooks.Add(new Hook(typeof(TombstoneSoulOrb).GetMethod(nameof(TombstoneSoulOrb.Begin)), SetSoulOrbProcCoefficient));

        _bitingGhoul.SetDuring(typeof(ClingState).GetMethod(nameof(ClingState.Bite)));
        _hooks.Add(new Hook(typeof(HealthComponent).GetMethod(nameof(HealthComponent.TakeDamage)), SetClingBiteProcCoefficient));

        _detonatingGhoul.SetDuring(typeof(GhoulDeath).GetMethod(nameof(GhoulDeath.Explode)));
        _hooks.Add(new Hook(typeof(BlastAttack).GetMethod(nameof(BlastAttack.Fire)), SetDetonationProcCoefficient));
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

    private static void SetSoulOrbProcCoefficient(Action<TombstoneSoulOrb> orig, TombstoneSoulOrb self)
    {
        self.procCoefficient = _settings.SoulOrbProcCoefficient.Value;
        orig(self);
    }

    private static void SetClingBiteProcCoefficient(Action<HealthComponent, DamageInfo> orig, HealthComponent self, DamageInfo damageInfo)
    {
        var bitingGhoul = _bitingGhoul.Current;
        if (bitingGhoul != null && damageInfo.inflictor == bitingGhoul.gameObject)
            damageInfo.procCoefficient = _settings.GhoulClingBiteProcCoefficient.Value;

        orig(self, damageInfo);
    }

    private static BlastAttack.Result SetDetonationProcCoefficient(Func<BlastAttack, BlastAttack.Result> orig, BlastAttack self)
    {
        var detonatingGhoul = _detonatingGhoul.Current;
        if (detonatingGhoul != null && self == detonatingGhoul.attack)
            self.procCoefficient = _settings.SacrificeProcCoefficient.Value;

        return orig(self);
    }
}
