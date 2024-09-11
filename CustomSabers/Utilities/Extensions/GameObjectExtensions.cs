using UnityEngine;

namespace CustomSabersLite.Utilities.Extensions;

internal static class GameObjectExtensions
{
    public static T TryGetComponentOrDefault<T>(this GameObject obj) where T : MonoBehaviour =>
        obj.GetComponent<T>() ?? obj.AddComponent<T>();

    public static void Destroy(this GameObject obj)
    {
        if (obj) Object.Destroy(obj);
    }

    public static void DestroyImmediate(this GameObject obj)
    {
        if (obj) Object.DestroyImmediate(obj);
    }
}
