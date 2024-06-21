using CustomSabersLite.Components.Game;
using CustomSabersLite.Configuration;
using CustomSabersLite.Data;
using CustomSabersLite.Utilities.AssetBundles;
using System.Threading.Tasks;
using UnityEngine;

namespace CustomSabersLite.Components.Managers;

internal class LiteSaberSet(CustomSabersLoader customSabersLoader, CSLConfig config)
{
    private readonly CustomSabersLoader customSabersLoader = customSabersLoader;
    private readonly CSLConfig config = config;

    private GameObject leftSaberPrefab = null;
    private GameObject rightSaberPrefab = null;

    public CustomSaberData Data { get; private set; }

    public LiteSaber NewSaberForSaberType(SaberType saberType)
    {
        var original = saberType == SaberType.SaberA ? leftSaberPrefab : rightSaberPrefab;
        return original ? GameObject.Instantiate(original).AddComponent<LiteSaber>() : null;
    }

    public async Task SetSabers(string saberPath = null)
    {
        var saberData = await GetSaberData(saberPath ?? config.CurrentlySelectedSaber);
        if (saberData.SaberPrefab != null)
        {
            leftSaberPrefab = saberData.SaberPrefab.transform.Find("LeftSaber")?.gameObject;
            rightSaberPrefab = saberData.SaberPrefab.transform.Find("RightSaber")?.gameObject;
        }
    }

    public async Task<CustomSaberData> GetSaberData(string saberPath) =>
        Data = await customSabersLoader.GetSaberData(saberPath);
}
