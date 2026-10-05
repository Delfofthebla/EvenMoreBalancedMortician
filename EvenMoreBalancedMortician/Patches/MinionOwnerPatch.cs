using System;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using Morris.Components;
using RoR2;

namespace EvenMoreBalancedMortician.Patches;

// Mortician's minions remember their owner's body from when they were created, so they lose him once he dies, even if he is revived.
internal static class MinionOwnerPatch
{
    private static ILHook _tombstoneSpawnHook;

    public static void Install()
    {
        var tombstoneSpawn = typeof(TombstoneController).GetMethod(nameof(TombstoneController.SpawnGhoulAtClosestNode));
        _tombstoneSpawnHook = new ILHook(tombstoneSpawn, AdoptGhoulsRaisedWithoutOwner);

        CharacterBody.onBodyStartGlobal += ReclaimMinions;
    }

    private static void AdoptGhoulsRaisedWithoutOwner(ILContext il)
    {
        var cursor = new ILCursor(il);
        var foundSummon = cursor.TryGotoNext(MoveType.After,
            instruction => instruction.MatchCallvirt<MasterSummon>(nameof(MasterSummon.Perform)));

        if (!foundSummon)
        {
            EvenMoreBalancedMorticianPlugin.Log.LogError("Could not find where a tombstone raises its ghouls; ghouls it raises after Mortician dies will have no owner.");
            return;
        }

        cursor.Emit(OpCodes.Ldarg_0);
        cursor.EmitDelegate<Func<CharacterMaster, TombstoneController, CharacterMaster>>(AdoptGhoul);
    }

    private static CharacterMaster AdoptGhoul(CharacterMaster ghoul, TombstoneController tombstone)
    {
        if (!ghoul || ghoul.minionOwnership.ownerMaster)
            return ghoul;

        var owner = MinionOwner.MasterOf(tombstone.characterBody);
        if (owner)
            ghoul.minionOwnership.SetOwner(owner);

        return ghoul;
    }

    private static void ReclaimMinions(CharacterBody body)
    {
        var ownerLocator = body.GetComponent<TombstoneLocator>();
        if (!ownerLocator || !body.master)
            return;

        foreach (var minion in UnityEngine.Object.FindObjectsOfType<MorrisMinionController>())
        {
            if (MinionOwner.MasterOf(minion.characterBody) != body.master)
                continue;

            minion.owner = body.gameObject;
            minion.ownerBody = body;
        }

        TombstoneSoulPatch.HandOverTombstones(body.master, ownerLocator);
    }
}
