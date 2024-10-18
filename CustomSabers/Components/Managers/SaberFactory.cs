using CustomSabersLite.Components.Game;
using CustomSabersLite.Configuration;
using CustomSabersLite.Models;
using CustomSabersLite.Utilities;
using System.Threading.Tasks;
using UnityEngine;
using static UnityEngine.Object;

namespace CustomSabersLite.Components.Managers;

internal class SaberFactory(CustomSabersLoader customSabersLoader, CSLConfig config)
{
    private readonly CustomSabersLoader customSabersLoader = customSabersLoader;
    private readonly CSLConfig config = config;

    public async Task<ISaberData> GetCurrentSaberDataAsync() =>
        config.CurrentlySelectedSaber is null ? NoSaberData.Value
        : await customSabersLoader.GetSaberData(config.CurrentlySelectedSaber, true);

    public LiteSaber? TryCreate(SaberType saberType, ISaberData saberData) =>
        saberData.Prefab is null ? null
        : Create(
            saberType == SaberType.SaberA ? saberData.Prefab.Left : saberData.Prefab.Right,
            saberData.Metadata.FileInfo.Type);

    private LiteSaber Create(GameObject prefab, CustomSaberType customSaberType)
    {
        var liteSaber = Instantiate(prefab).AddComponent<LiteSaber>();
        liteSaber.Init(customSaberType);

        return liteSaber;
    }
}
