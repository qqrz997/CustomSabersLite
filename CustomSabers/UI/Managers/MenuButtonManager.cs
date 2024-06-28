using BeatSaberMarkupLanguage.MenuButtons;
using CustomSabersLite.Utilities.AssetBundles;
using System;
using Zenject;
using System.Threading.Tasks;

namespace CustomSabersLite.UI.Managers;

internal class MenuButtonManager(MainFlowCoordinator mainFlowCoordinator, CSLFlowCoordinator sabersFlowCoordinator, CacheManager cacheManager) : IInitializable, IDisposable
{
    private readonly MainFlowCoordinator mainFlowCoordinator = mainFlowCoordinator;
    private readonly CSLFlowCoordinator sabersFlowCoordinator = sabersFlowCoordinator;
    private readonly CacheManager cacheManager = cacheManager;

    private MenuButton button;

    public void Initialize()
    {
        button = new("Sabers Loading...", "Choose your custom sabers", PresentCSLFlowCoordinator, interactable: false);
        MenuButtons.instance.RegisterButton(button);
        
        if (cacheManager.InitializationFinished)
        {
            OnCacheInitFinished();
        }
        else
        {
            cacheManager.CacheInitializationFinished += OnCacheInitFinished;
        }
    }

    private void PresentCSLFlowCoordinator() =>
        mainFlowCoordinator.PresentFlowCoordinator(sabersFlowCoordinator);

    private void OnCacheInitFinished()
    {
        if (cacheManager.InitializationFailed)
        {
            button.Text = "Error loading sabers";
        }
        else
        {
            button.Text = "Custom Sabers";
            button.Interactable = true;
        }

        cacheManager.CacheInitializationFinished -= OnCacheInitFinished;

        MenuButtons.instance.UnregisterButton(button);
        MenuButtons.instance.RegisterButton(button);
    }

    public void Dispose()
    {
        cacheManager.CacheInitializationFinished -= OnCacheInitFinished;
        MenuButtons.instance.UnregisterButton(button);
    }
}
