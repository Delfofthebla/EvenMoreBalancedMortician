using System;
using System.Linq;
using BepInEx.Configuration;
using EvenMoreBalancedMortician.Presets;
using MonoMod.RuntimeDetour;
using Morris.Components;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace EvenMoreBalancedMortician.Scepter;

internal static class RestlessGrave
{
    private static Hook _tombstoneStartHook;
    private static PresetSetting<float> _radius;
    private static PresetSetting<float> _cooldown;
    private static PresetSetting<int> _risenGhoulLimit;

    public static void Install(PresetSetting<float> radius, PresetSetting<float> cooldown, PresetSetting<int> risenGhoulLimit, ConfigEntry<bool> showRadius)
    {
        _radius = radius;
        _cooldown = cooldown;
        _risenGhoulLimit = risenGhoulLimit;
        RaiseRadiusIndicator.Configure(radius, showRadius);

        var tombstoneStart = typeof(TombstoneController).GetMethod(nameof(TombstoneController.Start));
        _tombstoneStartHook = new Hook(tombstoneStart, (Action<Action<TombstoneController>, TombstoneController>)AddRestlessGraveComponents);

        GlobalEventManager.onCharacterDeathGlobal += RaiseSlainEnemy;
    }

    private static void AddRestlessGraveComponents(Action<TombstoneController> orig, TombstoneController self)
    {
        orig(self);

        self.gameObject.AddComponent<RaiseRadiusIndicator>();

        if (NetworkServer.active)
            self.gameObject.AddComponent<GraveRaiser>();
    }

    private static void RaiseSlainEnemy(DamageReport report)
    {
        if (!NetworkServer.active || !report.victimMaster || !report.victimBody)
            return;

        var corpsePosition = report.victimBody.corePosition;
        var grave = NearestReadyGrave(report.victimTeamIndex, corpsePosition);
        if (grave)
            grave.Raise(corpsePosition, _cooldown.Value);
    }

    private static GraveRaiser NearestReadyGrave(TeamIndex victimTeam, Vector3 corpsePosition)
    {
        var radiusSquared = _radius.Value * _radius.Value;

        return InstanceTracker.GetInstancesList<GraveRaiser>()
            .Where(grave => DistanceSquared(grave, corpsePosition) <= radiusSquared)
            .OrderBy(grave => DistanceSquared(grave, corpsePosition))
            .FirstOrDefault(grave => grave.CanRaise(victimTeam, _risenGhoulLimit.Value));
    }

    private static float DistanceSquared(GraveRaiser grave, Vector3 position) => (grave.transform.position - position).sqrMagnitude;
}
