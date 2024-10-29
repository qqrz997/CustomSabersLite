using CustomSaber;
using UnityEngine;

namespace CustomSabersLite.Models;

internal interface ILiteSaber
{
    public GameObject GameObject { get; }
    public Transform Transform { get; }

    public EventManager? EventManager { get; }
    public CustomTrailData[] TrailData { get; }

    public void SetColor(Color color);
    public void SetParent(Transform parent);
    public void Destroy();
}
