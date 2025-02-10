using System;
using System.Threading;
using System.Threading.Tasks;
using CustomSabersLite.Menu.Views;
using CustomSabersLite.Utilities.Services;
using HMUI;
using Zenject;
using static CustomSabersLite.Utilities.Common.UnityAsync;

namespace CustomSabersLite.Menu;

internal class CslFlowCoordinator : FlowCoordinator
{
    [Inject] private readonly SaberListViewController saberList = null!;
    [Inject] private readonly SaberSettingsViewController saberSettings = null!;
    [Inject] private readonly MetadataCacheLoader metadataCacheLoader = null!;

    private CancellationTokenSource titleTokenSource = new();

    public event Action? DidFinish;
    
    public override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
    {
        if (firstActivation) showBackButton = true;
        if (addedToHierarchy) ProvideInitialViewControllers(saberList, saberSettings);
        
        SetTitle(metadataCacheLoader.CurrentProgress switch
        {
            { Completed: true } => "Custom Sabers",
            _ => FormatProgress(metadataCacheLoader.CurrentProgress)            
        });
    }

    public override void BackButtonWasPressed(ViewController topViewController) => DidFinish?.Invoke();

    private void OnEnable() => metadataCacheLoader.LoadingProgressChanged += LoadingProgressChanged;
    private void OnDisable() => metadataCacheLoader.LoadingProgressChanged -= LoadingProgressChanged;

    private void LoadingProgressChanged(MetadataCacheLoader.Progress progress)
    {
        if (!progress.Completed)
        {
            SetTitle(FormatProgress(progress));
            return;
        }

        titleTokenSource.Cancel();
        titleTokenSource.Dispose();
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
    
    private static string FormatProgress(MetadataCacheLoader.Progress progress) => 
        !progress.StagePercent.HasValue ? $"{progress.Stage}"
        : $"{progress.Stage} {progress.StagePercent.Value}%";
}
