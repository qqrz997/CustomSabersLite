using CustomSabersLite.Components.Game;
using CustomSabersLite.Configuration;
using CustomSabersLite.Data;
using CustomSabersLite.Utilities.AssetBundles;
using System.Threading.Tasks;
using UnityEngine;

namespace CustomSabersLite.Components.Managers;

internal class SaberFactory(CustomSabersLoader customSabersLoader, CSLConfig config)
{
    private readonly CustomSabersLoader customSabersLoader = customSabersLoader;
    private readonly CSLConfig config = config;

    public async Task<CustomSaberData> GetCurrentSaberDataAsync() =>
        await customSabersLoader.GetSaberData(config.CurrentlySelectedSaber);

    public LiteSaber TryCreate(SaberType saberType, CustomSaberData saberData)
    {
        if (saberData is null || saberData.FilePath is null)
        {
            return null;
        }

        var original = saberData.GetPrefab(saberType);
        if (!original)
        {
            return null;
        }

        var newSaber = GameObject.Instantiate(original).AddComponent<LiteSaber>();
        newSaber.Init(saberData.Type);
        return newSaber;
    }
}
