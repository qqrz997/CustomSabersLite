using BeatSaberMarkupLanguage.GameplaySetup;
using CustomSabersLite.UI.Views;
using System;
using Zenject;

namespace CustomSabersLite.UI.Managers;

internal class GameplaySetupTabController(GameplaySetupViewController gameplaySetupViewController, GameplaySetup gameplaySetup, GameplaySetupTab tab) : IInitializable, IDisposable
{
    private readonly GameplaySetupViewController gameplaySetupViewController = gameplaySetupViewController;
    private readonly GameplaySetup gameplaySetup = gameplaySetup;
    private readonly GameplaySetupTab gameplaySetupTab = tab;

    private const string ResourceName = "CustomSabersLite.UI.BSML.gameplaySetup.bsml";

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
