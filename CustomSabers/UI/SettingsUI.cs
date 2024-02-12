using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.MenuButtons;
using HMUI;

namespace CustomSaber.UI
{
    internal class SettingsUI
    {
        public static CustomSaberFlowCoordinator flowCoordinator;

        public static bool created = false;

        public static void CreateMenu()
        {
            if (!created)
            {
                created = true;

                MenuButton menuButton = new MenuButton("Custom Sabers", "Choose your custom sabers.", SabersMenuButtonPressed, true);
                MenuButtons.instance.RegisterButton(menuButton);
            }
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
