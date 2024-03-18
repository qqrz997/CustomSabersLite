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

        /*public static CSLFlowCoordinator flowCoordinator;

        private static bool created = false;

        public static MenuButton MenuButton { get; private set; }

        public static bool MenuButtonActive { get; set; } = false;

        public static void CreateMenu()
        {
            if (!created)
            {
                created = true;

                Logger.Debug("Creating menu button");
                *//*MenuButton = new MenuButton("Loading Sabers", "Choose your custom sabers.", SabersMenuButtonPressed, false);
                MenuButtons.instance.RegisterButton(MenuButton);*/

                /*Logger.Debug("Creating tab");
                GameplaySetupTab tab = new GameplaySetupTab();
                GameplaySetup.instance.AddTab("Custom Sabers", "CustomSabersLite.UI.BSML.playerSettingsTab.bsml", tab);*//*
            }
        }

        public static void UpdateMenuOnSabersLoaded()
        {
            // todo - menu button refresh (surely there's a proper way to do this)
            MenuButton.Interactable = true;
            MenuButton.Text = "Custom Sabers";

            if (SceneManager.GetActiveScene().name == "MainMenu")
            { 
                UpdateMenu(true);
            }
        }

        public static void UpdateMenu(bool active)
        {
            *//*MenuButtons.instance.UnregisterButton(MenuButton);
            MenuButtons.instance.RegisterButton(MenuButton);

            MenuButtonActive = active;*//*
        }

        public static void ShowCustomSaberFlowCoordinator()
        {
            if (flowCoordinator == null)
            {
                flowCoordinator = BeatSaberUI.CreateFlowCoordinator<CSLFlowCoordinator>();
            }

            flowCoordinator.(BeatSaberUI.MainFlowCoordinator);
            BeatSaberUI.MainFlowCoordinator.PresentFlowCoordinator(flowCoordinator, null, ViewController.AnimationDirection.Horizontal, true);
        }

        private static void SabersMenuButtonPressed() => ShowCustomSaberFlowCoordinator();*/
    }
}
