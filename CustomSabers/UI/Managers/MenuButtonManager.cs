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
        button = new("Custom Sabers", "Choose your custom sabers", PresentCSLFlowCoordinator, interactable: true);
        MenuButtons.Instance.RegisterButton(button);
    }

    private void PresentCSLFlowCoordinator() =>
        mainFlowCoordinator.PresentFlowCoordinator(sabersFlowCoordinator);

    public void Dispose() =>
        MenuButtons.Instance.UnregisterButton(button);
}
