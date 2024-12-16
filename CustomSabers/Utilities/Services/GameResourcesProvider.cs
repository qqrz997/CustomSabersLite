using System;
using System.Linq;
using BGLib.UnityExtension;
using CustomSabersLite.Utilities.Extensions;
using UnityEngine;
using Zenject;
using Object = UnityEngine.Object;

namespace CustomSabersLite.Utilities.Services;


internal class GameResourcesProvider : IInitializable
{
    private readonly SaberTrailRenderer saberTrailRenderer;
    private readonly GameObject saberModelPrefab;

    private GameResourcesProvider()
    {
        saberTrailRenderer = LoadAsset<GameObject>("Assets/Prefabs/Effects/Sabers/SaberTrailRenderer.prefab").GetComponent<SaberTrailRenderer>();
        saberModelPrefab = LoadAsset<GameObject>("Assets/Prefabs/Sabers/BasicSaberModel.prefab");
    }

    public Material DefaultTrailMaterial => saberTrailRenderer._meshRenderer.material;
    
    public SaberTrailRenderer CreateNewSaberTrailRenderer() => 
        Object.Instantiate(saberTrailRenderer, Vector3.zero, Quaternion.identity);

    public GameObject CreateNewDefaultSaber()
    {
        var saberObject = Object.Instantiate(saberModelPrefab, Vector3.zero, Quaternion.identity);
        saberObject.GetComponentsInChildren<SetSaberGlowColor>().ForEach(x => x.enabled = false);
        saberObject.GetComponentsInChildren<SetSaberFakeGlowColor>().ForEach(x => x.enabled = false);
        saberObject.GetComponent<SaberTrail>().enabled = false;
        saberObject.SetActive(true);
        return saberObject;
    }

    public void Initialize()
    {
        saberTrailRenderer._meshRenderer = saberTrailRenderer.GetComponent<MeshRenderer>();
        saberTrailRenderer._meshFilter = saberTrailRenderer.GetComponent<MeshFilter>();
    }

    private static T LoadAsset<T>(object label) where T : Object => 
        AddressablesExtensions.LoadContent<T>(label).FirstOrDefault() 
        ?? throw new InvalidOperationException("An internal resource failed to load");
}
