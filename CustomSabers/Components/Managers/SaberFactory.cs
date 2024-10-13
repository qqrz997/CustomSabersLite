using CustomSabersLite.Components.Game;
using CustomSabersLite.Configuration;
using CustomSabersLite.Models;
using CustomSabersLite.Utilities;
using System.Threading.Tasks;
using UnityEngine;

namespace CustomSabersLite.Components.Managers;

internal class SaberFactory(CustomSabersLoader customSabersLoader, CSLConfig config)
{
    private readonly CustomSabersLoader customSabersLoader = customSabersLoader;
    private readonly CSLConfig config = config;

    public async Task<ISaberData> GetCurrentSaberDataAsync() =>
        config.CurrentlySelectedSaber is null ? new NoSaberData()
        : await customSabersLoader.GetSaberData(config.CurrentlySelectedSaber, true);

    public LiteSaber? TryCreate(SaberType saberType, ISaberData saberData)
    {
        var original = saberData.GetPrefab(saberType);

        if (original == null)
            return null;

        var newSaber = GameObject.Instantiate(original).AddComponent<LiteSaber>();
        newSaber.Init(saberData.Metadata.FileInfo.Type);

        return newSaber;
    }
}
