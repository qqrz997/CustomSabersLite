using BGLib.UnityExtension;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

namespace CustomSabersLite.Utilities;


internal class InternalResourcesProvider : IInitializable, IDisposable
{
    public SaberTrailRenderer SaberTrailRenderer { get; private set; }

    private readonly List<UnityEngine.Object> loadedObjects = [];

    public void Initialize()
    {
        var saberTrailRendererPrefab = TryLoadAsset<GameObject>("Assets/Prefabs/Effects/Sabers/SaberTrailRenderer.prefab");
        var saberTrailRendererComponent = saberTrailRendererPrefab.GetComponent<SaberTrailRenderer>();
        saberTrailRendererComponent._meshRenderer = saberTrailRendererPrefab.GetComponent<MeshRenderer>();
        saberTrailRendererComponent._meshFilter = saberTrailRendererPrefab.GetComponent<MeshFilter>();
        SaberTrailRenderer = saberTrailRendererComponent;
    }

    public void Dispose()
    {
        foreach (var obj in loadedObjects)
        {
            UnityEngine.Object.Destroy(obj);
        }
    }

    private T TryLoadAsset<T>(object label) where T : UnityEngine.Object
    {
        var asset = AddressablesExtensions.LoadContent<T>(label).FirstOrDefault();
        if (asset == null)
        {
            Logger.Error($"Couldn't load internal asset\n\t-{label}");
            return null;
        }
        loadedObjects.Add(asset);
        return asset;
    }
}
