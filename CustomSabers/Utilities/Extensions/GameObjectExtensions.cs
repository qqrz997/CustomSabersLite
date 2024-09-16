using UnityEngine;

namespace CustomSabersLite.Utilities.Extensions;

internal static class GameObjectExtensions
{
    public static T? Maybe<T>(this T? obj) where T : Object =>
        obj is not null && obj ? obj! : null;

    public static T TryGetComponentOrAdd<T>(this GameObject obj) where T : MonoBehaviour =>
        obj.GetComponent<T>() ?? obj.AddComponent<T>();

    public static void Destroy(this GameObject? obj)
    {
        if (obj != null && obj) Object.Destroy(obj);
    }

    public static void DestroyImmediate(this GameObject? obj)
    {
        if (obj != null && obj) Object.DestroyImmediate(obj);
    }
}
