using BeatSaberMarkupLanguage.GameplaySetup;
using CustomSabersLite.UI.Views;
using System;
using Zenject;

namespace CustomSabersLite.UI.Managers;

internal class GameplaySetupTabController : IInitializable, IDisposable
{
    [Inject] private readonly GameplaySetupViewController gameplaySetupViewController;
    [Inject] private readonly GameplaySetupTab gameplaySetupTab;

    private const string ResourceName = "CustomSabersLite.UI.BSML.gameplaySetup.bsml";

    public void Initialize()
    {
        GameplaySetup.Instance.AddTab("Custom Sabers", ResourceName, gameplaySetupTab);
        gameplaySetupViewController.didActivateEvent += GameplaySetupActivated;
    }

    public void Dispose()
    {
        GameplaySetup.Instance.RemoveTab(ResourceName);
        gameplaySetupViewController.didActivateEvent -= GameplaySetupActivated;
    }

    private void GameplaySetupActivated(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling) =>
        gameplaySetupTab.Activated(firstActivation, addedToHierarchy, screenSystemEnabling);
}
