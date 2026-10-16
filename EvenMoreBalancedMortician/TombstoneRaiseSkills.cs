using EntityStates;
using Morris;
using RoR2;
using RoR2.Skills;
using UnityEngine;
using MorticianContent = Morris.Modules.Content;
using MorticianSkills = Morris.Modules.Skills;

namespace EvenMoreBalancedMortician;

internal static class TombstoneRaiseSkills
{
    private static readonly string GhoulSpawnName = "DELFOFTHEBLA_TOMBSTONE_GHOUL_SPAWN";
    private static readonly string GhoulRaiseName = "DELFOFTHEBLA_TOMBSTONE_GHOUL_RAISE";

    public static SkillDef GhoulSpawn { get; private set; }
    public static SkillDef GhoulRaise { get; private set; }

    public static bool Register()
    {
        var bodyPrefab = MorrisPlugin.TombstoneBodyPrefab;
        var skills = bodyPrefab
            ? bodyPrefab.GetComponent<SkillLocator>()
            : null;

        if (!skills)
        {
            EvenMoreBalancedMorticianPlugin.Log.LogError("Tombstone skill locator not found; the tombstone keeps Mortician's own ghoul spawn timer.");
            return false;
        }

        var ghoulSpawnSlot = AddHiddenSlot(bodyPrefab, "GhoulSpawn", GhoulSpawnName);
        var ghoulRaiseSlot = AddHiddenSlot(bodyPrefab, "GhoulRaise", GhoulRaiseName);

        skills.primary = ghoulSpawnSlot;
        GhoulSpawn = ghoulSpawnSlot.skillFamily.defaultSkillDef;
        GhoulRaise = ghoulRaiseSlot.skillFamily.defaultSkillDef;

        return true;
    }

    public static GenericSkill GhoulSpawnSlotOf(CharacterBody tombstone) => SlotOf(tombstone, GhoulSpawn);

    public static GenericSkill GhoulRaiseSlotOf(CharacterBody tombstone) => SlotOf(tombstone, GhoulRaise);

    public static bool TryConsume(GenericSkill slot)
    {
        if (!slot || slot.stock <= 0)
            return false;

        slot.DeductStock(1);
        return true;
    }

    private static GenericSkill SlotOf(CharacterBody tombstone, SkillDef skillDef) => skillDef && tombstone && tombstone.skillLocator
        ? tombstone.skillLocator.FindSkillByDef(skillDef)
        : null;

    private static GenericSkill AddHiddenSlot(GameObject bodyPrefab, string familyName, string skillName)
    {
        var skillDef = ScriptableObject.CreateInstance<SkillDef>();
        ((ScriptableObject)skillDef).name = skillName;
        skillDef.skillName = skillName;
        skillDef.skillNameToken = skillName;
        skillDef.activationState = new SerializableEntityStateType(typeof(Idle));
        skillDef.dontAllowPastMaxStocks = true;

        MorticianContent.AddSkillDef(skillDef);

        var slot = MorticianSkills.CreateGenericSkillWithSkillFamily(bodyPrefab, familyName, hidden: true);
        MorticianSkills.AddSkillToFamily(slot.skillFamily, skillDef);

        return slot;
    }
}
