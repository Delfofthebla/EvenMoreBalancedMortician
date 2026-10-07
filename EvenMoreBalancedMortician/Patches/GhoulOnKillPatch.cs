using System;
using EvenMoreBalancedMortician.Presets;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using Morris.Components;
using RoR2;

namespace EvenMoreBalancedMortician.Patches;

internal static class GhoulOnKillPatch
{
    private static ILHook _ghoulDeathHook;
    private static PresetSetting<float> _triggerChancePercent;
    private static PresetSetting<bool> _sacrificeGuaranteesTrigger;

    public static void Install(PresetSetting<float> triggerChancePercent, PresetSetting<bool> sacrificeGuaranteesTrigger)
    {
        _triggerChancePercent = triggerChancePercent;
        _sacrificeGuaranteesTrigger = sacrificeGuaranteesTrigger;

        var ghoulDeath = typeof(MorrisMinionController).GetMethod(nameof(MorrisMinionController.OnDeathStart));
        _ghoulDeathHook = new ILHook(ghoulDeath, RollForOnKillTrigger);
    }

    private static void RollForOnKillTrigger(ILContext il)
    {
        var cursor = new ILCursor(il);
        var foundOnKillTrigger = cursor.TryGotoNext(
            instruction => instruction.MatchCallvirt<GlobalEventManager>(nameof(GlobalEventManager.OnCharacterDeath)));

        if (!foundOnKillTrigger)
        {
            EvenMoreBalancedMorticianPlugin.Log.LogError("Could not find where a dying ghoul triggers on-kill items; the On-Kill Trigger Chance setting will not work.");
            return;
        }

        cursor.Remove();
        cursor.Emit(OpCodes.Ldarg_0);
        cursor.EmitDelegate<Action<GlobalEventManager, DamageReport, MorrisMinionController>>(TriggerOnKill);
    }

    private static void TriggerOnKill(GlobalEventManager eventManager, DamageReport ghoulDeath, MorrisMinionController ghoul)
    {
        if (IsGuaranteed(ghoul) || Util.CheckRoll(_triggerChancePercent.Value, ghoulDeath.attackerMaster))
            eventManager.OnCharacterDeath(ghoulDeath);
    }

    private static bool IsGuaranteed(MorrisMinionController ghoul) => ghoul.sacrificed && _sacrificeGuaranteesTrigger.Value;
}
