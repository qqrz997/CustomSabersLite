using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

namespace CustomSabersLite.Utilities;
internal class MenuPointerProvider : IInitializable
{
    public GameObject LeftPointer { get; private set; }

    public GameObject RightPointer { get; private set; }

    private readonly List<MeshRenderer> meshRenderers = [];

    public void Initialize()
    {
        var controllers = Resources.FindObjectsOfTypeAll<VRController>();
        LeftPointer = controllers.First(c => c.transform.name == "ControllerLeft").transform.Find("MenuHandle").gameObject;
        RightPointer = controllers.First(c => c.transform.name == "ControllerRight").transform.Find("MenuHandle").gameObject;

        meshRenderers.AddRange(GetMenuHandleRenderers(LeftPointer));
        meshRenderers.AddRange(GetMenuHandleRenderers(RightPointer));
    }

    public void SetPointerVisibility(bool visible)
    {
        foreach (var renderer in meshRenderers) renderer.enabled = visible;
    }

    private List<MeshRenderer> GetMenuHandleRenderers(GameObject menuHandle) => [
        menuHandle.transform.Find("Glowing").GetComponent<MeshRenderer>(), 
        menuHandle.transform.Find("Normal").GetComponent<MeshRenderer>(), 
        menuHandle.transform.Find("FakeGlow0").GetComponent<MeshRenderer>(),
        menuHandle.transform.Find("FakeGlow1").GetComponent<MeshRenderer>()
    ];
}
