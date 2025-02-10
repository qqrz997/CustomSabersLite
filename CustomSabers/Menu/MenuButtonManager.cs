using System;
using BeatSaberMarkupLanguage.MenuButtons;
using Zenject;

namespace CustomSabersLite.Menu;

internal class MenuButtonManager : IInitializable, IDisposable
{
    private readonly CslFlowCoordinator cslFlowCoordinator;
    private readonly MainFlowCoordinator mainFlowCoordinator;
    private readonly MenuButtons menuButtons;
    private readonly MenuButton menuButton;

    public MenuButtonManager(
        MenuButtons menuButtons,
        CslFlowCoordinator cslFlowCoordinator,
        MainFlowCoordinator mainFlowCoordinator)
    {
        this.menuButtons = menuButtons;
        this.cslFlowCoordinator = cslFlowCoordinator;
        this.mainFlowCoordinator = mainFlowCoordinator;
        menuButton = new("CustomSabersLite", PresentFlowCoordinator);
    }
    
    public void Initialize()
    {
        cslFlowCoordinator.DidFinish += DismissFlowCoordinator;
        menuButtons.RegisterButton(menuButton);
    }

    public void Dispose()
    {
        cslFlowCoordinator.DidFinish -= DismissFlowCoordinator;
        menuButtons.UnregisterButton(menuButton);
    }
    
    private void PresentFlowCoordinator() => mainFlowCoordinator.PresentFlowCoordinator(cslFlowCoordinator);
    private void DismissFlowCoordinator() => mainFlowCoordinator.DismissFlowCoordinator(cslFlowCoordinator);
}
