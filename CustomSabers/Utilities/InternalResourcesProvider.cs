using BGLib.UnityExtension;
using System.Linq;
using UnityEngine;
using Zenject;

namespace CustomSabersLite.Utilities;


internal class InternalResourcesProvider : IInitializable
{
    private readonly Transform resourcesParent;

    private InternalResourcesProvider() =>
        resourcesParent = new GameObject("CustomSabersLite InternalResources").transform;

    public SaberTrailRenderer SaberTrailRenderer { get; private set; }

    public void Initialize()
    {
        var saberTrailRendererPrefab = AddressablesExtensions.LoadContent<GameObject>("Assets/Prefabs/Effects/Sabers/SaberTrailRenderer.prefab").First();
        saberTrailRendererPrefab.transform.SetParent(resourcesParent);
        var saberTrailRendererComponent = saberTrailRendererPrefab.GetComponent<SaberTrailRenderer>();
        saberTrailRendererComponent._meshRenderer = saberTrailRendererPrefab.GetComponent<MeshRenderer>();
        saberTrailRendererComponent._meshFilter = saberTrailRendererPrefab.GetComponent<MeshFilter>();
        SaberTrailRenderer = saberTrailRendererComponent;
    }
}
