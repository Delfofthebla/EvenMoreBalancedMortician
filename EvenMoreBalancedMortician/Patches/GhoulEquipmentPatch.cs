using System;
using System.Reflection;
using Morris;
using Morris.Components;
using MonoMod.RuntimeDetour;
using RoR2;
using SkillStates.Morris;

namespace EvenMoreBalancedMortician.Patches;

internal static class GhoulEquipmentPatch
{
    private static readonly ScopedValue<SpawnGhoul> _morticianSummoning = new();
    private static readonly ScopedValue<TombstoneController> _tombstoneSummoning = new();

    private static Hook _equipmentCopyHook;

    public static void Install()
    {
        _morticianSummoning.SetDuring(typeof(SpawnGhoul).GetMethod(nameof(SpawnGhoul.AttemptSpawnGhoul), BindingFlags.Instance | BindingFlags.NonPublic));
        _tombstoneSummoning.SetDuring(typeof(TombstoneController).GetMethod(nameof(TombstoneController.SpawnGhoulAtClosestNode)));

        var equipmentCopy = typeof(Inventory).GetMethod(nameof(Inventory.CopyEquipmentFrom), [typeof(Inventory), typeof(bool)]);
        _equipmentCopyHook = new Hook(equipmentCopy, SkipSummonerEquipmentCopy);
    }

    private static void SkipSummonerEquipmentCopy(Action<Inventory, Inventory, bool> orig, Inventory self, Inventory other, bool includeChargeData)
    {
        if (IsSummonerCopyIntoGhoul(self, other))
            return;

        orig(self, other, includeChargeData);
    }

    private static bool IsSummonerCopyIntoGhoul(Inventory target, Inventory source)
    {
        var summonerInventory = SummonerInventory();
        return summonerInventory && source == summonerInventory && BelongsToGhoul(target);
    }

    private static Inventory SummonerInventory()
    {
        var morticianSummon = _morticianSummoning.Current;
        if (morticianSummon != null)
            return morticianSummon.characterBody.inventory;

        var tombstone = _tombstoneSummoning.Current;
        return tombstone
            ? tombstone.characterBody.inventory
            : null;
    }

    private static bool BelongsToGhoul(Inventory inventory) => inventory.TryGetComponent<CharacterMaster>(out var master) && master.bodyPrefab == MorrisPlugin.GhoulBodyPrefab;
}
