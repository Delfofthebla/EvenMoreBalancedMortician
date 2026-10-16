using R2API.Networking.Interfaces;
using UnityEngine.Networking;

namespace EvenMoreBalancedMortician.Networking;

internal sealed class HostSettingsMessage(string[] hostValues) : INetMessage
{
    private string[] _hostValues = hostValues;

    public HostSettingsMessage() : this([])
    {
    }

    public void Serialize(NetworkWriter writer)
    {
        writer.Write(_hostValues.Length);

        foreach (var value in _hostValues)
            writer.Write(value);
    }

    public void Deserialize(NetworkReader reader)
    {
        _hostValues = new string[reader.ReadInt32()];

        for (var i = 0; i < _hostValues.Length; i++)
            _hostValues[i] = reader.ReadString();
    }

    public void OnReceived() => HostSettingsSync.ReceiveHostValues(_hostValues);
}
