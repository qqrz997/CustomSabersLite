using CustomSaber;
using UnityEngine;

namespace CustomSabersLite.Models;

internal interface ILiteSaber
{
    public GameObject GameObject { get; }
    public EventManager? EventManager { get; }

    public void SetColor(Color color);
    public void SetParent(Transform parent);
    public void SetLength(float length);
    public void SetWidth(float width);
    public void Destroy();
}
