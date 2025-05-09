using UnityEngine;

namespace CustomSabersLite.Utilities.Extensions;

internal static class GameObjectExtensions
{
    public static T TryGetComponentOrAdd<T>(this GameObject obj) where T : MonoBehaviour =>
        obj.GetComponent<T>() ?? obj.AddComponent<T>();

    public static void Destroy(this GameObject? obj)
    {
        if (obj != null) Object.Destroy(obj);
    }

    public static void Destroy(this Transform? t)
    {
        if (t != null && t.gameObject != null) Object.Destroy(t.gameObject);
    }

    public static void DestroyImmediate(this GameObject? obj)
    {
        if (obj != null && obj) Object.DestroyImmediate(obj);
    }

    public static void DestroyImmediate(this Transform? t)
    {
        if (t != null && t.gameObject != null) Object.DestroyImmediate(t.gameObject);
    }

    public static void DestroyComponent(this Component? component)
    {
        if (component != null) Object.Destroy(component);
    }

    public static void DestroyComponent<T>(this GameObject? gameObject) where T : Component
    {
        if (gameObject != null) gameObject.GetComponent<T>().DestroyComponent();
    }

    public static void SetLayerRecursively(this GameObject obj, int layer) => SetLayer(obj, layer);
    private static void SetLayer(GameObject gameObject, int layer)
    {
        gameObject.layer = layer;
        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            SetLayer(gameObject.transform.GetChild(i).gameObject, layer);
        }
    }
}
