using CustomSabersLite.Components;
using CustomSabersLite.Utilities;
using UnityEngine;
using Zenject;

using static UnityEngine.Object;
using static UnityEngine.Random;

namespace CustomSabersLite;

internal class Jester : IInitializable
{
    private GameObject SaberModel { get; }
    
    public Jester(GameResourcesProvider gameResourcesProvider)
    {
        SaberModel = Instantiate(gameResourcesProvider.SaberModelPrefab);
        SaberModel.SetActive(false);
        SaberModel.name = "<color=yellow>Very funny saber</color>";
        SaberModel.transform.position = Vector3.zero;
        SaberModel.GetComponentsInChildren<SetSaberGlowColor>().ForEach(x => x.enabled = false);
        SaberModel.GetComponentsInChildren<SetSaberFakeGlowColor>().ForEach(x => x.enabled = false);
        SaberModel.GetComponent<SaberTrail>().enabled = false;
    }

    public void Initialize()
    {
        for (int i = 0; i < 100; i++)
        {
            var saber = Instantiate(SaberModel);
            saber.transform.position = new(Range(-20f, 20f), Range(0.4f, 10f), Range(-20f, 20f));
            saber.transform.rotation = rotation;
            saber.AddComponent<DefaultSaberColorer>().SetColor(new(Range(0f, 1f), Range(0f, 1f), Range(0f, 1f)));
            saber.SetActive(true);
        }
    }
}
