using System;
using EvenMoreBalancedMortician.Presets;
using MonoMod.RuntimeDetour;
using Morris.Components;
using Morris.Modules.NPC;
using R2API;
using RoR2;
using UnityEngine.Events;
using UnityEngine.Networking;
using MorticianConfig = Morris.Modules.Config;
using MorticianSurvivor = Morris.Modules.Survivors.Morris;

namespace EvenMoreBalancedMortician.Patches;

internal static class GhoulLimitPatch
{
    private static Hook _minionStartHook;
    private static PresetSetting<int> _ghoulLimit;

    public static void Install(PresetSetting<int> limit)
    {
        _ghoulLimit = limit;
        MorticianSurvivor.ghoulSlot = DeployableAPI.RegisterDeployableSlot(GetGhoulLimit);
        EnsureGhoulsAreDeployable();

        _minionStartHook = new Hook(MorticianMethods.MinionStart, TrackGhoulAsDeployable);
    }

    // The base mod only gives the ghoul master a Deployable when its own config limit was positive at load, and the tracking below needs one.
    private static void EnsureGhoulsAreDeployable()
    {
        var ghoulMaster = GhoulMinion.ghoulMasterPrefab;
        if (!ghoulMaster)
        {
            EvenMoreBalancedMorticianPlugin.Log.LogError("Ghoul master prefab not found; the Ghoul Limit setting will not work.");
            return;
        }

        if (!ghoulMaster.GetComponent<Deployable>())
            ghoulMaster.AddComponent<Deployable>();
    }

    private static int GetGhoulLimit(CharacterMaster master, int deployableCountMultiplier) => _ghoulLimit.Value > 0
        ? _ghoulLimit.Value
        : int.MaxValue;

    // The base mod only tracks ghouls in its deployable slot while its own config limit is positive. Otherwise this does the same tracking, so our slot's limit applies.
    private static void TrackGhoulAsDeployable(Action<MorrisMinionController> orig, MorrisMinionController self)
    {
        orig(self);

        if (MorticianTracksGhouls() || !IsOwnedGhoulOnServer(self))
            return;

        var ghoulMaster = self.characterBody.master;
        var deployable = ghoulMaster.GetComponent<Deployable>();
        deployable.onUndeploy = new UnityEvent();
        deployable.onUndeploy.AddListener(ghoulMaster.TrueKill);

        self.ownerMaster.AddDeployable(deployable, MorticianSurvivor.ghoulSlot);
    }

    private static bool MorticianTracksGhouls() => MorticianConfig.ghoulLimit.Value > 0;

    private static bool IsOwnedGhoulOnServer(MorrisMinionController minion) => NetworkServer.active && minion.minionType == MorrisMinionController.MorrisMinionType.Ghoul && minion.ownerMaster;
}
