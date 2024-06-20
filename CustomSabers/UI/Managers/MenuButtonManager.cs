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

    private void PresentCSLFlowCoordinator() => 
        mainFlowCoordinator.PresentFlowCoordinator(sabersFlowCoordinator);

    public async void Initialize()
    {
        button = new("Sabers Loading...", "Choose your custom sabers", PresentCSLFlowCoordinator, interactable: false);
        MenuButtons.instance.RegisterButton(button);
        
        try
        {
            await cacheManager.CacheInitialization;
            button.Text = "Custom Sabers";
            button.Interactable = true;
        }
        catch (Exception ex) 
        {
            Logger.Error($"{ex}");
            button.Text = "Error loading sabers";
        }
        finally
        {
            MenuButtons.instance.UnregisterButton(button);
            MenuButtons.instance.RegisterButton(button);
        }
    }

    public void Dispose() => MenuButtons.instance.UnregisterButton(button);
}
