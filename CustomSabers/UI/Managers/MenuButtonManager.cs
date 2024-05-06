using BeatSaberMarkupLanguage.MenuButtons;
using System;
using Zenject;

namespace CustomSabersLite.UI.Managers
{
    internal class MenuButtonManager : IInitializable, IDisposable
    {
        private readonly MenuButton button;
        private readonly MainFlowCoordinator mainFlowCoordinator;
        private readonly CSLFlowCoordinator sabersFlowCoordinator;

        public MenuButtonManager(MainFlowCoordinator mainFlowCoordinator, CSLFlowCoordinator sabersFlowCoordinator)
        {
            button = new MenuButton("Custom Sabers", "Choose your custom sabers", PresentCSLFlowCoordinator, true);
            this.mainFlowCoordinator = mainFlowCoordinator;
            this.sabersFlowCoordinator = sabersFlowCoordinator;
        }

        private void PresentCSLFlowCoordinator()
        {
            mainFlowCoordinator.PresentFlowCoordinator(sabersFlowCoordinator);
        }

        public void Initialize()
        {
            MenuButtons.instance.RegisterButton(button);
        }

        public void Dispose()
        {
            MenuButtons.instance.UnregisterButton(button);
        }
    }
}
