using System;
using System.Collections.Generic;
using System.Linq;
using EvenMoreBalancedMortician.Presets;
using MonoMod.RuntimeDetour;
using Morris.Components;
using RoR2;
using UnityEngine.Networking;

namespace EvenMoreBalancedMortician.Patches;

internal static class GhoulAspectPatch
{
    private static readonly string AspectNameFragment = "Aspect";

    private static readonly HashSet<string> _detectedAspects = [];

    private static Hook _minionStartHook;
    private static Hook _minionDeathStartHook;
    private static PresetSetting<float> _inheritChancePercent;

    public static void Install(PresetSetting<float> inheritChancePercent)
    {
        _inheritChancePercent = inheritChancePercent;

        _minionStartHook = new Hook(MorticianMethods.MinionStart, InheritAspects);
        _minionDeathStartHook = new Hook(MorticianMethods.MinionDeathStart, ShedAspectsBeforeOnKillProc);
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

        var location = ghoulInventory.FindBestEquipmentLocationOrCurrent(Inventory.EquipmentLocationQueryArgs.defaultPickup);
        ghoulInventory.SetEquipmentIndex(equipment, location);
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

    // Mortician's on-kill proc names the dying ghoul as its victim, so an aspected ghoul would count as an elite kill and repeat its elite death effects.
    private static void ShedAspectsBeforeOnKillProc(Action<MorrisMinionController> orig, MorrisMinionController self)
    {
        if (NetworkServer.active && self.minionType == MorrisMinionController.MorrisMinionType.Ghoul && self.characterBody)
            ShedAspects(self.characterBody);

        orig(self);
    }

    private static void ShedAspects(CharacterBody ghoul)
    {
        var eliteBuffs = BuffCatalog.eliteBuffIndices.Where(ghoul.HasBuff).ToArray();
        if (eliteBuffs.Length == 0)
            return;

        if (ghoul.inventory)
            RemoveAspects(ghoul.inventory);

        foreach (var eliteBuff in eliteBuffs)
        {
            ghoul.ClearTimedBuffs(eliteBuff);
            ghoul.SetBuffCount(eliteBuff, 0);
        }

        RefreshEliteStatus(ghoul);
    }

    private static void RemoveAspects(Inventory ghoulInventory)
    {
        if (IsAspectEquipment(ghoulInventory.currentEquipmentIndex))
            ghoulInventory.SetEquipmentIndex(EquipmentIndex.None, ghoulInventory.activeEquipmentLocation);

        foreach (var aspect in ghoulInventory.itemAcquisitionOrder.Where(IsAspectItem).ToArray())
            ghoulInventory.ResetItemPermanent(aspect);
    }

    private static void RefreshEliteStatus(CharacterBody ghoul) => ghoul.RecalculateStats();

    private static bool IsAspectEquipment(EquipmentIndex equipment)
    {
        var equipmentDef = EquipmentCatalog.GetEquipmentDef(equipment);
        return equipmentDef && equipmentDef.passiveBuffDef && equipmentDef.passiveBuffDef.isElite;
    }

    private static bool IsAspectItem(ItemIndex item)
    {
        var itemDef = ItemCatalog.GetItemDef(item);
        return !itemDef.hidden && itemDef.name.IndexOf(AspectNameFragment, StringComparison.OrdinalIgnoreCase) >= 0;
    }

    private static bool RollInheritChance(CharacterMaster owner) => Util.CheckRoll(_inheritChancePercent.Value, owner);

    private static void LogFirstDetection(string aspectKind, string aspectName)
    {
        if (_detectedAspects.Add(aspectKind + ":" + aspectName))
            EvenMoreBalancedMorticianPlugin.Log.LogInfo($"Ghouls can inherit aspect {aspectKind}: {aspectName}");
    }
}
