using CustomSabersLite.Models;
using UnityEngine;

namespace CustomSabersLite.Menu;

internal class BasicPreviewSaber
{
    private readonly Transform root = new GameObject("BasicPreviewSaber").transform;
    private ILiteSaber? saber;
    
    public void SetParent(Transform parent) => root.SetParent(parent, false);
    public void ReplaceSaber(ILiteSaber? newSaber)
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