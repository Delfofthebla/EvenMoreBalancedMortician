using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace EvenMoreBalancedMortician.Patches;

internal sealed class GhoulRotRefresher : MonoBehaviour
{
    private static readonly float RefreshInterval = 0.5f;

    private CharacterBody _body;
    private float _untilRefresh;

    private void Awake() => _body = GetComponent<CharacterBody>();

    private void FixedUpdate()
    {
        if (!NetworkServer.active || !GhoulRotPatch.IsRotting)
            return;

        _untilRefresh -= Time.fixedDeltaTime;
        if (_untilRefresh > 0f)
            return;

        _untilRefresh = RefreshInterval;
        _body.MarkAllStatsDirty();
    }
}
