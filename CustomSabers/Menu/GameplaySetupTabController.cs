using System;
using BeatSaberMarkupLanguage.GameplaySetup;
using CustomSabersLite.Menu.Views;
using Zenject;

namespace CustomSabersLite.Menu;

internal class GameplaySetupTabController : IInitializable, IDisposable
{
    private readonly GameplaySetup gameplaySetup;
    private readonly GameplaySetupTab gameplaySetupTab;

    public GameplaySetupTabController(GameplaySetup gameplaySetup, GameplaySetupTab gameplaySetupTab)
    {
        this.gameplaySetup = gameplaySetup;
        this.gameplaySetupTab = gameplaySetupTab;
    }

    private const string ResourceName = "CustomSabersLite.Menu.BSML.gameplaySetup.bsml";

    public void Initialize()
    {
        gameplaySetup.AddTab("Custom Sabers", ResourceName, gameplaySetupTab);
    }

    public void Dispose()
    {
        gameplaySetup.RemoveTab(ResourceName);
    }
}
