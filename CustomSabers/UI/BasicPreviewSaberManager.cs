using CustomSabersLite.Models;
using UnityEngine;

namespace CustomSabersLite.UI;

internal class BasicPreviewSaberManager
{
    private readonly Transform left = new GameObject("Basic Preview Saber").transform;
    private readonly Transform right = new GameObject("Basic Preview Saber").transform;

    private ILiteSaber? leftSaber;
    private ILiteSaber? rightSaber;

    public void Init(Transform leftParent, Transform rightParent)
    {
        left.SetParent(leftParent, false);
        right.SetParent(rightParent, false);
    }

    public void ReplaceSabers(ILiteSaber? newLeftSaber, ILiteSaber? newRightSaber)
    {
        leftSaber?.Destroy();
        rightSaber?.Destroy();

        leftSaber = newLeftSaber;
        rightSaber = newRightSaber;

        leftSaber?.SetParent(left);
        rightSaber?.SetParent(right);
    }

    public void SetColor(Color left, Color right)
    {
        leftSaber?.SetColor(left);
        rightSaber?.SetColor(right);
    }

    public void SetActive(bool active)
    {
        leftSaber?.GameObject.SetActive(active);
        rightSaber?.GameObject.SetActive(active);
    }
}
