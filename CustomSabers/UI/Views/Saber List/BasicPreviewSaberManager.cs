using CustomSabersLite.Components.Game;
using CustomSabersLite.Utilities.Extensions;
using UnityEngine;

namespace CustomSabersLite.UI.Managers;

internal class BasicPreviewSaberManager
{
    private readonly Transform left = new GameObject("Basic Preview Saber").transform;
    private readonly Transform right = new GameObject("Basic Preview Saber").transform;

    private LiteSaber leftSaber;
    private LiteSaber rightSaber;

    public void Init(Transform leftParent, Transform rightParent)
    {
        left.SetParent(leftParent, false);
        right.SetParent(rightParent, false);
    }

    public void ReplaceSabers(LiteSaber newLeftSaber, LiteSaber newRightSaber)
    {
        leftSaber?.gameObject.DestroyImmediate();
        rightSaber?.gameObject.DestroyImmediate();

        leftSaber = newLeftSaber;
        rightSaber = newRightSaber;

        if (leftSaber)
        {
            leftSaber.SetParent(left);
        }

        if (rightSaber)
        {
            rightSaber.SetParent(right);
        }
    }

    public void SetColor(Color left, Color right)
    {
        leftSaber?.SetColor(left);
        rightSaber?.SetColor(right);
    }

    public void SetActive(bool active)
    {
        leftSaber?.gameObject.SetActive(active);
        rightSaber?.gameObject.SetActive(active);
    }
}
