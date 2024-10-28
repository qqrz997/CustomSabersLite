using CustomSabersLite.Utilities;
using CustomSabersLite.Utilities.Extensions;
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

        saber.GetComponentsInChildren<SetSaberGlowColor>().ForEach(x => x.SetNewColor(color));
        saber.GetComponentsInChildren<SetSaberFakeGlowColor>().ForEach(x => x.SetNewColor(color));
        saber.transform.position = pos;
        saber.transform.rotation = rot;
    }
}
