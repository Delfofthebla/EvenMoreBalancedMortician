using System;
using EvenMoreBalancedMortician.Presets;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using Morris.Components;
using RoR2;

namespace EvenMoreBalancedMortician.Patches;

internal static class GhoulOnKillPatch
{
    private static ILHook _ghoulDeathHook;
    private static PresetSetting<float> _triggerChancePercent;

    public static void Install(PresetSetting<float> triggerChancePercent)
    {
        _triggerChancePercent = triggerChancePercent;

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
        cursor.EmitDelegate<Action<GlobalEventManager, DamageReport>>(TriggerOnKillByChance);
    }

    private static void TriggerOnKillByChance(GlobalEventManager eventManager, DamageReport ghoulDeath)
    {
        if (Util.CheckRoll(_triggerChancePercent.Value, ghoulDeath.attackerMaster))
            eventManager.OnCharacterDeath(ghoulDeath);
    }
}
