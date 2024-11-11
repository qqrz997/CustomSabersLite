using CustomSabersLite.Components;
using CustomSabersLite.Utilities;
using UnityEngine;
using Zenject;

using static UnityEngine.Object;
using static UnityEngine.Random;

namespace CustomSabersLite;

internal class Jester(GameResourcesProvider gameResourcesProvider) : IInitializable
{
    private GameObject SaberModel { get; } = Instantiate(gameResourcesProvider.SaberModelPrefab);

    public void Initialize()
    {
        SaberModel.name = "<color=yellow>Very funny saber</color>";
        SaberModel.SetActive(false);

        for (int i = 0; i < 100; i++)
        {
            var saber = Instantiate(SaberModel);
            saber.SetActive(true);
            var colorer = saber.AddComponent<DefaultSaberColorer>();

            saber.transform.position = new Vector3(
                Range(-20f, 20f),
                Range(0.4f, 10f),
                Range(-20f, 20f)
            );
            saber.transform.rotation = rotation;
            colorer.SetColor(new Color(
                Range(0f, 1f),
                Range(0f, 1f),
                Range(0f, 1f)
            ));
        }
    }
}
