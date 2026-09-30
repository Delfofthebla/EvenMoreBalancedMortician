using System.Collections.Generic;
using BepInEx.Configuration;

namespace EvenMoreBalancedMortician.Presets;

internal sealed class PresetSetting<T>(ConfigEntry<T> entry, PresetValues<T> presetValues) : IPresetSetting
{
    public ConfigEntry<T> Entry { get; } = entry;
    public T Value => Entry.Value;
    public void ApplyPreset(MorticianPreset preset) => Entry.Value = presetValues.For(preset);

    public bool Matches(MorticianPreset preset) => EqualityComparer<T>.Default.Equals(Entry.Value, presetValues.For(preset));
}
