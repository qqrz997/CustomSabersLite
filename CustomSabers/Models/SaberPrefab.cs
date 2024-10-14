using CustomSabersLite.Utilities;
using System;
using UnityEngine;

namespace CustomSabersLite.Models;

internal class SaberPrefab : IDisposable
{
    public GameObject Prefab { get; }

    public GameObject Left { get; }
    public GameObject Right { get; }

    private SaberPrefab(GameObject parentPrefab)
    {
        Prefab = parentPrefab;
        Left = parentPrefab.transform.Find("LeftSaber").gameObject;
        Right = parentPrefab.transform.Find("RightSaber").gameObject;
    }

    public static SaberPrefab? TryCreate(GameObject prefab)
    {
        if (prefab == null)
        {
            return null;
        }

        try
        {
            return new SaberPrefab(prefab);
        }
        catch
        {
            return null;
        }
    }

    public void Dispose()
    {
        if (Prefab) Prefab.Destroy();
    }
}
