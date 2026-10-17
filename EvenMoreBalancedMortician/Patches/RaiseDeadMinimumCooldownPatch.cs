using EvenMoreBalancedMortician.Presets;
using Morris;
using RoR2;

namespace EvenMoreBalancedMortician.Patches;

internal static class RaiseDeadMinimumCooldownPatch
{
    private static PresetSetting<float> _minimumCooldown;

    public static void Install(PresetSetting<float> minimumCooldown)
    {
        _minimumCooldown = minimumCooldown;
        CharacterBody.onBodyStartGlobal += WatchMorticianStats;
    }

    private static void WatchMorticianStats(CharacterBody body)
    {
        if (body.bodyIndex != MorrisPlugin.MorrisBodyIndex)
            return;

        body.onRecalculateStats += ApplyMinimumCooldown;
        body.MarkAllStatsDirty();
    }

    private static void ApplyMinimumCooldown(CharacterBody mortician)
    {
        var raiseDead = mortician.skillLocator.secondary;
        if (!raiseDead || raiseDead.baseRechargeInterval <= 0f)
            return;

        var reducedCooldown = raiseDead.baseRechargeInterval * raiseDead.cooldownScale - raiseDead.flatCooldownReduction;
        if (reducedCooldown >= _minimumCooldown.Value)
            return;

        raiseDead.flatCooldownReduction = 0f;
        raiseDead.cooldownScale = _minimumCooldown.Value / raiseDead.baseRechargeInterval;
    }
}
