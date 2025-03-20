using CustomSabersLite.Components;
using CustomSabersLite.Services;
using UnityEngine;
using Zenject;
using static UnityEngine.Object;
using static UnityEngine.Random;

namespace CustomSabersLite.Misc;

internal class Jester : IInitializable
{
    private GameObject SaberModel { get; }
    
    public Jester(GameResourcesProvider gameResourcesProvider)
    {
        SaberModel = gameResourcesProvider.CreateNewDefaultSaber();
        SaberModel.name = "<color=yellow>Very funny saber</color>";
    }

    public void Initialize()
    {
        for (int i = 0; i < 100; i++)
        {
            var saber = i == 0 ? SaberModel : Instantiate(SaberModel);
            saber.transform.position = new(Range(-20f, 20f), Range(0.4f, 10f), Range(-20f, 20f));
            saber.transform.rotation = rotation;
            saber.AddComponent<DefaultSaberColorer>().SetColor(new(Range(0f, 1f), Range(0f, 1f), Range(0f, 1f)));
            saber.SetActive(true);
        }
    }
}
