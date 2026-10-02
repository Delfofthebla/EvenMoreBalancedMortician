using System.Collections.Generic;
using System.Globalization;
using EvenMoreBalancedMortician.Scepter;
using R2API;

namespace EvenMoreBalancedMortician;

internal static class SkillDescriptions
{
    private const string TokenPrefix = "BOG_MORRIS_BODY_";

    private static LanguageAPI.LanguageOverlay _overlay;

    public static void Apply(MorticianSettings settings)
    {
        _overlay?.Remove();
        _overlay = LanguageAPI.AddOverlay(BuildDescriptions(settings));
    }

    private static Dictionary<string, string> BuildDescriptions(MorticianSettings settings) => new()
    {
        [TokenPrefix + "PRIMARY_SHOVEL_DESCRIPTION"] =
            $"Swing your shovel for {Damage(settings.ShovelDamagePercent.Value)}. " +
            $"Hit ghouls and tombstones to <style=cIsUtility>launch</style> them for {Damage(settings.LaunchDamagePercent.Value)}.",

        [TokenPrefix + "SECONDARY_GHOUL_DESCRIPTION"] =
            "<style=cIsUtility>Soulbound</style>. Spawn a ghoul on the ground in front of you. " +
            $"Ghouls bite for {Damage(settings.GhoulBiteDamagePercent.Value)}, " +
            $"and spit <style=cIsDamage>Blighted</style> bile for {Damage(settings.GhoulSpitDamagePercent.Value)}.",

        [TokenPrefix + "UTILITY_LANTERN_DESCRIPTION"] =
            $"<style=cIsHealth>Detonate</style> the target ghoul for {Damage(settings.SacrificeDamagePercent.Value)}, " +
            $"and <style=cIsHealing>heal {Number(settings.SacrificeHealPercent.Value)}% of your maximum health</style>.",

        [TokenPrefix + "SPECIAL_TOMBSTONE_DESCRIPTION"] = TombstoneDescription(settings),

        [RestlessGraveSkill.NameToken] = "Restless Grave",
        [RestlessGraveSkill.DescriptionToken] = TombstoneDescription(settings) + RestlessGraveDescription(settings),
    };

    private static string TombstoneDescription(MorticianSettings settings) =>
        $"Erect a tombstone{DescribeDuration(settings.TombstoneDuration.Value)} that spawns a ghoul every <style=cIsUtility>{Number(settings.TombstoneGhoulSpawnInterval.Value)} seconds</style>. " +
        "Whenever a ghoul is slain, the tombstone generates an <style=cIsDamage>explosive</style> <style=cIsUtility>vengeful soul</style> " +
        $"which it will fire at a nearby enemy for {Damage(settings.SoulOrbDamagePercent.Value)}.";

    private static string RestlessGraveDescription(MorticianSettings settings) =>
        $"\n<color=#d299ff>SCEPTER: Enemies slain within {Number(settings.RestlessGraveRadius.Value)}m of a tombstone rise as ghouls, " +
        $"at most once every {Number(settings.RestlessGraveCooldown.Value)} seconds per tombstone." +
        DescribeRisenGhoulLimit(settings.RestlessGraveRisenGhoulLimit.Value) +
        "</color>";

    private static string DescribeRisenGhoulLimit(int limit) => limit > 0
        ? $" Each tombstone keeps up to {limit} risen ghouls."
        : "";

    private static string DescribeDuration(float seconds) => seconds > 0f
        ? $" lasting <style=cIsUtility>{Number(seconds)} seconds</style>"
        : "";

    private static string Damage(float percent) => $"<style=cIsDamage>{Number(percent)}% damage</style>";

    private static string Number(float value) => value.ToString("0.##", CultureInfo.InvariantCulture);
}
