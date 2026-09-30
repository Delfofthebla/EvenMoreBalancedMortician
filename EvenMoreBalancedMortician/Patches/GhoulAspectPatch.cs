using System;
using System.Collections.Generic;
using System.Reflection;
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

        var minionStart = typeof(MorrisMinionController).GetMethod("Start", BindingFlags.Instance | BindingFlags.NonPublic);
        _minionStartHook = new Hook(minionStart, (Action<Action<MorrisMinionController>, MorrisMinionController>)InheritAspects);
    }

    private static void InheritAspects(Action<MorrisMinionController> orig, MorrisMinionController self)
    {
        orig(self);

        if (!NetworkServer.active || self.minionType != MorrisMinionController.MorrisMinionType.Ghoul || _inheritChancePercent.Value <= 0f)
            return;

        var ghoulInventory = self.characterBody ? self.characterBody.inventory : null;
        var ownerInventory = self.ownerBody ? self.ownerBody.inventory : null;
        if (!ghoulInventory || !ownerInventory)
            return;

        InheritAspectEquipment(ownerInventory, ghoulInventory);

        foreach (var item in ownerInventory.itemAcquisitionOrder)
        {
            if (!IsAspectItem(item))
                continue;

            LogFirstDetection("item", ItemCatalog.GetItemDef(item).name);
            InheritAspectItemStacks(item, ownerInventory, ghoulInventory);
        }
    }

    private static void InheritAspectEquipment(Inventory ownerInventory, Inventory ghoulInventory)
    {
        var equipment = ownerInventory.currentEquipmentIndex;
        if (!IsAspectEquipment(equipment))
            return;

        LogFirstDetection("equipment", EquipmentCatalog.GetEquipmentDef(equipment).name);
        if (!RollInheritChance())
            return;

        ghoulInventory.SetEquipmentIndex(equipment, false);
    }

    private static void InheritAspectItemStacks(ItemIndex aspect, Inventory ownerInventory, Inventory ghoulInventory)
    {
        var inheritedStacks = 0;
        var ownedStacks = ownerInventory.GetItemCountEffective(aspect);

        for (var stack = 0; stack < ownedStacks; stack++)
        {
            if (RollInheritChance())
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

    private static bool IsAspectItem(ItemIndex item) =>
        ItemCatalog.GetItemDef(item).name.IndexOf(AspectNameFragment, StringComparison.OrdinalIgnoreCase) >= 0;

    private static bool RollInheritChance() => Util.CheckRoll(_inheritChancePercent.Value);

    private static void LogFirstDetection(string aspectKind, string aspectName)
    {
        if (_detectedAspects.Add(aspectKind + ":" + aspectName))
            EvenMoreBalancedMorticianPlugin.Log.LogInfo($"Ghouls can inherit aspect {aspectKind}: {aspectName}");
    }
}
