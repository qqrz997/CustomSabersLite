using UnityEngine;

namespace CustomSabersLite.Data;

internal readonly struct SaberPrefab
{
    public GameObject Prefab { get; }
    
    public SaberPrefab(GameObject prefab, SaberType saberType)
    {
        if (!prefab) return;
        var name = saberType == SaberType.SaberA ? "LeftSaber" : "RightSaber";
        Prefab = prefab.transform.Find(name).gameObject;
    }
}
