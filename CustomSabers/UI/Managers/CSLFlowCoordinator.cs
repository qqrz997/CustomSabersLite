﻿using CustomSabersLite.UI.Views;
using CustomSabersLite.Utilities.AssetBundles;
using HMUI;
using System.Threading.Tasks;
using Zenject;

namespace CustomSabersLite.UI.Managers;

internal class CSLFlowCoordinator : FlowCoordinator
{
    [Inject] private readonly MainFlowCoordinator mainFlowCoordinator;
    [Inject] private readonly SaberListViewController saberList;
    [Inject] private readonly SaberSettingsViewController saberSettings;
    [Inject] private readonly CacheManager cacheManager;

    [InjectOptional] private readonly TabTest tabTest;

    protected override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
    {
        if (firstActivation)
        {
            SetTitle("Custom Sabers");
            showBackButton = true;
        }

        if (addedToHierarchy)
        {
            ProvideInitialViewControllers(saberList, saberSettings, tabTest);
        }

        cacheManager.LoadingProgressChanged += DisplayPercentageProgress;
        cacheManager.LoadingComplete += LoadingCompleted;
    }

    private void DisplayPercentageProgress(int percent)
    {
        var barString = new string('+', percent / 10);
        var percentString = $"{percent}%";
        SetTitle($"{barString,10} {percentString} {barString,-10}");
    }

    private async void LoadingCompleted()
    {
        SetTitle("<color=#BFB>Loading Completed!</color>");
        await Task.Delay(3000);
        SetTitle("Custom Sabers");
    }

    protected override void DidDeactivate(bool removedFromHierarchy, bool screenSystemDisabling)
    {
        cacheManager.LoadingProgressChanged -= DisplayPercentageProgress;
        cacheManager.LoadingComplete -= LoadingCompleted;
    }

    protected override void BackButtonWasPressed(ViewController topViewController) =>
        mainFlowCoordinator.DismissFlowCoordinator(this);
}
