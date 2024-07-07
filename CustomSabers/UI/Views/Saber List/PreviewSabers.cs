using CustomSabersLite.Components.Game;
using CustomSabersLite.Utilities.Extensions;
using UnityEngine;

namespace CustomSabersLite.UI.Managers;

internal class PreviewSabers
{
    private LiteSaber leftSaber;
    private LiteSaber rightSaber;

    private Vector3 leftPosition;
    private Vector3 rightPosition;
    private Quaternion leftRotation;
    private Quaternion rightRotation;

    public void Init(Vector3 leftPosition, Vector3 rightPosition, Quaternion leftRotation, Quaternion rightRotation)
    {
        this.leftPosition = leftPosition;
        this.rightPosition = rightPosition;
        this.leftRotation = leftRotation;
        this.rightRotation = rightRotation;
    }

    public void ReplaceSabers(LiteSaber newLeftSaber, LiteSaber newRightSaber)
    {
        leftSaber?.gameObject.DestroyImmediate();
        rightSaber?.gameObject.DestroyImmediate();

        leftSaber = newLeftSaber;
        rightSaber = newRightSaber;

        if (leftSaber)
        {
            leftSaber.transform.SetPositionAndRotation(leftPosition, leftRotation);
            leftSaber.gameObject.name = "Preview Saber Left";
        }

        if (rightSaber)
        {
            rightSaber.transform.SetPositionAndRotation(rightPosition, rightRotation);
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
