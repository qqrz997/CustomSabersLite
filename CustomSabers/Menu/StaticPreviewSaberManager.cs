using CustomSabersLite.Models;
using UnityEngine;

namespace CustomSabersLite.Menu;

internal class StaticPreviewSaberManager
{
    private readonly StaticPreviewSaber leftSaber = new();
    private readonly StaticPreviewSaber rightSaber = new();

    public void Init(Transform leftParent, Transform rightParent)
    {
        leftSaber.SetParent(leftParent);
        rightSaber.SetParent(rightParent);
    }

    public void ReplaceSabers(SaberInstanceSet saberSet)
    {
        leftSaber.ReplaceSaber(saberSet.LeftSaber);
        rightSaber.ReplaceSaber(saberSet.RightSaber);
    }

    public void SetColor(Color left, Color right)
    {
        leftSaber.SetColor(left);
        rightSaber.SetColor(right);
    }

    public void UpdateSaberScale(float length, float width)
    {
        leftSaber.SetScale(length, width);
        rightSaber.SetScale(length, width);
    }
}