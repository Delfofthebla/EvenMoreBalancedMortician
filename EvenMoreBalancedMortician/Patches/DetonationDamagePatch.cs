using System;
using EvenMoreBalancedMortician.Presets;
using MonoMod.RuntimeDetour;
using RoR2;
using SkillStates.Ghoul;

namespace EvenMoreBalancedMortician.Patches;

internal static class DetonationDamagePatch
{
    private static Hook _explodeHook;
    private static PresetSetting<bool> _scalesWithMortician;

    public static void Install(PresetSetting<bool> scalesWithMortician)
    {
        _scalesWithMortician = scalesWithMortician;
        _explodeHook = new Hook(typeof(GhoulDeath).GetMethod(nameof(GhoulDeath.Explode)), ExplodeWithDetonationDamageStat);
    }

    private static void ExplodeWithDetonationDamageStat(Action<GhoulDeath> orig, GhoulDeath self)
    {
        if (!_scalesWithMortician.Value)
        {
            orig(self);
            return;
        }

        var ghoulDamageStat = self.damageStat;
        self.damageStat = DetonationDamageStat(self);

        orig(self);

        self.damageStat = ghoulDamageStat;
    }

    private static float DetonationDamageStat(GhoulDeath ghoulDeath)
    {
        var minion = ghoulDeath.minionController;
        var sacrificer = minion.sacrificeOwner ? minion.sacrificeOwner : minion.owner;
        var sacrificerBody = sacrificer
            ? sacrificer.GetComponent<CharacterBody>()
            : null;

        return sacrificerBody ? sacrificerBody.damage : ghoulDeath.damageStat;
    }
}
