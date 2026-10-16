using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EvenMoreBalancedMortician.Presets;
using MonoMod.RuntimeDetour;
using Morris.Components;
using UnityEngine;

namespace EvenMoreBalancedMortician.Patches;

internal static class TombstoneSoulPatch
{
    private static readonly List<TombstoneController> _liveTombstones = [];

    private static Hook _tombstoneStartHook;
    private static Hook _tombstoneDestroyHook;
    private static Hook _soulDeliveryHook;
    private static PresetSetting<SoulRecipient> _soulRecipient;

    public static void Install(PresetSetting<SoulRecipient> soulRecipient)
    {
        _soulRecipient = soulRecipient;

        _tombstoneStartHook = HookTombstone(MorticianMethods.TombstoneStart, TrackTombstone);
        _tombstoneDestroyHook = HookTombstone(typeof(TombstoneController).GetMethod(nameof(TombstoneController.OnDestroy)), HandOverActiveTombstone);
        _soulDeliveryHook = HookTombstone(typeof(TombstoneController).GetMethod(nameof(TombstoneController.AddSoulStockServer)), DeliverSoulBySetting);
    }

    private static Hook HookTombstone(MethodInfo method, Action<Action<TombstoneController>, TombstoneController> hook) => new(method, hook);

    private static void TrackTombstone(Action<TombstoneController> orig, TombstoneController self)
    {
        orig(self);
        _liveTombstones.Add(self);
    }

    // Mortician feeds souls only to the newest tombstone and clears it on destruction, which would starve an older tombstone that outlives it.
    private static void HandOverActiveTombstone(Action<TombstoneController> orig, TombstoneController self)
    {
        _liveTombstones.Remove(self);
        orig(self);

        var ownerLocator = self.ownerLocator;
        if (!ownerLocator || ownerLocator.activeTombstone)
            return;

        var newestRemaining = _liveTombstones.LastOrDefault(tombstone => tombstone.ownerLocator == ownerLocator);
        if (newestRemaining)
            ownerLocator.SetActiveTombstone(newestRemaining);
    }

    private static void DeliverSoulBySetting(Action<TombstoneController> orig, TombstoneController activeTombstone)
    {
        var ghoul = DyingGhoul.Current;
        if (!ghoul)
        {
            orig(activeTombstone);
            return;
        }

        foreach (var recipient in SoulRecipients(activeTombstone, ghoul.transform.position))
            orig(recipient);
    }

    private static IEnumerable<TombstoneController> SoulRecipients(TombstoneController activeTombstone, Vector3 ghoulPosition) => _soulRecipient.Value switch
    {
        SoulRecipient.Nearest => [NearestTombstone(activeTombstone, ghoulPosition)],
        SoulRecipient.Every => TombstonesSharingOwner(activeTombstone),
        _ => [activeTombstone],
    };

    private static TombstoneController NearestTombstone(TombstoneController activeTombstone, Vector3 position)
        => TombstonesSharingOwner(activeTombstone).OrderBy(tombstone => (tombstone.transform.position - position).sqrMagnitude).First();

    private static IEnumerable<TombstoneController> TombstonesSharingOwner(TombstoneController activeTombstone)
        => _liveTombstones.Where(tombstone => tombstone.ownerLocator == activeTombstone.ownerLocator);
}
