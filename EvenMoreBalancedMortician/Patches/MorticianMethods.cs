using System.Reflection;
using Morris.Components;

namespace EvenMoreBalancedMortician.Patches;

internal static class MorticianMethods
{
    public static MethodInfo MinionStart { get; } = typeof(MorrisMinionController).GetMethod("Start", BindingFlags.Instance | BindingFlags.NonPublic);
    public static MethodInfo MinionDeathStart { get; } = typeof(MorrisMinionController).GetMethod(nameof(MorrisMinionController.OnDeathStart));
    public static MethodInfo TombstoneStart { get; } = typeof(TombstoneController).GetMethod(nameof(TombstoneController.Start));
    public static MethodInfo TombstoneFixedUpdate { get; } = typeof(TombstoneController).GetMethod(nameof(TombstoneController.FixedUpdate));
}
