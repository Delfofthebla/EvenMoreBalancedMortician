namespace EvenMoreBalancedMortician.Presets;

internal interface IPresetSetting
{
    void ApplyPreset(MorticianPreset preset);
    bool Matches(MorticianPreset preset);
}
