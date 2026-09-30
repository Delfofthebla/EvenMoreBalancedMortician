using System;

namespace EvenMoreBalancedMortician.Presets;

internal readonly struct PresetValues<T>(T evenMoreBalanced, T balancedMortician, T original)
{
    public T EvenMoreBalanced { get; } = evenMoreBalanced;
    public T BalancedMortician { get; } = balancedMortician;
    public T Original { get; } = original;

    public T For(MorticianPreset preset) => preset switch
    {
        MorticianPreset.EvenMoreBalanced => EvenMoreBalanced,
        MorticianPreset.BalancedMortician => BalancedMortician,
        MorticianPreset.Original => Original,
        _ => throw new ArgumentOutOfRangeException(nameof(preset), preset, "Custom has no preset values."),
    };
}
