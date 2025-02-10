using System;
using System.Linq;
using CustomSabersLite.Utilities.Extensions;
using UnityEngine;
using Zenject;
using static BGLib.UnityExtension.AddressablesExtensions;
using static UnityEngine.Object;
using Object = UnityEngine.Object;

namespace CustomSabersLite.Utilities.Services;

internal class GameResourcesProvider : IInitializable
{
    private readonly SaberTrailRenderer trailRendererPrefab;
    private readonly GameObject saberModelPrefab;

    private GameResourcesProvider()
    {
        trailRendererPrefab = LoadAsset<SaberTrailRenderer>("Assets/Prefabs/Effects/Sabers/SaberTrailRenderer.prefab");
        saberModelPrefab = LoadPrefab("Assets/Prefabs/Sabers/BasicSaberModel.prefab");
    }

    public Material DefaultTrailMaterial => trailRendererPrefab._meshRenderer.material;
    
    public SaberTrailRenderer CreateNewSaberTrailRenderer() => 
        Instantiate(trailRendererPrefab, Vector3.zero, Quaternion.identity);

    public GameObject CreateNewDefaultSaber()
    {
        var saberObject = Instantiate(saberModelPrefab, Vector3.zero, Quaternion.identity);
        saberObject.name = "NewSaberModel";
        saberObject.GetComponentsInChildren<SetSaberGlowColor>().ForEach(x => x.enabled = false);
        saberObject.GetComponentsInChildren<SetSaberFakeGlowColor>().ForEach(x => x.enabled = false);
        saberObject.GetComponent<SaberTrail>().enabled = false;
        return saberObject;
    }

    public void Initialize()
    {
        trailRendererPrefab._meshRenderer = trailRendererPrefab.GetComponent<MeshRenderer>();
        trailRendererPrefab._meshFilter = trailRendererPrefab.GetComponent<MeshFilter>();
    }

    private static Exception ResourceException => new InvalidOperationException("An internal resource failed to load");
    
    private static GameObject LoadPrefab(object label) =>
        LoadContent<GameObject>(label).FirstOrDefault() ?? throw ResourceException;
    private static T LoadAsset<T>(object label) where T : Object => 
        LoadContent<GameObject>(label).FirstOrDefault()?.GetComponent<T>() ?? throw ResourceException;
}
