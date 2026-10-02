using System.Reflection;
using Morris.Components;

namespace EvenMoreBalancedMortician.Patches;

internal static class MorticianMethods
{
    public static MethodInfo MinionStart { get; } = typeof(MorrisMinionController).GetMethod("Start", BindingFlags.Instance | BindingFlags.NonPublic);
    public static MethodInfo TombstoneStart { get; } = typeof(TombstoneController).GetMethod(nameof(TombstoneController.Start));
}
