using System.Collections.Generic;
using Morris.Components;
using Morris.Modules.NPC;
using RoR2;
using RoR2.Navigation;
using UnityEngine;

namespace EvenMoreBalancedMortician.Scepter;

internal sealed class GraveRaiser : MonoBehaviour
{
    private readonly List<CharacterBody> _risenGhouls = [];

    private TombstoneController _tombstone;
    private CharacterBody _tombstoneBody;
    private Run.FixedTimeStamp _readyAt = Run.FixedTimeStamp.negativeInfinity;

    private TeamIndex Team => _tombstoneBody.teamComponent.teamIndex;
    private CharacterBody Owner => MinionOwner.BodyOf(_tombstoneBody);

    private void Awake()
    {
        _tombstone = GetComponent<TombstoneController>();
        _tombstoneBody = GetComponent<CharacterBody>();
    }

    private void OnEnable() => InstanceTracker.Add(this);

    private void OnDisable() => InstanceTracker.Remove(this);

    public bool CanRaise(TeamIndex victimTeam, int risenGhoulLimit)
    {
        if (!_readyAt.hasPassed || victimTeam == Team || victimTeam == TeamIndex.Neutral)
            return false;

        if (!RestlessGraveSkill.IsEquippedBy(Owner))
            return false;

        _risenGhouls.RemoveAll(IsGone);
        return risenGhoulLimit <= 0 || _risenGhouls.Count < risenGhoulLimit;
    }

    public void Raise(Vector3 corpsePosition, float cooldown)
    {
        _readyAt = Run.FixedTimeStamp.now + cooldown;

        var summon = new MasterSummon
        {
            masterPrefab = GhoulMinion.ghoulMasterPrefab,
            ignoreTeamMemberLimit = true,
            teamIndexOverride = Team,
            summonerBodyObject = Owner.gameObject,
            position = GroundNear(corpsePosition),
            rotation = transform.rotation,
        };

        var ghoul = summon.Perform();
        var ghoulBody = ghoul ? ghoul.GetBody() : null;
        if (ghoulBody)
            _risenGhouls.Add(ghoulBody);
    }

    private Vector3 GroundNear(Vector3 position)
    {
        var nodeGraph = _tombstone.nodeGraph;
        if (!nodeGraph)
            return position;

        var node = nodeGraph.FindClosestNodeWithFlagConditions(position, HullClassification.Human, NodeFlags.None, NodeFlags.NoCharacterSpawn, false);
        return nodeGraph.GetNodePosition(node, out var groundPosition) ? groundPosition : position;
    }

    private static bool IsGone(CharacterBody ghoul) => !ghoul || !ghoul.healthComponent.alive;
}
