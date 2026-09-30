using System.Collections.Generic;
using System.Reflection;
using EvenMoreBalancedMortician.Presets;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using Morris.Components;
using RoR2;
using SkillStates.Morris;

namespace EvenMoreBalancedMortician.Patches;

internal static class GhoulEquipmentPatch
{
    private const BindingFlags AnyInstanceMethod = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

    private static readonly List<ILHook> hooks = new();
    private static PresetSetting<bool> ghoulsInheritEquipment;

    public static void Install(PresetSetting<bool> inheritEquipment)
    {
        ghoulsInheritEquipment = inheritEquipment;

        PatchEquipmentCopy(typeof(SpawnGhoul).GetMethod(nameof(SpawnGhoul.AttemptSpawnGhoul), AnyInstanceMethod));
        PatchEquipmentCopy(typeof(TombstoneController).GetMethod(nameof(TombstoneController.SpawnGhoulAtClosestNode), AnyInstanceMethod));
    }

    private static void PatchEquipmentCopy(MethodInfo ghoulSpawner)
    {
        hooks.Add(new ILHook(ghoulSpawner, MakeEquipmentCopyConditional));
    }

    private static void MakeEquipmentCopyConditional(ILContext il)
    {
        var cursor = new ILCursor(il);
        if (!cursor.TryGotoNext(instruction => instruction.MatchCallvirt<Inventory>(nameof(Inventory.CopyEquipmentFrom))))
        {
            EvenMoreBalancedMorticianPlugin.Log.LogError($"Could not find the equipment copy in {il.Method.Name}; ghouls will always inherit equipment.");
            return;
        }

        cursor.Remove();
        cursor.EmitDelegate<System.Action<Inventory, Inventory>>(CopyEquipmentIfEnabled);
    }

    private static void CopyEquipmentIfEnabled(Inventory ghoulInventory, Inventory ownerInventory)
    {
        if (!ghoulsInheritEquipment.Value)
            return;

        ghoulInventory.CopyEquipmentFrom(ownerInventory, includeChargeData: false);
    }
}
