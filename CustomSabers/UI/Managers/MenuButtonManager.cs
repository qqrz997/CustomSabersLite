using BeatSaberMarkupLanguage.MenuButtons;
using System;
using Zenject;

namespace CustomSabersLite.UI.Managers;

internal class MenuButtonManager(MenuButtons menuButtons, MainFlowCoordinator mainFlowCoordinator, CSLFlowCoordinator sabersFlowCoordinator) : IInitializable, IDisposable
{
    private readonly MenuButtons menuButtons = menuButtons;
    private readonly MainFlowCoordinator mainFlowCoordinator = mainFlowCoordinator;
    private readonly CSLFlowCoordinator sabersFlowCoordinator = sabersFlowCoordinator;

    private MenuButton button = null!;

    public void Initialize()
    {
        button = new("Custom Sabers", "Choose your custom sabers", PresentCSLFlowCoordinator, interactable: true);
        menuButtons.RegisterButton(button);
    }

    private void PresentCSLFlowCoordinator() =>
        mainFlowCoordinator.PresentFlowCoordinator(sabersFlowCoordinator);

    public void Dispose() =>
        menuButtons.UnregisterButton(button);
}
