using System;
using EvenMoreBalancedMortician.Presets;
using MonoMod.RuntimeDetour;
using RoR2;
using SkillStates.Morris;

namespace EvenMoreBalancedMortician.Patches;

internal static class ShovelDamageSourcePatch
{
    private static Hook _swingHook;
    private static PresetSetting<bool> _shovelCountsAsPrimarySkill;

    public static void Install(PresetSetting<bool> countsAsPrimarySkill)
    {
        _shovelCountsAsPrimarySkill = countsAsPrimarySkill;

        var onEnter = typeof(SwingShovel).GetMethod(nameof(SwingShovel.OnEnter));
        _swingHook = new Hook(onEnter, (Action<Action<SwingShovel>, SwingShovel>)TagSwingAsPrimaryIfEnabled);
    }

    private static void TagSwingAsPrimaryIfEnabled(Action<SwingShovel> orig, SwingShovel self)
    {
        orig(self);

        if (!_shovelCountsAsPrimarySkill.Value)
            return;

        self.attack.damageType.damageSource = DamageSource.Primary;
    }
}
