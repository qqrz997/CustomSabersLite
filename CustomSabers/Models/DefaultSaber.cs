using CustomSaber;
using CustomSabersLite.Components;
using UnityEngine;

namespace CustomSabersLite.Models;

internal class DefaultSaber : ILiteSaber
{
    private readonly DefaultSaberColorer defaultSaberColorer;

    public bool InUse { get; set; }
    public GameObject GameObject { get; }
    public EventManager? EventManager => null;
    
    public DefaultSaber(GameObject defaultSaberObject)
    {
        GameObject = defaultSaberObject;
        defaultSaberColorer = GameObject.AddComponent<DefaultSaberColorer>();
    }

    public void SetColor(Color color) => defaultSaberColorer.SetColor(color);
    public void SetParent(Transform parent)
    {
        GameObject.transform.SetParent(parent, false);
        GameObject.transform.position = parent.position;
        GameObject.transform.rotation = parent.rotation;
    }

    public void SetLength(float length) =>
        GameObject.transform.localScale = GameObject.transform.localScale with { z = length };

    public void SetWidth(float width) =>
        GameObject.transform.localScale = GameObject.transform.localScale with { x = width, y = width };

    public void Destroy()
    {
        if (GameObject != null)
        {
            Object.Destroy(GameObject);
        }
    }
}
