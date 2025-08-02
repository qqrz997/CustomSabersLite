using System;
using System.Threading;
using System.Threading.Tasks;
using CustomSabersLite.Menu.Views;
using CustomSabersLite.Utilities.Extensions;
using HMUI;
using SabersLib.Services;
using Zenject;
using static CustomSabersLite.Utilities.Common.UnityAsync;

namespace CustomSabersLite.Menu;

internal class CslFlowCoordinator : FlowCoordinator
{
    [Inject] private readonly SaberListViewController saberList = null!;
    [Inject] private readonly SaberSettingsViewController saberSettings = null!;
    [Inject] private readonly ISaberMetadataLoader saberMetadataLoader = null!;

    private CancellationTokenSource titleTokenSource = new();

    public event Action? DidFinish;
    
    public override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
    {
        if (firstActivation) showBackButton = true;
        if (addedToHierarchy) ProvideInitialViewControllers(saberList, saberSettings);
        
        SetTitle(saberMetadataLoader.CurrentProgress is { Completed: true } ? "Custom Sabers"
            : FormatProgress(saberMetadataLoader.CurrentProgress));
    }

    public override void BackButtonWasPressed(ViewController topViewController) => DidFinish?.Invoke();

    private void OnEnable() => saberMetadataLoader.LoadingProgressChanged += LoadingProgressChanged;
    private void OnDisable() => saberMetadataLoader.LoadingProgressChanged -= LoadingProgressChanged;

    private void LoadingProgressChanged(MetadataLoaderProgress progress)
    {
        if (!progress.Completed)
        {
            SetTitle(FormatProgress(progress));
            return;
        }

        titleTokenSource.CancelThenDispose();
        titleTokenSource = new();
        
        StartUnitySafeTask(() => AnimateTitleOnLoadingCompleted(titleTokenSource.Token));
    }

    private async Task AnimateTitleOnLoadingCompleted(CancellationToken token)
    {
        SetTitle("<color=#BFB>Loading Completed!</color>");
        await Task.Delay(3000, token);
        SetTitle("Custom Sabers");
    }

    protected void OnDestroy() => titleTokenSource.Dispose();
    
    private static string FormatProgress(MetadataLoaderProgress progress) => 
        progress is { StagePercent: int p } ? $"{progress.Stage} {p}%" : $"{progress.Stage}";
}
