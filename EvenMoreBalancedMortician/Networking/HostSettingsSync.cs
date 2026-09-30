using System;
using R2API.Networking;
using R2API.Networking.Interfaces;
using RoR2;
using RoR2.Networking;
using UnityEngine.Networking;

namespace EvenMoreBalancedMortician.Networking;

internal static class HostSettingsSync
{
    private static MorticianSettings _settings;
    private static Action _applySettings;

    public static void Install(MorticianSettings settings, Action applySettings)
    {
        _settings = settings;
        _applySettings = applySettings;

        NetworkingAPI.RegisterMessageType<HostSettingsMessage>();

        NetworkUser.onPostNetworkUserStart += _ => SendToClients();
        Run.onRunStartGlobal += _ => SendToClients();
        settings.PresetSelector.SettingsChanged += SendToClients;
        NetworkManagerSystem.onStopClientGlobal += RevertToLocalValues;
    }

    public static void ReceiveHostValues(string[] hostValues)
    {
        if (NetworkServer.active)
            return;

        if (!_settings.TryUseHostValues(hostValues))
        {
            EvenMoreBalancedMorticianPlugin.Log.LogError("The host's settings don't match this version of the mod; using local settings instead.");
            return;
        }

        _applySettings();
    }

    private static void SendToClients()
    {
        if (!NetworkServer.active)
            return;

        new HostSettingsMessage(_settings.SerializeLocalValues()).Send(NetworkDestination.Clients);
    }

    private static void RevertToLocalValues()
    {
        _settings.UseLocalValues();
        _applySettings();
    }
}
