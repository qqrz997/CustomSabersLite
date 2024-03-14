using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.GameplaySetup;
using BeatSaberMarkupLanguage.MenuButtons;
using HMUI;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CustomSaber.UI
{
    internal class UIManager
    {
        public static CustomSaberFlowCoordinator flowCoordinator;

        private static bool created = false;

        public static MenuButton MenuButton { get; private set; }

        public static bool MenuButtonActive { get; set; } = false;

        public static void CreateMenu()
        {
            if (!created)
            {
                created = true;

                Plugin.Log.Info("Creating menu button");
                MenuButton = new MenuButton("Loading Sabers", "Choose your custom sabers.", SabersMenuButtonPressed, false);
                MenuButtons.instance.RegisterButton(MenuButton);

                Plugin.Log.Info("Creating tab");
                GameplaySetupTab tab = new GameplaySetupTab();
                GameplaySetup.instance.AddTab("Custom Sabers", "CustomSaber.UI.BSML.playerSettingsTab.bsml", tab);
            }
        }

        public static void UpdateMenuOnSabersLoaded()
        {
            // todo - menu button refresh (surely there's a proper way to do this)
            MenuButton.Interactable = true;
            MenuButton.Text = "Custom Sabers";

            if (SceneManager.GetActiveScene().name == BS_Utils.SceneNames.Menu)
            { 
                UpdateMenu(true);
            }
        }

        public static void UpdateMenu(bool active)
        {
            MenuButtons.instance.UnregisterButton(MenuButton);
            MenuButtons.instance.RegisterButton(MenuButton);

            MenuButtonActive = active;
        }

        public static void ShowCustomSaberFlowCoordinator()
        {
            if (flowCoordinator == null)
            {
                flowCoordinator = BeatSaberUI.CreateFlowCoordinator<CustomSaberFlowCoordinator>();
            }

            flowCoordinator.SetParentFlowCoordinator(BeatSaberUI.MainFlowCoordinator);
            BeatSaberUI.MainFlowCoordinator.PresentFlowCoordinator(flowCoordinator, null, ViewController.AnimationDirection.Horizontal, true);
        }

        private static void SabersMenuButtonPressed() => ShowCustomSaberFlowCoordinator();
    }
}
