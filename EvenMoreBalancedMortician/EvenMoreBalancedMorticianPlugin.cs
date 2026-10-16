using BepInEx;
using BepInEx.Logging;
using EvenMoreBalancedMortician.Networking;
using EvenMoreBalancedMortician.Patches;
using EvenMoreBalancedMortician.Scepter;
using Morris;
using R2API;
using R2API.ContentManagement;
using R2API.Networking;
using R2API.Utils;

namespace EvenMoreBalancedMortician;

[BepInPlugin(GUID, NAME, VERSION)]
[BepInDependency(MorrisPlugin.MODUID)]
[BepInDependency(AncientScepterCompat.GUID, BepInDependency.DependencyFlags.SoftDependency)]
[BepInDependency(R2APIContentManager.PluginGUID, BepInDependency.DependencyFlags.SoftDependency)]
[BepInDependency(LanguageAPI.PluginGUID)]
[BepInDependency(DeployableAPI.PluginGUID)]
[BepInDependency(NetworkingAPI.PluginGUID)]
[BepInDependency(RiskOfOptionsMenu.GUID, BepInDependency.DependencyFlags.SoftDependency)]
[NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.EveryoneNeedSameModVersion)]
[BepInIncompatibility(BALANCED_MORTICIAN_GUID)]
public class EvenMoreBalancedMorticianPlugin : BaseUnityPlugin
{
    public const string GUID = "com.Delfofthebla.EvenMoreBalancedMortician";
    public const string NAME = "EvenMoreBalancedMortician";
    public const string VERSION = "1.5.0";

    private const string BALANCED_MORTICIAN_GUID = "com.Bloonjitsu7.BalancedMortician";

    internal static ManualLogSource Log { get; private set; }

    private MorticianSettings _settings;

    private void Awake()
    {
        Log = Logger;
        _settings = new MorticianSettings(Config);

        DyingGhoul.Install();
        ShovelDamageSourcePatch.Install(_settings.ShovelCountsAsPrimarySkill);
        GhoulEquipmentPatch.Install();
        GhoulAspectPatch.Install(_settings.GhoulAspectInheritChance);
        GhoulOnKillPatch.Install(_settings.GhoulOnKillTriggerChance, _settings.SacrificeGuaranteesOnKill);
        ProcCoefficientPatch.Install(_settings);
        GhoulLimitPatch.Install(_settings.GhoulLimit);
        DetonationDamagePatch.Install(_settings.DetonationScalesWithMortician);
        TombstoneDurationPatch.Install(_settings.TombstoneDuration);
        TombstoneLimitPatch.Install(_settings.LysateCellAddsTombstone);
        TombstoneSoulPatch.Install(_settings.TombstoneSoulRecipient);
        GhoulRaiseCooldownPatch.Install(_settings.GhoulRaisingUsesCooldownReduction);
        InstallAncientScepterSupport();

        ApplySettings();
        _settings.PresetSelector.SettingsChanged += ApplySettings;
        HostSettingsSync.Install(_settings, ApplySettings);

        RiskOfOptionsMenu.Register(this, Log, entry => entry == _settings.ConfigVersion);
    }

    private void InstallAncientScepterSupport()
    {
        if (!AncientScepterCompat.IsInstalled)
            return;

        RestlessGraveSkill.Register(_settings.RestlessGraveEnabled);
        RestlessGrave.Install(_settings.RestlessGraveRadius, _settings.RestlessGraveRisenGhoulLimit, _settings.ShowRaiseRadius);
    }

    private void ApplySettings()
    {
        MorticianTuning.Apply(_settings);
        SkillDescriptions.Apply(_settings);
        RestlessGraveSkill.ApplyEnabledSetting();
    }
}
