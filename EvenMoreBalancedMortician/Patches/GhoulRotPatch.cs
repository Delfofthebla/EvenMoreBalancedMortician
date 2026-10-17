using EvenMoreBalancedMortician.Presets;
using Morris;
using R2API;
using RoR2;

namespace EvenMoreBalancedMortician.Patches;

internal static class GhoulRotPatch
{
    private static PresetSetting<float> _degenGrowthPercent;

    public static bool IsRotting => _degenGrowthPercent.Value > 0f;

    public static void Install(PresetSetting<float> degenGrowthPercent)
    {
        _degenGrowthPercent = degenGrowthPercent;

        var ghoulBody = MorrisPlugin.GhoulBodyPrefab;
        if (!ghoulBody)
        {
            EvenMoreBalancedMorticianPlugin.Log.LogError("Ghoul body prefab not found; the Ghoul Degen Growth setting will not work.");
            return;
        }

        ghoulBody.AddComponent<GhoulRotRefresher>();
        RecalculateStatsAPI.GetStatCoefficients += GrowDegenWithAge;
    }

    private static void GrowDegenWithAge(CharacterBody body, RecalculateStatsAPI.StatHookEventArgs stats)
    {
        if (body.bodyIndex != MorrisPlugin.GhoulBodyIndex || !body.hasStarted)
            return;

        var growth = _degenGrowthPercent.Value / 100f * body.localStartTime.timeSince;
        stats.baseRegenAdd += body.baseRegen * growth;
        stats.levelRegenAdd += body.levelRegen * growth;
    }
}
