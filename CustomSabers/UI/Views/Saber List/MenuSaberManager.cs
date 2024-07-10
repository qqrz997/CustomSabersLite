using CustomSabersLite.Components.Game;
using CustomSabersLite.Configuration;
using CustomSabersLite.UI.Views.Saber_List;
using CustomSabersLite.Utilities;
using UnityEngine;
using Zenject;

namespace CustomSabersLite.UI.Managers;

internal class MenuSaberManager
{
    [Inject] private readonly MenuPointerProvider menuPointerProvider;
    [Inject] private readonly InternalResourcesProvider internalResourcesProvider;
    [Inject] private readonly DiContainer container;
    [Inject] private readonly CSLConfig config;

    private MenuSaber leftSaber;
    private MenuSaber rightSaber;

    public void Init()
    {
        leftSaber = container.Instantiate<MenuSaber>([menuPointerProvider.LeftPointer.transform]);
        rightSaber = container.Instantiate<MenuSaber>([menuPointerProvider.RightPointer.transform]);
    }

    public void ReplaceSabers(LiteSaber newLeftSaber, LiteSaber newRightSaber)
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

        menuPointerProvider.SetPointerVisibility(!active);
    }
}
