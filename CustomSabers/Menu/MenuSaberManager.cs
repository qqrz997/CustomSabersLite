using CustomSabersLite.Models;
using CustomSabersLite.Services;
using UnityEngine;
using Zenject;

namespace CustomSabersLite.Menu;

internal class MenuSaberManager
{
    private readonly MenuPointers menuPointers;
    private readonly MenuSaber leftSaber;
    private readonly MenuSaber rightSaber;

    public MenuSaberManager(
        MenuPointers menuPointers,
        [Inject(Id = SaberType.SaberA)] MenuSaber leftSaber,
        [Inject(Id = SaberType.SaberB)] MenuSaber rightSaber)
    {
        leftSaber.SetParent(menuPointers.LeftParent);
        rightSaber.SetParent(menuPointers.RightParent);
        
        this.menuPointers = menuPointers;
        this.leftSaber = leftSaber;
        this.rightSaber = rightSaber;
    }

    public void ReplaceSabers(SaberInstanceSet saberSet)
    {
        leftSaber.ReplaceSaber(saberSet.LeftSaber, saberSet.LeftTrails);
        rightSaber.ReplaceSaber(saberSet.RightSaber, saberSet.RightTrails);
    }

    public void UpdateTrails()
    {
        leftSaber.UpdateTrails();
        rightSaber.UpdateTrails();
    }

    public void UpdateSaberScale(float length, float width)
    {
        leftSaber.UpdateSaberScale(length, width);
        rightSaber.UpdateSaberScale(length, width);
    }

    public void SetColor(Color left, Color right)
    {
        leftSaber.SetColor(left);
        rightSaber.SetColor(right);
    }

    public void SetActive(bool active)
    {
        leftSaber.SetActive(active);
        rightSaber.SetActive(active);

        menuPointers.SetPointerVisibility(!active);
    }
}
