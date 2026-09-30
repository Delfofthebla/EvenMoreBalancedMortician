using BepInEx;
using BepInEx.Logging;
using EvenMoreBalancedMortician.Networking;
using EvenMoreBalancedMortician.Patches;
using Morris;
using R2API;
using R2API.Networking;
using R2API.Utils;

namespace EvenMoreBalancedMortician;

[BepInPlugin(Guid, Name, Version)]
[BepInDependency(MorrisPlugin.MODUID)]
[BepInDependency(LanguageAPI.PluginGUID)]
[BepInDependency(DeployableAPI.PluginGUID)]
[BepInDependency(NetworkingAPI.PluginGUID)]
[NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.EveryoneNeedSameModVersion)]
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
        GhoulEquipmentPatch.Install();
        GhoulAspectPatch.Install(_settings.GhoulAspectInheritChance);
        GhoulLimitPatch.Install(_settings.GhoulLimit);
        TombstoneDurationPatch.Install(_settings.TombstoneDuration);

        ApplySettings();
        _settings.PresetSelector.SettingsChanged += ApplySettings;
        HostSettingsSync.Install(_settings, ApplySettings);
    }

    private void ApplySettings()
    {
        MorticianTuning.Apply(_settings);
        SkillDescriptions.Apply(_settings);
    }
}
