using Morris.Components;

namespace EvenMoreBalancedMortician.Patches;

internal static class DyingGhoul
{
    private static readonly ScopedValue<MorrisMinionController> _scope = new();

    public static MorrisMinionController Current => _scope.Current;

    public static void Install() => _scope.SetDuring(MorticianMethods.MinionDeathStart);
}
