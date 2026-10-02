using EvenMoreBalancedMortician.Presets;
using R2API;
using RoR2;
using MorticianSurvivor = Morris.Modules.Survivors.Morris;

namespace EvenMoreBalancedMortician.Patches;

internal static class TombstoneLimitPatch
{
    private const int BaseTombstoneLimit = 1;

    private static PresetSetting<bool> _lysateCellAddsTombstone;

    public static void Install(PresetSetting<bool> lysateCellAddsTombstone)
    {
        _lysateCellAddsTombstone = lysateCellAddsTombstone;
        MorticianSurvivor.tombstoneSlot = DeployableAPI.RegisterDeployableSlot(GetTombstoneLimit);
    }

    private static int GetTombstoneLimit(CharacterMaster master, int deployableCountMultiplier)
        => _lysateCellAddsTombstone.Value && HasLysateCell(master) ? BaseTombstoneLimit + 1 : BaseTombstoneLimit;

    private static bool HasLysateCell(CharacterMaster master)
        => master.inventory && master.inventory.GetItemCountEffective(DLC1Content.Items.EquipmentMagazineVoid) > 0;
}
