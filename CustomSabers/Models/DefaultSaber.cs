using CustomSaber;
using CustomSabersLite.Components;
using UnityEngine;

namespace CustomSabersLite.Models;

internal class DefaultSaber(GameObject basicSaberModel) : ILiteSaber
{
    private readonly DefaultSaberColorer defaultSaberColorer = basicSaberModel.AddComponent<DefaultSaberColorer>();

    public GameObject GameObject { get; } = basicSaberModel;
    public Transform Transform { get; } = basicSaberModel.transform;
    public EventManager? EventManager { get; } = null;
    public CustomTrailData[] TrailData { get; } = [];

    public void SetColor(Color color) => defaultSaberColorer.SetColor(color);
    public void SetParent(Transform parent) => Transform.SetParent(parent, false);
    
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
