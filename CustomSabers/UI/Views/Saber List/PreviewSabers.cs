using CustomSabersLite.Components.Game;
using CustomSabersLite.Components.Managers;
using CustomSabersLite.Utilities.Extensions;
using UnityEngine;

namespace CustomSabersLite.UI.Managers;

internal class PreviewSabers
{
    private LiteSaber leftSaber;
    private LiteSaber rightSaber;

    private const string leftSaberName = "Left Preview LiteSaber";
    private const string rightSaberName = "Right Preview LiteSaber";

    public void SetSabers(LiteSaberSet saberSet)
    {
        leftSaber?.gameObject.DestroyImmediate();
        rightSaber?.gameObject.DestroyImmediate();

        leftSaber = saberSet.NewSaberForSaberType(SaberType.SaberA);
        rightSaber = saberSet.NewSaberForSaberType(SaberType.SaberB);
    }

    public void Init(Vector3 leftPosition, Vector3 rightPosition, Quaternion rotation)
    {
        leftSaber?.transform.SetPositionAndRotation(leftPosition, rotation);
        rightSaber?.transform.SetPositionAndRotation(rightPosition, rotation);
        leftSaber.gameObject.name = leftSaberName;
        rightSaber.gameObject.name = rightSaberName;
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
