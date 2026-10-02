using System;
using EntityStates;
using EvenMoreBalancedMortician.Presets;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using RoR2;
using SkillStates.Ghoul;

namespace EvenMoreBalancedMortician.Patches;

internal static class DetonationDamagePatch
{
    private static ILHook _explodeHook;
    private static PresetSetting<bool> _scalesWithMortician;

    public static void Install(PresetSetting<bool> scalesWithMortician)
    {
        _scalesWithMortician = scalesWithMortician;
        _explodeHook = new ILHook(typeof(GhoulDeath).GetMethod(nameof(GhoulDeath.Explode)), ScaleWithSacrificerDamage);
    }

    private static void ScaleWithSacrificerDamage(ILContext il)
    {
        var cursor = new ILCursor(il);
        var foundDamageStat = cursor.TryGotoNext(MoveType.After,
            instruction => instruction.MatchLdfld<BaseState>(nameof(BaseState.damageStat)),
            instruction => instruction.MatchMul(),
            instruction => instruction.MatchStfld<BlastAttack>(nameof(BlastAttack.baseDamage)));

        if (!foundDamageStat)
        {
            EvenMoreBalancedMorticianPlugin.Log.LogError("Could not find the Sacrifice explosion's damage; it keeps scaling with the ghoul's damage stat.");
            return;
        }

        cursor.Index -= 2;
        cursor.Emit(Mono.Cecil.Cil.OpCodes.Ldarg_0);
        cursor.EmitDelegate<Func<float, GhoulDeath, float>>(DetonationDamageStat);
    }

    private static float DetonationDamageStat(float ghoulDamageStat, GhoulDeath ghoulDeath)
    {
        if (!_scalesWithMortician.Value)
            return ghoulDamageStat;

        var minion = ghoulDeath.minionController;
        var sacrificer = minion.sacrificeOwner ? minion.sacrificeOwner : minion.owner;
        var sacrificerBody = sacrificer ? sacrificer.GetComponent<CharacterBody>() : null;

        return sacrificerBody ? sacrificerBody.damage : ghoulDamageStat;
    }
}
