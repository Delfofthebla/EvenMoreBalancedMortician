using System;
using System.Collections.Generic;
using System.Linq;
using EvenMoreBalancedMortician.Presets;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using Morris.Components;
using UnityEngine;

namespace EvenMoreBalancedMortician.Patches;

internal static class TombstoneSoulPatch
{
    private static readonly List<TombstoneController> _liveTombstones = [];

    private static Hook _tombstoneStartHook;
    private static Hook _tombstoneDestroyHook;
    private static ILHook _ghoulDeathHook;
    private static PresetSetting<SoulRecipient> _soulRecipient;

    public static void Install(PresetSetting<SoulRecipient> soulRecipient)
    {
        _soulRecipient = soulRecipient;

        _tombstoneStartHook = HookTombstone(nameof(TombstoneController.Start), TrackTombstone);
        _tombstoneDestroyHook = HookTombstone(nameof(TombstoneController.OnDestroy), HandOverActiveTombstone);

        var ghoulDeath = typeof(MorrisMinionController).GetMethod(nameof(MorrisMinionController.OnDeathStart));
        _ghoulDeathHook = new ILHook(ghoulDeath, DeliverSoulsBySetting);
    }

    private static Hook HookTombstone(string methodName, Action<Action<TombstoneController>, TombstoneController> hook)
        => new(typeof(TombstoneController).GetMethod(methodName), hook);

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

    private static void DeliverSoulsBySetting(ILContext il)
    {
        var cursor = new ILCursor(il);
        var foundSoulDelivery = cursor.TryGotoNext(
            instruction => instruction.MatchCallvirt<TombstoneController>(nameof(TombstoneController.AddSoulStockServer)));

        if (!foundSoulDelivery)
        {
            EvenMoreBalancedMorticianPlugin.Log.LogError("Could not find where a slain ghoul gives its soul to a tombstone; the Soul Recipient setting will not work.");
            return;
        }

        cursor.Remove();
        cursor.Emit(Mono.Cecil.Cil.OpCodes.Ldarg_0);
        cursor.EmitDelegate<Action<TombstoneController, MorrisMinionController>>(DeliverSoul);
    }

    private static void DeliverSoul(TombstoneController activeTombstone, MorrisMinionController ghoul)
    {
        switch (_soulRecipient.Value)
        {
            case SoulRecipient.Nearest:
                NearestTombstone(activeTombstone, ghoul.transform.position).AddSoulStockServer();
                break;

            case SoulRecipient.Every:
                foreach (var tombstone in TombstonesSharingOwner(activeTombstone))
                    tombstone.AddSoulStockServer();
                break;

            default:
                activeTombstone.AddSoulStockServer();
                break;
        }
    }

    private static TombstoneController NearestTombstone(TombstoneController activeTombstone, Vector3 position)
        => TombstonesSharingOwner(activeTombstone)
            .OrderBy(tombstone => (tombstone.transform.position - position).sqrMagnitude)
            .First();

    private static IEnumerable<TombstoneController> TombstonesSharingOwner(TombstoneController activeTombstone)
        => _liveTombstones.Where(tombstone => tombstone.ownerLocator == activeTombstone.ownerLocator);
}
