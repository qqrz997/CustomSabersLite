using HMUI;
using System;
using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Animations;

namespace CustomSaber.UI
{
    internal class CustomSaberFlowCoordinator : FlowCoordinator
    {
        public FlowCoordinator ParentFlowCoordinator { get; protected set; }
        public bool AllowFlowCoordinatorChange { get; protected set; } = true;

        private SaberListViewController saberList;
        private SaberSettingsViewController saberSettings;

        public void Awake()
        {
            if (!saberList)
            {
                saberList = BeatSaberUI.CreateViewController<SaberListViewController>();
            }

            if (!saberSettings)
            {
                saberSettings = BeatSaberUI.CreateViewController<SaberSettingsViewController>();
            }
        }

        public void SetParentFlowCoordinator(FlowCoordinator parent)
        {
            if (!AllowFlowCoordinatorChange)
                throw new InvalidOperationException("Changing the parent FlowCoordinator is not allowed on this instance.");
            ParentFlowCoordinator = parent;
        }

        protected override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
        {
            try
            {
                if (firstActivation)
                {
                    SetTitle("Custom Sabers");
                    showBackButton = true;
                    ProvideInitialViewControllers(saberList, saberSettings);
                }
            }
            catch (Exception ex)
            {
                Plugin.Log.Error(ex);
            }
        }

        protected override void BackButtonWasPressed(ViewController topViewController)
        {
            ParentFlowCoordinator.DismissFlowCoordinator(this);
        }
    }
}
