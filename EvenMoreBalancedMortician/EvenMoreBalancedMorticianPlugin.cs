using BepInEx;
using BepInEx.Logging;
using EvenMoreBalancedMortician.Patches;
using Morris;
using R2API;

namespace EvenMoreBalancedMortician;

[BepInPlugin(Guid, Name, Version)]
[BepInDependency(MorrisPlugin.MODUID)]
[BepInDependency(LanguageAPI.PluginGUID)]
[BepInDependency(DeployableAPI.PluginGUID)]
[BepInIncompatibility(BalancedMorticianGuid)]
public class EvenMoreBalancedMorticianPlugin : BaseUnityPlugin
{
    public const string Guid = "com.Delfofthebla.EvenMoreBalancedMortician";
    public const string Name = "EvenMoreBalancedMortician";
    public const string Version = "1.0.0";

    private const string BalancedMorticianGuid = "com.Bloonjitsu7.BalancedMortician";

    internal static ManualLogSource Log { get; private set; }

    private MorticianSettings _settings;

    private void Awake()
    {
        Log = Logger;
        _settings = new MorticianSettings(Config);

        ShovelDamageSourcePatch.Install(_settings.ShovelCountsAsPrimarySkill);
        GhoulEquipmentPatch.Install(_settings.GhoulsInheritEquipment);
        GhoulLimitPatch.Install(_settings.GhoulLimit);
        TombstoneLifetimePatch.Install(_settings.TombstoneLifetime);

        ApplySettings();
        _settings.PresetSelector.SettingsChanged += ApplySettings;
    }

    private void ApplySettings()
    {
        MorticianTuning.Apply(_settings);
        SkillDescriptions.Apply(_settings);
    }
}
