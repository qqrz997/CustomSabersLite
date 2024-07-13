using CustomSabersLite.Components.Game;
using CustomSabersLite.UI.Views.Saber_List;
using CustomSabersLite.Utilities;
using UnityEngine;
using Zenject;

namespace CustomSabersLite.UI.Managers;

internal class MenuSaberManager : IInitializable
{
    [Inject] private readonly MenuPointerProvider menuPointerProvider;
    [Inject] private readonly MenuSaber.Factory menuSaberFactory;

    private MenuSaber leftSaber;
    private MenuSaber rightSaber;

    public void Initialize()
    {
        leftSaber = menuSaberFactory.Create(menuPointerProvider.LeftPointer.transform, SaberType.SaberA);
        rightSaber = menuSaberFactory.Create(menuPointerProvider.RightPointer.transform, SaberType.SaberB);
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
