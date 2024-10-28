using CustomSabersLite.Utilities;
using UnityEngine;

namespace CustomSabersLite.Models;

internal abstract class SaberPrefab
{
    public abstract GameObject Prefab { get; }

    public abstract GameObject? Left { get; }
    public abstract GameObject? Right { get; }

    public static SaberPrefab? TryCreate(GameObject prefab)
    {
        var left = prefab.transform.Find("LeftSaber").gameObject;
        var right = prefab.transform.Find("RightSaber").gameObject;
        
        if (left != null && right != null)
        {
            return new CustomSaberPrefab(prefab, left, right);
        }

        return prefab.GetComponent<SaberModelController>() switch
        {
            SaberModelController => new DefaultSaberPrefab(prefab),
            _ => null
        };
    }

    public virtual void Dispose()
    {
        if (Prefab) Prefab.Destroy();
    }
}

internal class CustomSaberPrefab(GameObject parentPrefab, GameObject left, GameObject right) : SaberPrefab
{
    public override GameObject Prefab { get; } = parentPrefab;
    public override GameObject? Left { get; } = left;
    public override GameObject? Right { get; } = right;
}

internal class DefaultSaberPrefab(GameObject defaultSaberPrefab) : SaberPrefab
{
    public override GameObject Prefab { get; } = defaultSaberPrefab;

    public override GameObject? Left => null;
    public override GameObject? Right => null;
}
