using RoR2;

namespace EvenMoreBalancedMortician;

internal static class MinionOwner
{
    public static CharacterMaster MasterOf(CharacterBody minion)
        => minion && minion.master ? minion.master.minionOwnership.ownerMaster : null;

    public static CharacterBody BodyOf(CharacterBody minion)
    {
        var ownerMaster = MasterOf(minion);
        return ownerMaster ? ownerMaster.GetBody() : null;
    }
}
