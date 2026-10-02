using BepInEx.Configuration;
using EvenMoreBalancedMortician.Presets;
using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace EvenMoreBalancedMortician.Scepter;

internal sealed class RaiseRadiusIndicator : MonoBehaviour
{
    private const string IndicatorPrefabKey = "RoR2/Base/Common/TeamAreaIndicator, GroundOnly.prefab";

    private static GameObject _indicatorPrefab;
    private static PresetSetting<float> _radius;
    private static ConfigEntry<bool> _isShown;

    private CharacterBody _tombstone;
    private GameObject _indicator;

    public static void Configure(PresetSetting<float> radius, ConfigEntry<bool> isShown)
    {
        _radius = radius;
        _isShown = isShown;
        _indicatorPrefab = Addressables.LoadAssetAsync<GameObject>(IndicatorPrefabKey).WaitForCompletion();

        if (!_indicatorPrefab)
            EvenMoreBalancedMorticianPlugin.Log.LogError("Area indicator prefab not found; the Restless Grave radius will not be shown.");
    }

    private void Start()
    {
        _tombstone = GetComponent<CharacterBody>();
        if (!_indicatorPrefab)
            return;

        _indicator = Instantiate(_indicatorPrefab, transform);
        _indicator.GetComponentInChildren<TeamAreaIndicator>().teamComponent = GetComponent<TeamComponent>();
        _indicator.SetActive(false);
    }

    private void Update()
    {
        if (!_indicator)
            return;

        var isVisible = _isShown.Value && _radius.Value > 0f && RestlessGraveSkill.IsEquippedBy(TombstoneOwner.BodyOf(_tombstone));
        if (_indicator.activeSelf != isVisible)
            _indicator.SetActive(isVisible);

        if (isVisible)
            _indicator.transform.localScale = Vector3.one * (_radius.Value / transform.lossyScale.x);
    }
}
