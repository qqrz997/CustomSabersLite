using BeatSaberMarkupLanguage.MenuButtons;
using CustomSabersLite.Utilities.AssetBundles;
using System;
using Zenject;

namespace CustomSabersLite.UI.Managers
{
    internal class MenuButtonManager : IInitializable, IDisposable
    {
        private readonly MenuButton button;
        private readonly MainFlowCoordinator mainFlowCoordinator;
        private readonly CSLFlowCoordinator sabersFlowCoordinator;
        private readonly CacheManager cacheManager;

        public MenuButtonManager(MainFlowCoordinator mainFlowCoordinator, CSLFlowCoordinator sabersFlowCoordinator, CacheManager cacheManager)
        {
            button = new MenuButton("Sabers Loading...", "Choose your custom sabers", PresentCSLFlowCoordinator, interactable: false);
            this.mainFlowCoordinator = mainFlowCoordinator;
            this.sabersFlowCoordinator = sabersFlowCoordinator;
            this.cacheManager = cacheManager;
        }

        private void PresentCSLFlowCoordinator()
        {
            mainFlowCoordinator.PresentFlowCoordinator(sabersFlowCoordinator);
        }

        public async void Initialize()
        {
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

        public void Dispose()
        {
            MenuButtons.instance.UnregisterButton(button);
        }
    }
}
