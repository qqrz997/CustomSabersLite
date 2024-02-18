using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.MenuButtons;
using HMUI;
using System.ComponentModel;
using UnityEngine.SceneManagement;

namespace CustomSaber.UI
{
    internal class SettingsUI
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

                MenuButton = new MenuButton("Loading Sabers", "Choose your custom sabers.", SabersMenuButtonPressed, false);
                MenuButtons.instance.RegisterButton(MenuButton);
            }
        }

        public static void UpdateMenuOnSabersLoaded()
        {
            //todo - menu button refresh (surely there's a proper way to do this)
            MenuButton.Interactable = true;
            MenuButton.Text = "Custom Sabers";

            if (SceneManager.GetActiveScene().name == BS_Utils.SceneNames.Menu)
            { 
                UpdateMenu();
            }
        }

        public static void UpdateMenu()
        {
            MenuButtons.instance.UnregisterButton(MenuButton);
            MenuButtons.instance.RegisterButton(MenuButton);

            MenuButtonActive = true;
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
