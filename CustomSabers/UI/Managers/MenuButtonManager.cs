using BeatSaberMarkupLanguage.MenuButtons;
using System;
using Zenject;

namespace CustomSabersLite.UI.Managers;

internal class MenuButtonManager(MainFlowCoordinator mainFlowCoordinator, CSLFlowCoordinator sabersFlowCoordinator) : IInitializable, IDisposable
{
    private readonly MainFlowCoordinator mainFlowCoordinator = mainFlowCoordinator;
    private readonly CSLFlowCoordinator sabersFlowCoordinator = sabersFlowCoordinator;

    private MenuButton button;

    public void Initialize()
    {
        button = new("Sabers Loading...", "Choose your custom sabers", PresentCSLFlowCoordinator, interactable: true);
        MenuButtons.instance.RegisterButton(button);
    }

    private void PresentCSLFlowCoordinator() =>
        mainFlowCoordinator.PresentFlowCoordinator(sabersFlowCoordinator);

    public void Dispose() => 
        MenuButtons.instance.UnregisterButton(button);
}
