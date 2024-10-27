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
        try
        {
            // just in case transform.Find() fails, catch the NullReferenceException
            return prefab == null ? null : new SaberPrefab(prefab);
        }
        catch (NullReferenceException)
        {
            return null;
        }
    }

    public void Dispose()
    {
        if (Prefab) Prefab.Destroy();
    }
}
