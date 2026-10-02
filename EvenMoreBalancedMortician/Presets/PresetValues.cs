using System;

namespace EvenMoreBalancedMortician.Presets;

internal readonly record struct PresetValues<T>(T Original, T BalancedMortician, T EvenMoreBalanced)
{
    public T For(MorticianPreset preset) => preset switch
    {
        MorticianPreset.Original => Original,
        MorticianPreset.BalancedMortician => BalancedMortician,
        MorticianPreset.EvenMoreBalanced => EvenMoreBalanced,
        _ => throw new ArgumentOutOfRangeException(nameof(preset), preset, "Custom has no preset values."),
    };
}
