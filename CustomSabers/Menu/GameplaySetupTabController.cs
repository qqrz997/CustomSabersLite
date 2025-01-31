using System;
using BeatSaberMarkupLanguage.GameplaySetup;
using CustomSabersLite.Menu.Views;
using Zenject;

namespace CustomSabersLite.Menu;

internal class GameplaySetupTabController : IInitializable, IDisposable
{
    private readonly GameplaySetupViewController gameplaySetupViewController;
    private readonly GameplaySetup gameplaySetup;
    private readonly GameplaySetupTab gameplaySetupTab;

    public GameplaySetupTabController(GameplaySetupViewController gameplaySetupViewController,
        GameplaySetup gameplaySetup,
        GameplaySetupTab gameplaySetupTab)
    {
        this.gameplaySetupViewController = gameplaySetupViewController;
        this.gameplaySetup = gameplaySetup;
        this.gameplaySetupTab = gameplaySetupTab;
    }

    private const string ResourceName = "CustomSabersLite.Menu.BSML.gameplaySetup.bsml";

    public void Initialize()
    {
        gameplaySetup.AddTab("Custom Sabers", ResourceName, gameplaySetupTab);
        gameplaySetupViewController.didActivateEvent += GameplaySetupActivated;
    }

    public void Dispose()
    {
        gameplaySetup.RemoveTab(ResourceName);
        gameplaySetupViewController.didActivateEvent -= GameplaySetupActivated;
    }

    private void GameplaySetupActivated(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling) =>
        gameplaySetupTab.Activated(firstActivation, addedToHierarchy, screenSystemEnabling);
}
