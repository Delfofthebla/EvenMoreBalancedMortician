using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using BepInEx.Configuration;
using EvenMoreBalancedMortician.Presets;

namespace EvenMoreBalancedMortician;

internal sealed class MorticianSettings
{
    private readonly ConfigFile _config;
    private readonly List<IPresetSetting> _presetSettings = [];

    private const string GeneralSection = "General";
    private const string BaseStatsSection = "Base Stats";
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
    private const string ProcCoefficientVanilla =
        "Most survivor attacks use 1.0. Rapid multi-hit attacks use less, such as MUL-T's nailgun at 0.6.";

    public PresetSelector PresetSelector { get; }

    public PresetSetting<float> BaseHealth { get; }
    public PresetSetting<float> HealthPerLevel { get; }
    public PresetSetting<float> BaseRegen { get; }
    public PresetSetting<float> RegenPerLevel { get; }
    public PresetSetting<float> BaseArmor { get; }
    public PresetSetting<float> ArmorPerLevel { get; }
    public PresetSetting<float> BaseDamage { get; }
    public PresetSetting<float> DamagePerLevel { get; }

    public PresetSetting<bool> ShovelCountsAsPrimarySkill { get; }
    public PresetSetting<float> ShovelDamagePercent { get; }
    public PresetSetting<float> LaunchDamagePercent { get; }
    public PresetSetting<float> LaunchProcCoefficient { get; }

    public PresetSetting<float> RaiseDeadCooldown { get; }
    public PresetSetting<int> GhoulLimit { get; }
    public PresetSetting<float> GhoulBaseDamage { get; }
    public PresetSetting<float> GhoulDamagePerLevel { get; }
    public PresetSetting<float> GhoulDegen { get; }
    public PresetSetting<float> GhoulDegenPerLevel { get; }
    public PresetSetting<float> GhoulBiteDamagePercent { get; }
    public PresetSetting<float> GhoulBiteProcCoefficient { get; }
    public PresetSetting<float> GhoulClingBiteProcCoefficient { get; }
    public PresetSetting<float> GhoulSpitDamagePercent { get; }
    public PresetSetting<float> GhoulSpitProcCoefficient { get; }
    public PresetSetting<float> GhoulAspectInheritChance { get; }

    public PresetSetting<float> SacrificeCooldown { get; }
    public PresetSetting<float> SacrificeDamagePercent { get; }
    public PresetSetting<float> SacrificeProcCoefficient { get; }
    public PresetSetting<float> SacrificeRadius { get; }
    public PresetSetting<float> SacrificeHealPercent { get; }

    public PresetSetting<float> TombstoneCooldown { get; }
    public PresetSetting<float> TombstoneDuration { get; }
    public PresetSetting<float> TombstoneBaseDamage { get; }
    public PresetSetting<float> TombstoneDamagePerLevel { get; }
    public PresetSetting<float> SoulOrbDamagePercent { get; }
    public PresetSetting<float> SoulOrbProcCoefficient { get; }
    public PresetSetting<float> TombstoneGhoulSpawnInterval { get; }

    public MorticianSettings(ConfigFile config)
    {
        _config = config;

        var preset = config.Bind(GeneralSection, "Preset", MorticianPreset.EvenMoreBalanced,
            "Which preset the other settings follow. Choosing a preset overwrites every other setting with that preset's values; editing any setting afterwards switches this to Custom.\n" +
            "EvenMoreBalanced: Delf's rebalance.\n" +
            "BalancedMortician: Equivalent to Bloonjitsu7's BalancedMortician mod. \n" +
            "Original: Mortician with no changes.");

        BaseHealth = Bind(BaseStatsSection, "Base Health",
            "Mortician's maximum health at level 1.",
            Presets(original: 200f, balancedMortician: 200f, evenMoreBalanced: 170f),
            vanilla: "Most survivors sit at 110. Loader and Acrid 160. MUL-T 200.");
        HealthPerLevel = Bind(BaseStatsSection, "Health Per Level",
            "Maximum health Mortician gains per level.",
            Presets(original: 66f, balancedMortician: 66f, evenMoreBalanced: 51f),
            vanilla: "Every survivor gains 30% of their base health, so 33 for a 110 health survivor.");
        BaseRegen = Bind(BaseStatsSection, "Base Regen",
            "Health Mortician regenerates per second at level 1.",
            Presets(original: 2.5f, balancedMortician: 2.5f, evenMoreBalanced: 1f),
            vanilla: "Most survivors 1. Loader and Acrid 2.5.");
        RegenPerLevel = Bind(BaseStatsSection, "Regen Per Level",
            "Regen per second Mortician gains per level.",
            Presets(original: 0.5f, balancedMortician: 0.5f, evenMoreBalanced: 0.2f),
            vanilla: "Most survivors 0.2. Loader and Acrid 0.5.");
        BaseArmor = Bind(BaseStatsSection, "Base Armor",
            "Mortician's armor at level 1.",
            Presets(original: 20f, balancedMortician: 20f, evenMoreBalanced: 10f),
            vanilla: "Most survivors 0. Mercenary, Loader, Acrid, REX, Seeker and Drifter 20. MUL-T 12.");
        ArmorPerLevel = Bind(BaseStatsSection, "Armor Per Level",
            "Armor Mortician gains per level.",
            Presets(original: 0f, balancedMortician: 0f, evenMoreBalanced: 0f),
            vanilla: "Every survivor 0.");
        BaseDamage = Bind(BaseStatsSection, "Base Damage",
            "Mortician's damage at level 1. Only the shovel swing scales with it; ghoul and tombstone attacks use their own damage stats.",
            Presets(original: 12f, balancedMortician: 12f, evenMoreBalanced: 12f),
            vanilla: "Most survivors 12. MUL-T 11. Engineer 14. Acrid 15.");
        DamagePerLevel = Bind(BaseStatsSection, "Damage Per Level",
            "Damage Mortician gains per level.",
            Presets(original: 2.4f, balancedMortician: 2.4f, evenMoreBalanced: 2.4f),
            vanilla: "Every survivor gains 20% of their base damage, so 2.4 for a 12 damage survivor.");

        ShovelCountsAsPrimarySkill = Bind(PrimarySection, "Counts As Primary Skill Damage",
            "Whether shovel swings count as primary skill damage. Items that trigger on primary skill hits (such as Luminous Shot) require this.",
            Presets(original: false, balancedMortician: true, evenMoreBalanced: true));
        ShovelDamagePercent = Bind(PrimarySection, "Swing Damage Percent",
            "Shovel swing damage, as a percent of Mortician's damage stat.",
            Presets(original: 800f, balancedMortician: 360f, evenMoreBalanced: 280f));
        LaunchDamagePercent = Bind(PrimarySection, "Launch Damage Percent",
            "Damage a ghoul or tombstone deals when flung through enemies by your shovel, as a percent of their own damage stats.",
            Presets(original: 350f, balancedMortician: 600f, evenMoreBalanced: 360f),
            note: LaunchDamageNote);
        LaunchProcCoefficient = Bind(PrimarySection, "Launch Proc Coefficient",
            "How strongly each hit from a launched ghoul or tombstone triggers Mortician's on-hit items. 1.0 is full strength.",
            Presets(original: 1f, balancedMortician: 1f, evenMoreBalanced: 1f),
            vanilla: ProcCoefficientVanilla);

        RaiseDeadCooldown = Bind(SecondarySection, "Cooldown",
            "Seconds Raise Dead takes to recharge one of its 2 charges. Cooldown reduction items still apply.",
            Presets(original: 7f, balancedMortician: 7f, evenMoreBalanced: 7f));
        GhoulLimit = Bind(SecondarySection, "Ghoul Limit",
            "Maximum number of ghouls Mortician can have at once; raising another kills the oldest. 0 means no limit.",
            Presets(original: 0, balancedMortician: 0, evenMoreBalanced: 0),
            note: "Replaces the Ghoul limit setting in Mortician's own config.");
        GhoulBaseDamage = Bind(SecondarySection, "Ghoul Base Damage",
            "Ghoul damage at level 1. Sets the damage of ghoul bites, spit, launched ghouls and Sacrifice explosions.",
            Presets(original: 12f, balancedMortician: 8f, evenMoreBalanced: 8f));
        GhoulDamagePerLevel = Bind(SecondarySection, "Ghoul Damage Per Level",
            "Damage ghouls gain per level.",
            Presets(original: 2.4f, balancedMortician: 1.6f, evenMoreBalanced: 1.6f));
        GhoulDegen = Bind(SecondarySection, "Ghoul Degen",
            "Health each ghoul loses per second at level 1. 0 means ghouls never decay.",
            Presets(original: 10f, balancedMortician: 10f, evenMoreBalanced: 10f),
            note: "Ghouls start with 150 health and gain 45 per level, so the original values give every ghoul about 15 seconds to live at any level.");
        GhoulDegenPerLevel = Bind(SecondarySection, "Ghoul Degen Per Level",
            "Health lost per second that ghouls gain per level.",
            Presets(original: 3f, balancedMortician: 3f, evenMoreBalanced: 3f));
        GhoulBiteDamagePercent = Bind(SecondarySection, "Bite Damage Percent",
            "Ghoul bite damage as a percent of the ghoul's damage stat.",
            Presets(original: 150f, balancedMortician: 150f, evenMoreBalanced: 150f),
            note: GhoulDamageNote);
        GhoulBiteProcCoefficient = Bind(SecondarySection, "Bite Proc Coefficient",
            "How strongly each regular ghoul bite triggers Mortician's on-hit items. 1.0 is full strength.",
            Presets(original: 1f, balancedMortician: 1f, evenMoreBalanced: 1f),
            vanilla: ProcCoefficientVanilla);
        GhoulClingBiteProcCoefficient = Bind(SecondarySection, "Cling Bite Proc Coefficient",
            "How strongly each bite from a clinging ghoul triggers Mortician's on-hit items. 1.0 is full strength. A ghoul launched into a large enemy latches on and bites it every 0.7 seconds.",
            Presets(original: 0.8f, balancedMortician: 0.8f, evenMoreBalanced: 0.8f),
            vanilla: ProcCoefficientVanilla);
        GhoulSpitDamagePercent = Bind(SecondarySection, "Spit Damage Percent",
            "Ghoul bile spit damage as a percent of the ghoul's damage stat.",
            Presets(original: 100f, balancedMortician: 100f, evenMoreBalanced: 100f),
            note: GhoulDamageNote);
        GhoulSpitProcCoefficient = Bind(SecondarySection, "Spit Proc Coefficient",
            "How strongly each ghoul bile spit triggers Mortician's on-hit items. 1.0 is full strength.",
            Presets(original: 1f, balancedMortician: 1f, evenMoreBalanced: 0.7f),
            vanilla: ProcCoefficientVanilla);
        GhoulAspectInheritChance = Bind(SecondarySection, "Aspect Inherit Chance Percent",
            "Chance for a newly raised ghoul to copy each elite aspect Mortician holds. An aspect equipment rolls once; aspect items roll once per stack. 0 disables. Ghouls never copy any other items or equipment.",
            Presets(original: 100f, balancedMortician: 0f, evenMoreBalanced: 25f),
            note: "Aspect items are any item with \"Aspect\" in its name, such as ZetAspects' aspect items. The Original preset matches the base mod for aspect equipment, but the base mod never passed on aspect items.");

        SacrificeCooldown = Bind(UtilitySection, "Cooldown",
            "Seconds Sacrifice takes to recharge. Cooldown reduction items still apply.",
            Presets(original: 6f, balancedMortician: 6f, evenMoreBalanced: 6f));
        SacrificeDamagePercent = Bind(UtilitySection, "Detonation Damage Percent",
            "Damage of a sacrificed ghoul's explosion, as a percent of the ghoul's damage stat.",
            Presets(original: 700f, balancedMortician: 925f, evenMoreBalanced: 880f),
            note: GhoulDamageNote);
        SacrificeProcCoefficient = Bind(UtilitySection, "Detonation Proc Coefficient",
            "How strongly a sacrificed ghoul's explosion triggers Mortician's on-hit items, against every enemy it hits. 1.0 is full strength.",
            Presets(original: 1f, balancedMortician: 1f, evenMoreBalanced: 1f),
            vanilla: ProcCoefficientVanilla);
        SacrificeRadius = Bind(UtilitySection, "Detonation Radius",
            "Radius of a sacrificed ghoul's explosion, in meters.",
            Presets(original: 18f, balancedMortician: 20f, evenMoreBalanced: 20f));
        SacrificeHealPercent = Bind(UtilitySection, "Heal Percent",
            "Percent of Mortician's maximum health healed by each Sacrifice.",
            Presets(original: 15f, balancedMortician: 15f, evenMoreBalanced: 10f));

        TombstoneCooldown = Bind(SpecialSection, "Cooldown",
            "Seconds Tombstone takes to recharge. Cooldown reduction items still apply.",
            Presets(original: 30f, balancedMortician: 30f, evenMoreBalanced: 30f),
            vanilla: "Engineer's turrets 30.",
            note: "Tombstone Duration does not follow this; adjust it separately.");
        TombstoneDuration = Bind(SpecialSection, "Tombstone Duration",
            "Seconds a tombstone lasts before crumbling. 0 means it lasts until replaced or destroyed.",
            Presets(original: 0f, balancedMortician: 0f, evenMoreBalanced: 30f));
        TombstoneBaseDamage = Bind(SpecialSection, "Tombstone Base Damage",
            "Tombstone damage stat at level 1. Sets the damage of vengeful souls and launched tombstones.",
            Presets(original: 12f, balancedMortician: 12f, evenMoreBalanced: 12f));
        TombstoneDamagePerLevel = Bind(SpecialSection, "Tombstone Damage Per Level",
            "Damage stat tombstones gain per level.",
            Presets(original: 2.4f, balancedMortician: 2.4f, evenMoreBalanced: 2.4f));
        SoulOrbDamagePercent = Bind(SpecialSection, "Soul Orb Damage Percent",
            "Vengeful soul explosion damage, as a percent of the tombstone's damage stat.",
            Presets(original: 350f, balancedMortician: 200f, evenMoreBalanced: 250f),
            note: SoulOrbDamageNote);
        SoulOrbProcCoefficient = Bind(SpecialSection, "Soul Orb Proc Coefficient",
            "How strongly each vengeful soul explosion triggers on-hit items. 1.0 is full strength.",
            Presets(original: 0.2f, balancedMortician: 0.2f, evenMoreBalanced: 0.2f),
            vanilla: ProcCoefficientVanilla,
            note: "Triggers the tombstone's items, which it copies from Mortician when placed, not Mortician's own.");
        TombstoneGhoulSpawnInterval = Bind(SpecialSection, "Ghoul Spawn Interval",
            "Seconds between ghouls raised by the tombstone.",
            Presets(original: 10f, balancedMortician: 10f, evenMoreBalanced: 10f));

        PresetSelector = new PresetSelector(config, preset, _presetSettings);
    }

    public string[] SerializeLocalValues() => _presetSettings.Select(setting => setting.SerializedLocalValue).ToArray();

    public bool TryUseHostValues(IReadOnlyList<string> hostValues)
    {
        if (hostValues.Count != _presetSettings.Count)
            return false;

        for (var i = 0; i < hostValues.Count; i++)
            _presetSettings[i].UseHostValue(hostValues[i]);

        return true;
    }

    public void UseLocalValues()
    {
        foreach (var setting in _presetSettings)
            setting.UseLocalValue();
    }

    private static PresetValues<T> Presets<T>(T original, T balancedMortician, T evenMoreBalanced) =>
        new(original, balancedMortician, evenMoreBalanced);

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
