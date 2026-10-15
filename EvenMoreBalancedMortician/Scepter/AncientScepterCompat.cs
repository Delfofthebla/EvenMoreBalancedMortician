using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using AncientScepter;
using BepInEx.Bootstrap;
using MonoMod.RuntimeDetour;
using RoR2;
using RoR2.Skills;

namespace EvenMoreBalancedMortician.Scepter;

// Every method that touches AncientScepter's types is kept out of line so AncientScepter.dll is only loaded when the mod is installed.
internal static class AncientScepterCompat
{
    public const string GUID = "com.DestroyedClone.AncientScepter";

    private delegate bool TryGetScepterSlot(AncientScepterItem self, CharacterBody body, out SkillSlot slot);

    private static Hook _handleScepterSkillHook;
    private static Hook _tryGetScepterSlotHook;
    private static Func<CharacterBody, bool> _isUpgradeWithheld;

    public static bool IsInstalled => Chainloader.PluginInfos.ContainsKey(GUID);

    private static AncientScepterItem Scepter => ItemBase<AncientScepterItem>.instance;

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void RegisterSpecialUpgrade(SkillDef upgrade, string bodyName) => Scepter.RegisterScepterSkill(upgrade, bodyName, SkillSlot.Special, 0);

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void WithholdUpgradeFrom(Func<CharacterBody, bool> isUpgradeWithheld)
    {
        _isUpgradeWithheld = isUpgradeWithheld;

        _handleScepterSkillHook = new Hook(ScepterMethod(nameof(AncientScepterItem.HandleScepterSkill)), SkipWithheldUpgrade);
        _tryGetScepterSlotHook = new Hook(ScepterMethod(nameof(AncientScepterItem.TryGetScepterSlot)), SkipWithheldCooldownReduction);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void ReapplyUpgrade(CharacterBody body) => Scepter.HandleScepterSkill(body);

    private static MethodInfo ScepterMethod(string name) => typeof(AncientScepterItem).GetMethod(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

    private static bool SkipWithheldUpgrade(Func<AncientScepterItem, CharacterBody, bool, bool> orig, AncientScepterItem self, CharacterBody body, bool forceOff)
        => !_isUpgradeWithheld(body) && orig(self, body, forceOff);

    private static bool SkipWithheldCooldownReduction(TryGetScepterSlot orig, AncientScepterItem self, CharacterBody body, out SkillSlot slot)
    {
        if (!_isUpgradeWithheld(body))
            return orig(self, body, out slot);

        slot = SkillSlot.None;
        return false;
    }
}
