using System.Collections.Generic;
using BepInEx.Configuration;

namespace EvenMoreBalancedMortician.Presets;

internal sealed class PresetSetting<T>(ConfigEntry<T> entry, PresetValues<T> presetValues) : IPresetSetting
{
    private bool _hasHostValue;
    private T _hostValue;

    public T Value => _hasHostValue ? _hostValue : entry.Value;
    public string SerializedLocalValue => TomlTypeConverter.ConvertToString(entry.Value, typeof(T));

    public void ApplyPreset(MorticianPreset preset) => entry.Value = presetValues.For(preset);

    public bool Matches(MorticianPreset preset) => EqualityComparer<T>.Default.Equals(entry.Value, presetValues.For(preset));

    public void UseHostValue(string serializedValue)
    {
        _hostValue = TomlTypeConverter.ConvertToValue<T>(serializedValue);
        _hasHostValue = true;
    }

    public void UseLocalValue() => _hasHostValue = false;
}
