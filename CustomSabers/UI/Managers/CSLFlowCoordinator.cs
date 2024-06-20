using HMUI;
using System;
using CustomSabersLite.UI.Views;
using Zenject;

namespace CustomSabersLite.UI.Managers;

internal class CSLFlowCoordinator : FlowCoordinator
{
    private MainFlowCoordinator mainFlowCoordinator;
    private SaberListViewController saberList;
    private SaberSettingsViewController saberSettings;
    private SaberPreviewManager previewManager;

    [Inject]
    public void Contruct(MainFlowCoordinator mainFlowCoordinator, SaberListViewController saberList, SaberSettingsViewController saberSettings, SaberPreviewManager previewManager)
    {
        this.mainFlowCoordinator = mainFlowCoordinator;
        this.saberList = saberList;
        this.saberSettings = saberSettings;
        this.previewManager = previewManager;
    }

    protected override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
    {
        try
        {
            if (firstActivation)
            {
                SetTitle("Custom Sabers");
                showBackButton = true;
            }

            ProvideInitialViewControllers(saberList, saberSettings);
        }
        catch (Exception ex)
        {
            Logger.Error("CSLFlowCoordinator.DidActivate");
            Logger.Error(ex.Message);
        }
    }

    protected override void BackButtonWasPressed(ViewController topViewController)
    {
        mainFlowCoordinator.DismissFlowCoordinator(this);
        previewManager.SetPreviewActive(false);
    }
}
