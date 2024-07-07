using System.Linq;
using UnityEngine;
using Zenject;

namespace CustomSabersLite.Utilities;
internal class MenuPointerProvider : IInitializable
{
    public GameObject LeftPointer { get; private set; }

    public GameObject RightPointer { get; private set; }

    public void Initialize()
    {
        var controllers = Resources.FindObjectsOfTypeAll<VRController>();
        LeftPointer = controllers.First(c => c.transform.name == "ControllerLeft").transform.Find("MenuHandle").gameObject;
        RightPointer = controllers.First(c => c.transform.name == "ControllerRight").transform.Find("MenuHandle").gameObject;
    }
}
