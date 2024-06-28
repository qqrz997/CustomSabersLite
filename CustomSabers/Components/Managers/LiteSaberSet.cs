using CustomSabersLite.Components.Game;
using CustomSabersLite.Data;
using CustomSabersLite.Utilities.AssetBundles;
using System.Threading.Tasks;
using UnityEngine;

namespace CustomSabersLite.Components.Managers;

internal class LiteSaberSet(CustomSabersLoader customSabersLoader)
{
    private readonly CustomSabersLoader customSabersLoader = customSabersLoader;

    private CustomSaberData currentSaberData;

    public LiteSaber NewSaberForSaberType(SaberType saberType)
    {
        var original = currentSaberData.GetPrefab(saberType);
        if (!original) return null;
        var newSaber = GameObject.Instantiate(original).AddComponent<LiteSaber>();
        newSaber.Init(currentSaberData.Type);
        return newSaber;
    }

    public async Task SetSabers(string saberPath) =>
        currentSaberData = await customSabersLoader.GetSaberData(saberPath);
}
