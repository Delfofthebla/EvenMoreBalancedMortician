using RoR2;

namespace EvenMoreBalancedMortician.Scepter;

internal static class TombstoneOwner
{
    public static CharacterBody BodyOf(CharacterBody tombstone)
    {
        var ownerMaster = tombstone.master ? tombstone.master.minionOwnership.ownerMaster : null;
        return ownerMaster ? ownerMaster.GetBody() : null;
    }
}
