using System;

namespace EvenMoreBalancedMortician.Presets;

internal readonly struct PresetValues<T>(T original, T balancedMortician, T evenMoreBalanced)
{
    public T Original { get; } = original;
    public T BalancedMortician { get; } = balancedMortician;
    public T EvenMoreBalanced { get; } = evenMoreBalanced;

    public T For(MorticianPreset preset) => preset switch
    {
        MorticianPreset.Original => Original,
        MorticianPreset.BalancedMortician => BalancedMortician,
        MorticianPreset.EvenMoreBalanced => EvenMoreBalanced,
        _ => throw new ArgumentOutOfRangeException(nameof(preset), preset, "Custom has no preset values."),
    };
}
