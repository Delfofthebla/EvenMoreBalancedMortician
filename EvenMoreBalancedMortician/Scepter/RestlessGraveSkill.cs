using EvenMoreBalancedMortician.Presets;
using Morris;
using R2API;
using RoR2;
using RoR2.Skills;
using UnityEngine;

namespace EvenMoreBalancedMortician.Scepter;

internal static class RestlessGraveSkill
{
    private const string SkillName = "DELFOFTHEBLA_MORTICIAN_SPECIAL_RESTLESS_GRAVE";

    public const string NameToken = SkillName + "_NAME";
    public const string DescriptionToken = SkillName + "_DESCRIPTION";

    private static PresetSetting<bool> _enabled;

    public static SkillDef Definition { get; private set; }

    public static void Register(PresetSetting<bool> enabled)
    {
        _enabled = enabled;

        var bodyPrefab = MorrisPlugin.MorrisBodyPrefab;
        var skills = bodyPrefab ? bodyPrefab.GetComponent<SkillLocator>() : null;
        if (!skills)
        {
            EvenMoreBalancedMorticianPlugin.Log.LogError("Mortician skill locator not found; the Ancient Scepter will not upgrade Tombstone.");
            return;
        }

        Definition = Object.Instantiate(skills.special.skillFamily.variants[0].skillDef);
        ((ScriptableObject)Definition).name = SkillName;
        Definition.skillName = SkillName;
        Definition.skillNameToken = NameToken;
        Definition.skillDescriptionToken = DescriptionToken;
        Definition.icon = ScepterIcon.TintedCopyOf(Definition.icon);

        ContentAddition.AddSkillDef(Definition);
        AncientScepterCompat.RegisterSpecialUpgrade(Definition, bodyPrefab.name);
        AncientScepterCompat.WithholdUpgradeFrom(IsWithheldFrom);
    }

    public static void ApplyEnabledSetting()
    {
        if (!Definition)
            return;

        foreach (var body in LiveBodies.Of(MorrisPlugin.MorrisBodyIndex))
        {
            if (_enabled.Value)
                AncientScepterCompat.ReapplyUpgrade(body);
            else
                body.skillLocator.special.UnsetSkillOverride(body, Definition, GenericSkill.SkillOverridePriority.Upgrade);

            body.MarkAllStatsDirty();
        }
    }

    public static bool IsEquippedBy(CharacterBody body)
        => Definition && body && body.skillLocator && body.skillLocator.special && body.skillLocator.special.skillDef == Definition;

    private static bool IsWithheldFrom(CharacterBody body) => !_enabled.Value && body.bodyIndex == MorrisPlugin.MorrisBodyIndex;
}
