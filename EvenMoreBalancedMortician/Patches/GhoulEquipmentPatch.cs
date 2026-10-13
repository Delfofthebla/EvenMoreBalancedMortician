using System.Collections.Generic;
using System.Reflection;
using Mono.Cecil;
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

    private static readonly List<ILHook> _hooks = [];

    public static void Install()
    {
        RemoveEquipmentCopy(typeof(SpawnGhoul).GetMethod(nameof(SpawnGhoul.AttemptSpawnGhoul), AnyInstanceMethod));
        RemoveEquipmentCopy(typeof(TombstoneController).GetMethod(nameof(TombstoneController.SpawnGhoulAtClosestNode), AnyInstanceMethod));
    }

    private static void RemoveEquipmentCopy(MethodInfo ghoulSpawner)
    {
        _hooks.Add(new ILHook(ghoulSpawner, RemoveEquipmentCopyCall));
    }

    private static void RemoveEquipmentCopyCall(ILContext il)
    {
        var cursor = new ILCursor(il);
        MethodReference equipmentCopy = null;
        if (!cursor.TryGotoNext(instruction => instruction.MatchCallvirt(out equipmentCopy) && IsEquipmentCopy(equipmentCopy)))
        {
            EvenMoreBalancedMorticianPlugin.Log.LogError($"Could not find the equipment copy in {il.Method.Name}; ghouls will still copy Mortician's equipment.");
            return;
        }

        cursor.Remove();

        var instanceAndArguments = equipmentCopy.Parameters.Count + 1;
        for (var i = 0; i < instanceAndArguments; i++)
            cursor.Emit(OpCodes.Pop);
    }

    private static bool IsEquipmentCopy(MethodReference method)
        => method.Name == nameof(Inventory.CopyEquipmentFrom) && method.DeclaringType.FullName == typeof(Inventory).FullName;
}
