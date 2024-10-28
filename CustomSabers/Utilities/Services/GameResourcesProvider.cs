using BGLib.UnityExtension;
using System;
using System.Linq;
using UnityEngine;
using Zenject;

namespace CustomSabersLite.Utilities;


internal class GameResourcesProvider : IInitializable
{
    public SaberTrailRenderer SaberTrailRenderer { get; private set; }

    public GameObject SaberModelPrefab { get; private set; }

    private GameResourcesProvider()
    {
        SaberTrailRenderer = LoadAsset<GameObject>("Assets/Prefabs/Effects/Sabers/SaberTrailRenderer.prefab").GetComponent<SaberTrailRenderer>();
        SaberModelPrefab = LoadAsset<GameObject>("Assets/Prefabs/Sabers/BasicSaberModel.prefab");
    }

    public void Initialize()
    {
        SaberTrailRenderer._meshRenderer = SaberTrailRenderer.GetComponent<MeshRenderer>();
        SaberTrailRenderer._meshFilter = SaberTrailRenderer.GetComponent<MeshFilter>();

        SaberModelPrefab.transform.position = Vector3.zero;
        SaberModelPrefab.GetComponentsInChildren<SetSaberGlowColor>().ForEach(x => x.enabled = false);
        SaberModelPrefab.GetComponentsInChildren<SetSaberFakeGlowColor>().ForEach(x => x.enabled = false);
        SaberModelPrefab.GetComponent<SaberTrail>().enabled = false;
    }

    private T LoadAsset<T>(object label) where T : UnityEngine.Object => 
        AddressablesExtensions.LoadContent<T>(label).FirstOrDefault() 
        ?? throw new InvalidOperationException("An internal resource failed to load");
}
