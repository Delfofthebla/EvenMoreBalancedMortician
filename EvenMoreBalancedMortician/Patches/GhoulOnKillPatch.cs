using System;
using EvenMoreBalancedMortician.Presets;
using MonoMod.RuntimeDetour;
using Morris.Components;
using RoR2;

namespace EvenMoreBalancedMortician.Patches;

internal static class GhoulOnKillPatch
{
    private static Hook _onKillTriggerHook;
    private static PresetSetting<float> _triggerChancePercent;
    private static PresetSetting<bool> _sacrificeGuaranteesTrigger;

    public static void Install(PresetSetting<float> triggerChancePercent, PresetSetting<bool> sacrificeGuaranteesTrigger)
    {
        _triggerChancePercent = triggerChancePercent;
        _sacrificeGuaranteesTrigger = sacrificeGuaranteesTrigger;

        _onKillTriggerHook = new Hook(typeof(GlobalEventManager).GetMethod(nameof(GlobalEventManager.TriggerCharacterDeathProc)), RollForGhoulOnKillTrigger);
    }

    private static void RollForGhoulOnKillTrigger(Action<GlobalEventManager, DamageReport> orig, GlobalEventManager self, DamageReport death)
    {
        var ghoul = DyingGhoul.Current;
        if (IsOwnDeathProc(ghoul, death) && !PassesOnKillRoll(ghoul, death))
            return;

        orig(self, death);
    }

    private static bool IsOwnDeathProc(MorrisMinionController ghoul, DamageReport death) => ghoul && death.victim == ghoul.healthComponent;

    private static bool PassesOnKillRoll(MorrisMinionController ghoul, DamageReport death) => IsGuaranteed(ghoul) || Util.CheckRoll(_triggerChancePercent.Value, death.attackerMaster);

    private static bool IsGuaranteed(MorrisMinionController ghoul) => ghoul.sacrificed && _sacrificeGuaranteesTrigger.Value;
}
