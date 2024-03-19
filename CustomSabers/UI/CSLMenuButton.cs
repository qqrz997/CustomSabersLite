using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.GameplaySetup;
using BeatSaberMarkupLanguage.MenuButtons;
using HMUI;
using System;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace CustomSabersLite.UI
{
    internal class CSLMenuButton : IInitializable, IDisposable
    {
        private readonly MenuButton button;
        private readonly MainFlowCoordinator mainFlowCoordinator;
        private readonly CSLFlowCoordinator sabersFlowCoordinator;

        public CSLMenuButton(MainFlowCoordinator mainFlowCoordinator, CSLFlowCoordinator sabersFlowCoordinator)
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
