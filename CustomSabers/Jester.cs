using CustomSabersLite.Utilities;
using UnityEngine;
using Zenject;

using static UnityEngine.Object;
using static UnityEngine.Random;

namespace CustomSabersLite;

internal class Jester(InternalResourcesProvider resources) : IInitializable
{
    private GameObject SaberModel { get; } = Instantiate(resources.SaberModelPrefab);

    public void Initialize()
    {
        SaberModel.name = "<color=yellow>Very funny saber</color>";
        SaberModel.SetActive(false);
        SaberModel.GetComponentsInChildren<MonoBehaviour>().ForEach(Object.Destroy);

        for (var i = 0; i < 100; i++)
        {
            var position = new Vector3(
                Range(-20f, 20f),
                Range(0.4f, 10f),
                Range(-20f, 20f)
            );
            var color = new Color(
                Range(0f, 1f),
                Range(0f, 1f),
                Range(0f, 1f)
            );
            CreateNewSaber(position, rotation, color);
        }
    }
    private void CreateNewSaber(Vector3 pos, Quaternion rot, Color color)
    {
        var saber = Instantiate(SaberModel);

        foreach (var setSaberGlowColor in saber.GetComponentsInChildren<SetSaberGlowColor>())
        {
            var materialPropertyBlock = setSaberGlowColor._materialPropertyBlock ?? new MaterialPropertyBlock();

            setSaberGlowColor._propertyTintColorPairs.ForEach(pair =>
                materialPropertyBlock.SetColor(pair.property, color * pair.tintColor));

            setSaberGlowColor._meshRenderer.SetPropertyBlock(materialPropertyBlock);
        }

        saber.transform.position = pos;
        saber.transform.rotation = rot;
    }
}
