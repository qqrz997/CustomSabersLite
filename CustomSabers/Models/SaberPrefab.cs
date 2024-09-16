using UnityEngine;

namespace CustomSabersLite.Models;

internal class SaberPrefab
{
    public GameObject Prefab { get; }

    public SaberPrefab(GameObject prefab, SaberType saberType)
    {
        var name = saberType == SaberType.SaberA ? "LeftSaber" : "RightSaber";
        Prefab = prefab.transform.Find(name).gameObject;
    }
}
