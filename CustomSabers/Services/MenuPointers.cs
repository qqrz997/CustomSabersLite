using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CustomSabersLite.Services;

internal class MenuPointers
{
    private GameObject LeftPointer { get; }
    private GameObject RightPointer { get; }
    private List<MeshRenderer> MeshRenderers { get; } = [];

    private MenuPointers()
    {
        var controllers = Resources.FindObjectsOfTypeAll<VRController>();
        LeftPointer = controllers.First(c => c.transform.name == "ControllerLeft").transform.Find("MenuHandle").gameObject;
        RightPointer = controllers.First(c => c.transform.name == "ControllerRight").transform.Find("MenuHandle").gameObject;

        MeshRenderers.AddRange(GetMenuHandleRenderers(LeftPointer));
        MeshRenderers.AddRange(GetMenuHandleRenderers(RightPointer));
    }
    
    public (Transform leftParent, Transform rightParent) Parents => (LeftPointer.transform, RightPointer.transform);

    public void SetPointerVisibility(bool visible) =>
        MeshRenderers.ForEach(r => r.enabled = visible);

    private static List<MeshRenderer> GetMenuHandleRenderers(GameObject menuHandle) => [
        menuHandle.transform.Find("Glowing").GetComponent<MeshRenderer>(),
        menuHandle.transform.Find("Normal").GetComponent<MeshRenderer>(),
        menuHandle.transform.Find("FakeGlow0").GetComponent<MeshRenderer>(),
        menuHandle.transform.Find("FakeGlow1").GetComponent<MeshRenderer>()
    ];
}
