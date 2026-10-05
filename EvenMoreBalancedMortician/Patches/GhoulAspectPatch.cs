using System;
using System.Collections.Generic;
using EvenMoreBalancedMortician.Presets;
using MonoMod.RuntimeDetour;
using Morris.Components;
using RoR2;
using UnityEngine.Networking;

namespace EvenMoreBalancedMortician.Patches;

internal static class GhoulAspectPatch
{
    private const string AspectNameFragment = "Aspect";

    private static readonly HashSet<string> _detectedAspects = [];

    private static Hook _minionStartHook;
    private static PresetSetting<float> _inheritChancePercent;

    public static void Install(PresetSetting<float> inheritChancePercent)
    {
        _inheritChancePercent = inheritChancePercent;

        _minionStartHook = new Hook(MorticianMethods.MinionStart, (Action<Action<MorrisMinionController>, MorrisMinionController>)InheritAspects);
    }

    private static void InheritAspects(Action<MorrisMinionController> orig, MorrisMinionController self)
    {
        orig(self);

        if (!NetworkServer.active || self.minionType != MorrisMinionController.MorrisMinionType.Ghoul || _inheritChancePercent.Value <= 0f)
            return;

        var ghoulInventory = self.characterBody ? self.characterBody.inventory : null;
        var owner = self.ownerBody ? self.ownerBody.master : null;
        if (!ghoulInventory || !owner || !owner.inventory)
            return;

        InheritAspectEquipment(owner, ghoulInventory);

        foreach (var item in owner.inventory.itemAcquisitionOrder)
        {
            if (!IsAspectItem(item))
                continue;

            LogFirstDetection("item", ItemCatalog.GetItemDef(item).name);
            InheritAspectItemStacks(item, owner, ghoulInventory);
        }
    }

    private static void InheritAspectEquipment(CharacterMaster owner, Inventory ghoulInventory)
    {
        var equipment = owner.inventory.currentEquipmentIndex;
        if (!IsAspectEquipment(equipment))
            return;

        LogFirstDetection("equipment", EquipmentCatalog.GetEquipmentDef(equipment).name);
        if (!RollInheritChance(owner))
            return;

        ghoulInventory.SetEquipmentIndex(equipment, false);
    }

    private static void InheritAspectItemStacks(ItemIndex aspect, CharacterMaster owner, Inventory ghoulInventory)
    {
        var inheritedStacks = 0;
        var ownedStacks = owner.inventory.GetItemCountEffective(aspect);

        for (var stack = 0; stack < ownedStacks; stack++)
        {
            if (RollInheritChance(owner))
                inheritedStacks++;
        }

        if (inheritedStacks > 0)
            ghoulInventory.GiveItemPermanent(aspect, inheritedStacks);
    }

    private static bool IsAspectEquipment(EquipmentIndex equipment)
    {
        var equipmentDef = EquipmentCatalog.GetEquipmentDef(equipment);
        return equipmentDef && equipmentDef.passiveBuffDef && equipmentDef.passiveBuffDef.isElite;
    }

    private static bool IsAspectItem(ItemIndex item)
        => ItemCatalog.GetItemDef(item).name.IndexOf(AspectNameFragment, StringComparison.OrdinalIgnoreCase) >= 0;

    private static bool RollInheritChance(CharacterMaster owner) => Util.CheckRoll(_inheritChancePercent.Value, owner);

    private static void LogFirstDetection(string aspectKind, string aspectName)
    {
        if (_detectedAspects.Add(aspectKind + ":" + aspectName))
            EvenMoreBalancedMorticianPlugin.Log.LogInfo($"Ghouls can inherit aspect {aspectKind}: {aspectName}");
    }
}
