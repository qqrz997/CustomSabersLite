using SabersLib.Models;
using UnityEngine;

namespace CustomSabersLite.Menu;

internal class StaticPreviewSaber
{
    private readonly Transform root = new GameObject("StaticPreviewSaber").transform;
    private ISaber? saber;
    
    public void SetParent(Transform parent) => root.SetParent(parent, false);
    public void ReplaceSaber(ISaber? newSaber)
    {
        saber = newSaber;
        saber?.SetParent(root);
    }
    public void SetColor(Color color) => saber?.SetColor(color);
    public void SetScale(float length, float width)
    {
        if (saber is null) return;
        saber.SetLength(length);
        saber.SetWidth(width);
    }
}