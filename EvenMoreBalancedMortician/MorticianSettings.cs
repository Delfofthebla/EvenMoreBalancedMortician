using System;
using System.Collections.Generic;
using System.Globalization;
using BepInEx.Configuration;
using EvenMoreBalancedMortician.Presets;

namespace EvenMoreBalancedMortician;

internal sealed class MorticianSettings
{
    private readonly ConfigFile _config;
    private readonly List<IPresetSetting> _presetSettings = [];

    private const string GeneralSection = "General";
    private const string BodySection = "Body Stats";
    private const string PrimarySection = "Primary - Shovel Strike";
    private const string SecondarySection = "Secondary - Raise Dead";
    private const string UtilitySection = "Utility - Sacrifice";
    private const string SpecialSection = "Special - Tombstone";

    private const string GhoulDamageNote =
        "Scales with the ghoul's damage stat (" + SecondarySection + "), not Mortician's. Uses Mortician's crit chance and on-hit items.";
    private const string LaunchDamageNote =
        "Scales with the launched ghoul's or tombstone's damage stat, not Mortician's. Uses Mortician's crit chance and on-hit items.";
    private const string SoulOrbDamageNote =
        "Scales with the tombstone's damage stat (" + SpecialSection + "), not Mortician's. Uses the tombstone's crit chance and items, which it copies from Mortician when placed.";

    public PresetSelector PresetSelector { get; }

    public PresetSetting<float> BaseHealth { get; }
    public PresetSetting<float> HealthPerLevel { get; }
    public PresetSetting<float> BaseRegen { get; }
    public PresetSetting<float> RegenPerLevel { get; }
    public PresetSetting<float> BaseArmor { get; }
    public PresetSetting<float> ArmorPerLevel { get; }
    public PresetSetting<float> BaseDamage { get; }
    public PresetSetting<float> DamagePerLevel { get; }

    public PresetSetting<float> ShovelDamagePercent { get; }
    public PresetSetting<float> LaunchDamagePercent { get; }
    public PresetSetting<bool> ShovelCountsAsPrimarySkill { get; }

    public PresetSetting<int> GhoulLimit { get; }
    public PresetSetting<float> GhoulBaseDamage { get; }
    public PresetSetting<float> GhoulDamagePerLevel { get; }
    public PresetSetting<float> GhoulBiteDamagePercent { get; }
    public PresetSetting<float> GhoulSpitDamagePercent { get; }
    public PresetSetting<bool> GhoulsInheritEquipment { get; }

    public PresetSetting<float> SacrificeDamagePercent { get; }
    public PresetSetting<float> SacrificeRadius { get; }
    public PresetSetting<float> SacrificeHealPercent { get; }

    public PresetSetting<float> TombstoneBaseDamage { get; }
    public PresetSetting<float> TombstoneDamagePerLevel { get; }
    public PresetSetting<float> SoulOrbDamagePercent { get; }
    public PresetSetting<float> TombstoneGhoulSpawnInterval { get; }
    public PresetSetting<float> TombstoneLifetime { get; }

    public MorticianSettings(ConfigFile config)
    {
        _config = config;

        var preset = config.Bind(GeneralSection, "Preset", MorticianPreset.EvenMoreBalanced,
            "Which preset the other settings follow. Choosing a preset overwrites every other setting with that preset's values; editing any setting afterwards switches this to Custom.\n" +
            "EvenMoreBalanced: Delf's rebalance.\n" +
            "BalancedMortician: Equivalent to Bloonjitsu7's BalancedMortician mod. \n" +
            "Original: Mortician with no changes.");

        BaseHealth = Bind(BodySection, "Base Health",
            "Mortician's maximum health at level 1.",
            Presets(evenMoreBalanced: 170f, balancedMortician: 200f, original: 200f),
            vanilla: "Most survivors sit at 110. Loader and Acrid 160. MUL-T 200.");
        HealthPerLevel = Bind(BodySection, "Health Per Level",
            "Maximum health Mortician gains per level.",
            Presets(evenMoreBalanced: 51f, balancedMortician: 66f, original: 66f),
            vanilla: "Every survivor gains 30% of their base health, so 33 for a 110 health survivor.");
        BaseRegen = Bind(BodySection, "Base Regen",
            "Health Mortician regenerates per second at level 1.",
            Presets(evenMoreBalanced: 1f, balancedMortician: 2.5f, original: 2.5f),
            vanilla: "Most survivors 1. Loader and Acrid 2.5.");
        RegenPerLevel = Bind(BodySection, "Regen Per Level",
            "Regen per second Mortician gains per level.",
            Presets(evenMoreBalanced: 0.2f, balancedMortician: 0.5f, original: 0.5f),
            vanilla: "Most survivors 0.2. Loader and Acrid 0.5.");
        BaseArmor = Bind(BodySection, "Base Armor",
            "Mortician's armor at level 1.",
            Presets(evenMoreBalanced: 10f, balancedMortician: 20f, original: 20f),
            vanilla: "Most survivors 0. Mercenary, Loader, Acrid, REX, Seeker and Drifter 20. MUL-T 12.");
        ArmorPerLevel = Bind(BodySection, "Armor Per Level",
            "Armor Mortician gains per level.",
            Presets(evenMoreBalanced: 0f, balancedMortician: 0f, original: 0f),
            vanilla: "Every survivor 0.");
        BaseDamage = Bind(BodySection, "Base Damage",
            "Mortician's damage at level 1. Only the shovel swing scales with it; ghoul and tombstone attacks use their own damage stats.",
            Presets(evenMoreBalanced: 12f, balancedMortician: 12f, original: 12f),
            vanilla: "Most survivors 12. MUL-T 11. Engineer 14. Acrid 15.");
        DamagePerLevel = Bind(BodySection, "Damage Per Level",
            "Damage Mortician gains per level.",
            Presets(evenMoreBalanced: 2.4f, balancedMortician: 2.4f, original: 2.4f),
            vanilla: "Every survivor gains 20% of their base damage, so 2.4 for a 12 damage survivor.");

        ShovelDamagePercent = Bind(PrimarySection, "Swing Damage Percent",
            "Shovel swing damage, as a percent of Mortician's damage stat.",
            Presets(evenMoreBalanced: 280f, balancedMortician: 360f, original: 800f));
        LaunchDamagePercent = Bind(PrimarySection, "Launch Damage Percent",
            "Damage a ghoul or tombstone deals when flung through enemies by your shovel, as a percent of their own damage stats.",
            Presets(evenMoreBalanced: 350f, balancedMortician: 600f, original: 350f),
            note: LaunchDamageNote);
        ShovelCountsAsPrimarySkill = Bind(PrimarySection, "Counts As Primary Skill Damage",
            "Whether shovel swings count as primary skill damage. Items that trigger on primary skill hits (such as Luminous Shot) require.",
            Presets(evenMoreBalanced: true, balancedMortician: true, original: false));

        GhoulLimit = Bind(SecondarySection, "Ghoul Limit",
            "Maximum number of ghouls Mortician can have at once; raising another kills the oldest. 0 means no limit.",
            Presets(evenMoreBalanced: 0, balancedMortician: 0, original: 0),
            note: "Replaces the Ghoul limit setting in Mortician's own config.");
        GhoulBaseDamage = Bind(SecondarySection, "Ghoul Base Damage",
            "Ghoul damage at level 1. Sets the damage of ghoul bites, spit, launched ghouls and Sacrifice explosions.",
            Presets(evenMoreBalanced: 8f, balancedMortician: 8f, original: 12f));
        GhoulDamagePerLevel = Bind(SecondarySection, "Ghoul Damage Per Level",
            "Damage ghouls gain per level.",
            Presets(evenMoreBalanced: 1.6f, balancedMortician: 1.6f, original: 2.4f));
        GhoulBiteDamagePercent = Bind(SecondarySection, "Bite Damage Percent",
            "Ghoul bite damage as a percent of the ghoul's damage stat.",
            Presets(evenMoreBalanced: 150f, balancedMortician: 150f, original: 150f),
            note: GhoulDamageNote);
        GhoulSpitDamagePercent = Bind(SecondarySection, "Spit Damage Percent",
            "Ghoul bile spit damage as a percent of the ghoul's damage stat.",
            Presets(evenMoreBalanced: 100f, balancedMortician: 100f, original: 100f),
            note: GhoulDamageNote);
        GhoulsInheritEquipment = Bind(SecondarySection, "Ghouls Inherit Equipment",
            "Whether newly raised ghouls copy Mortician's items.",
            Presets(evenMoreBalanced: false, balancedMortician: false, original: true));

        SacrificeDamagePercent = Bind(UtilitySection, "Detonation Damage Percent",
            "Damage of a sacrificed ghoul's explosion, as a percent of the ghoul's damage stat.",
            Presets(evenMoreBalanced: 600f, balancedMortician: 925f, original: 700f),
            note: GhoulDamageNote);
        SacrificeRadius = Bind(UtilitySection, "Detonation Radius",
            "Radius of a sacrificed ghoul's explosion, in meters.",
            Presets(evenMoreBalanced: 20f, balancedMortician: 20f, original: 18f));
        SacrificeHealPercent = Bind(UtilitySection, "Heal Percent",
            "Percent of Mortician's maximum health healed by each Sacrifice.",
            Presets(evenMoreBalanced: 10f, balancedMortician: 15f, original: 15f));

        TombstoneBaseDamage = Bind(SpecialSection, "Tombstone Base Damage",
            "Tombstone damage stat at level 1. Sets the damage of vengeful souls and launched tombstones.",
            Presets(evenMoreBalanced: 12f, balancedMortician: 12f, original: 12f));
        TombstoneDamagePerLevel = Bind(SpecialSection, "Tombstone Damage Per Level",
            "Damage stat tombstones gain per level.",
            Presets(evenMoreBalanced: 2.4f, balancedMortician: 2.4f, original: 2.4f));
        SoulOrbDamagePercent = Bind(SpecialSection, "Soul Orb Damage Percent",
            "Vengeful soul explosion damage, as a percent of the tombstone's damage stat.",
            Presets(evenMoreBalanced: 200f, balancedMortician: 200f, original: 350f),
            note: SoulOrbDamageNote);
        TombstoneGhoulSpawnInterval = Bind(SpecialSection, "Ghoul Spawn Interval",
            "Seconds between ghouls raised by the tombstone.",
            Presets(evenMoreBalanced: 10f, balancedMortician: 10f, original: 10f));
        TombstoneLifetime = Bind(SpecialSection, "Tombstone Lifetime",
            "Seconds a tombstone lasts before crumbling. 0 means it lasts until replaced or destroyed.",
            Presets(evenMoreBalanced: 23f, balancedMortician: 0f, original: 0f));

        PresetSelector = new PresetSelector(config, preset, _presetSettings);
    }

    private static PresetValues<T> Presets<T>(T evenMoreBalanced, T balancedMortician, T original) =>
        new(evenMoreBalanced, balancedMortician, original);

    private PresetSetting<T> Bind<T>(string section, string key, string summary, PresetValues<T> presets, string vanilla = null, string note = null)
    {
        var entry = _config.Bind(section, key, presets.EvenMoreBalanced, Describe(summary, presets, vanilla, note));
        var setting = new PresetSetting<T>(entry, presets);
        _presetSettings.Add(setting);

        return setting;
    }

    private static string Describe<T>(string summary, PresetValues<T> presets, string vanilla, string note)
    {
        var lines = new List<string>
        {
            summary,
            $"Presets:",
            $"Original {Format(presets.Original)}",
            $"BalancedMortician {Format(presets.BalancedMortician)}",
            $"EvenMoreBalanced {Format(presets.EvenMoreBalanced)}"
        };

        if (vanilla != null)
            lines.Add("Vanilla: " + vanilla);

        if (note != null)
            lines.Add("Note: " + note);

        return string.Join("\n", lines);
    }

    private static string Format<T>(T value) => value is bool flag
        ? flag ? "true" : "false"
        : Convert.ToString(value, CultureInfo.InvariantCulture);
}
