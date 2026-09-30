using System;
using EvenMoreBalancedMortician.Presets;
using MonoMod.RuntimeDetour;
using Morris.Components;
using RoR2;
using UnityEngine.Networking;

namespace EvenMoreBalancedMortician.Patches;

internal static class TombstoneLifetimePatch
{
    private static Hook _tombstoneStartHook;
    private static PresetSetting<float> _tombstoneLifetime;

    public static void Install(PresetSetting<float> lifetime)
    {
        _tombstoneLifetime = lifetime;

        var tombstoneStart = typeof(TombstoneController).GetMethod(nameof(TombstoneController.Start));
        _tombstoneStartHook = new Hook(tombstoneStart, (Action<Action<TombstoneController>, TombstoneController>)StartExpiryTimer);
    }

    private static void StartExpiryTimer(Action<TombstoneController> orig, TombstoneController self)
    {
        orig(self);

        if (!NetworkServer.active || _tombstoneLifetime.Value <= 0f)
            return;

        var master = self.GetComponent<CharacterBody>().master;
        if (!master)
            return;

        master.gameObject.AddComponent<MasterSuicideOnTimer>().lifeTimer = _tombstoneLifetime.Value;
    }
}
