using CustomSabersLite.Models;
using CustomSabersLite.Utilities;
using UnityEngine;

namespace CustomSabersLite.UI.Managers;

internal class MenuSaberManager(MenuPointers menuPointers, MenuSaber.Factory menuSaberFactory)
{
    private readonly MenuPointers menuPointers = menuPointers;
    private readonly MenuSaber leftSaber = menuSaberFactory.Create(menuPointers.LeftParent, SaberType.SaberA);
    private readonly MenuSaber rightSaber = menuSaberFactory.Create(menuPointers.RightParent, SaberType.SaberB);

    public void ReplaceSabers(ILiteSaber? newLeftSaber, ILiteSaber? newRightSaber)
    {
        leftSaber?.ReplaceSaber(newLeftSaber);
        rightSaber?.ReplaceSaber(newRightSaber);
    }

    public void UpdateTrails()
    {
        leftSaber?.UpdateTrails();
        rightSaber?.UpdateTrails();
    }

    public void SetColor(Color left, Color right)
    {
        leftSaber?.SetColor(left);
        rightSaber?.SetColor(right);
    }

    public void SetActive(bool active)
    {
        leftSaber?.SetActive(active);
        rightSaber?.SetActive(active);

        menuPointers.SetPointerVisibility(!active);
    }
}
