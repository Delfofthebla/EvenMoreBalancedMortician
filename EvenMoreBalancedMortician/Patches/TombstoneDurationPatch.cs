using System;
using EvenMoreBalancedMortician.Presets;
using MonoMod.RuntimeDetour;
using Morris.Components;
using RoR2;
using UnityEngine.Networking;

namespace EvenMoreBalancedMortician.Patches;

internal static class TombstoneDurationPatch
{
    private static Hook _tombstoneStartHook;
    private static PresetSetting<float> _tombstoneDuration;

    public static void Install(PresetSetting<float> duration)
    {
        _tombstoneDuration = duration;

        var tombstoneStart = typeof(TombstoneController).GetMethod(nameof(TombstoneController.Start));
        _tombstoneStartHook = new Hook(tombstoneStart, (Action<Action<TombstoneController>, TombstoneController>)StartExpiryTimer);
    }

    private static void StartExpiryTimer(Action<TombstoneController> orig, TombstoneController self)
    {
        orig(self);

        if (!NetworkServer.active || _tombstoneDuration.Value <= 0f)
            return;

        var master = self.GetComponent<CharacterBody>().master;
        if (!master)
            return;

        master.gameObject.AddComponent<MasterSuicideOnTimer>().lifeTimer = _tombstoneDuration.Value;
    }
}
