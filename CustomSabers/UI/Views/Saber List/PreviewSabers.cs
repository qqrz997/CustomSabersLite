using CustomSabersLite.Components.Game;
using CustomSabersLite.Utilities.Extensions;
using UnityEngine;

namespace CustomSabersLite.UI.Managers;

internal class PreviewSabers
{
    private LiteSaber leftSaber;
    private LiteSaber rightSaber;

    public void SetSabers(LiteSaber leftSaber, LiteSaber rightSaber)
    {
        this.leftSaber?.gameObject.DestroyImmediate();
        this.rightSaber?.gameObject.DestroyImmediate();

        this.leftSaber = leftSaber;
        this.rightSaber = rightSaber;
    }

    public void Init(Vector3 leftPosition, Vector3 rightPosition, Quaternion leftRotation, Quaternion rightRotation)
    {
        if (leftSaber && rightSaber)
        {
            leftSaber.transform.SetPositionAndRotation(leftPosition, leftRotation);
            rightSaber.transform.SetPositionAndRotation(rightPosition, rightRotation);
            leftSaber.gameObject.name = "Preview Saber Left";
            rightSaber.gameObject.name = "Preview Saber Right";
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
