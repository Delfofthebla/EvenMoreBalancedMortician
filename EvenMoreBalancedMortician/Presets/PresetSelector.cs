using System;
using System.Collections.Generic;
using System.Linq;
using BepInEx.Configuration;

namespace EvenMoreBalancedMortician.Presets;

internal sealed class PresetSelector
{
    private readonly ConfigFile _config;
    private readonly ConfigEntry<MorticianPreset> _preset;
    private readonly IReadOnlyList<IPresetSetting> _settings;
    private bool _isRewritingSettings;

    public event Action SettingsChanged;

    public PresetSelector(ConfigFile config, ConfigEntry<MorticianPreset> preset, ConfigEntry<string> configVersion, string modVersion, IReadOnlyList<IPresetSetting> settings)
    {
        _config = config;
        _preset = preset;
        _settings = settings;

        UpdatePresetFromEarlierVersion(configVersion, modVersion);
        SelectCustomIfSettingsDiverge();
        config.SettingChanged += OnSettingChanged;
    }

    private void UpdatePresetFromEarlierVersion(ConfigEntry<string> configVersion, string modVersion)
    {
        if (configVersion.Value == modVersion)
            return;

        ApplySelectedPreset();
        configVersion.Value = modVersion;
    }

    private void OnSettingChanged(object sender, SettingChangedEventArgs args)
    {
        if (_isRewritingSettings)
            return;

        if (args.ChangedSetting == _preset)
            ApplySelectedPreset();
        else
            SelectCustomIfSettingsDiverge();

        SettingsChanged?.Invoke();
    }

    private void ApplySelectedPreset()
    {
        if (_preset.Value == MorticianPreset.Custom)
            return;

        RewriteSettings(() =>
        {
            foreach (var setting in _settings)
                setting.ApplyPreset(_preset.Value);
        });
    }

    private void SelectCustomIfSettingsDiverge()
    {
        if (_preset.Value == MorticianPreset.Custom)
            return;

        if (_settings.All(setting => setting.Matches(_preset.Value)))
            return;

        RewriteSettings(() => _preset.Value = MorticianPreset.Custom);
    }

    private void RewriteSettings(Action rewrite)
    {
        var saveOnConfigSet = _config.SaveOnConfigSet;
        _config.SaveOnConfigSet = false;
        _isRewritingSettings = true;

        try
        {
            rewrite();
        }
        finally
        {
            _isRewritingSettings = false;
            _config.SaveOnConfigSet = saveOnConfigSet;
            _config.Save();
        }
    }
}
