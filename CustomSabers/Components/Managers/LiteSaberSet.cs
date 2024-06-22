using CustomSabersLite.Components.Game;
using CustomSabersLite.Configuration;
using CustomSabersLite.Data;
using CustomSabersLite.Utilities.AssetBundles;
using System.Threading.Tasks;
using UnityEngine;

namespace CustomSabersLite.Components.Managers;

internal class LiteSaberSet(CustomSabersLoader customSabersLoader)
{
    private readonly CustomSabersLoader customSabersLoader = customSabersLoader;

    private GameObject leftSaberPrefab = null;
    private GameObject rightSaberPrefab = null;

    public CustomSaberData Data { get; private set; }

    public LiteSaber NewSaberForSaberType(SaberType saberType)
    {
        var original = saberType == SaberType.SaberA ? leftSaberPrefab : rightSaberPrefab;
        return original ? GameObject.Instantiate(original).AddComponent<LiteSaber>() : null;
    }

    public async Task SetSabers(string saberPath)
    {
        var saberData = await GetSaberData(saberPath);
        leftSaberPrefab = saberData.LeftSaberPrefab;
        rightSaberPrefab = saberData.RightSaberPrefab;
    }

    public async Task<CustomSaberData> GetSaberData(string saberPath) =>
        Data = await customSabersLoader.GetSaberData(saberPath);
}
