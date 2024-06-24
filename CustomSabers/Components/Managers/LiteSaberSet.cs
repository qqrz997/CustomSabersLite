using CustomSabersLite.Components.Game;
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

    private CustomSaberData currentSaberData;

    public LiteSaber NewSaberForSaberType(SaberType saberType)
    {
        var original = saberType == SaberType.SaberA ? leftSaberPrefab : rightSaberPrefab;
        return original ? GameObject.Instantiate(original).AddComponent<LiteSaber>() : null;
    }

    public async Task SetSabers(string saberPath)
    {
        currentSaberData = await customSabersLoader.GetSaberData(saberPath);
        leftSaberPrefab = currentSaberData.Left.Prefab;
        rightSaberPrefab = currentSaberData.Right.Prefab;
    }

    public CustomSaberType GetCurrentCustomSaberType() =>
        currentSaberData?.Type ?? CustomSaberType.Default;
}
