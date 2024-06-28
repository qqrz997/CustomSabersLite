using BeatSaberMarkupLanguage.MenuButtons;
using CustomSabersLite.Utilities.AssetBundles;
using System;
using Zenject;

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

        if (cacheManager.Initialized)
        {
            OnCacheInitSucceeded();
        }
        else
        {
            cacheManager.CacheInitializationSucceeded += OnCacheInitSucceeded;
            cacheManager.CacheInitializationFailed += OnCacheInitFailed;
        }
    }

    private void PresentCSLFlowCoordinator() =>
        mainFlowCoordinator.PresentFlowCoordinator(sabersFlowCoordinator);

    private void OnCacheInitSucceeded()
    {
        button.Text = "Custom Sabers";
        button.Interactable = true;

        MenuButtons.instance.UnregisterButton(button);
        MenuButtons.instance.RegisterButton(button);
    }

    private void OnCacheInitFailed()
    {
        button.Text = "Error loading sabers";

        MenuButtons.instance.UnregisterButton(button);
        MenuButtons.instance.RegisterButton(button);
    }

    public void Dispose()
    {
        cacheManager.CacheInitializationSucceeded -= OnCacheInitSucceeded;
        cacheManager.CacheInitializationFailed -= OnCacheInitFailed;

        MenuButtons.instance.UnregisterButton(button);
    }
}
