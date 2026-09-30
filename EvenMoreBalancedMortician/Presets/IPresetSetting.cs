namespace EvenMoreBalancedMortician.Presets;

internal interface IPresetSetting
{
    string SerializedLocalValue { get; }

    void ApplyPreset(MorticianPreset preset);
    bool Matches(MorticianPreset preset);

    void UseHostValue(string serializedValue);
    void UseLocalValue();
}
