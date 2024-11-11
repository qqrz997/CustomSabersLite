using CustomSabersLite.UI.Views;
using CustomSabersLite.Utilities;
using HMUI;
using IPA.Utilities.Async;
using System.Threading;
using System.Threading.Tasks;
using Zenject;

namespace CustomSabersLite.UI.Managers;

internal class CSLFlowCoordinator : FlowCoordinator
{
    [Inject] private readonly MainFlowCoordinator mainFlowCoordinator = null!;
    [Inject] private readonly SaberListViewController saberList = null!;
    [Inject] private readonly SaberSettingsViewController saberSettings = null!;
    [Inject] private readonly SaberMetadataCache cacheManager = null!;

    [InjectOptional] private readonly TabTest? tabTest = null;

    private SaberMetadataCache.Progress? currentCacheProgress;
    private CancellationTokenSource cancellationTokenSource = new();


    private void Awake() =>
        cacheManager.LoadingProgressChanged += LoadingProgressChanged;

    protected override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
    {
        if (firstActivation)
        {
            string title = currentCacheProgress is null ? "Custom Sabers"
                : currentCacheProgress.Completed ? "Custom Sabers"
                : FormatProgress(currentCacheProgress);
            SetTitle(title);
            showBackButton = true;
        }

        if (addedToHierarchy)
        {
            ProvideInitialViewControllers(saberList, saberSettings, tabTest);
        }
    }

    private void LoadingProgressChanged(SaberMetadataCache.Progress progress)
    {
        currentCacheProgress = progress;
        SetTitle(FormatProgress(progress));

        if (progress.Completed)
        {
            cancellationTokenSource.Cancel();
            cancellationTokenSource.Dispose();
            cancellationTokenSource = new();
            UnityMainThreadTaskScheduler.Factory.StartNew(async () =>
            {
                SetTitle("<color=#BFB>Loading Completed!</color>");
                await Task.Delay(3000, cancellationTokenSource.Token);
                SetTitle("Custom Sabers");
            });
        }
    }

    private string FormatProgress(SaberMetadataCache.Progress progress) => 
        !progress.StagePercent.HasValue ? $"{progress.Stage}"
        : $"{progress.Stage} {progress.StagePercent.Value}%";

    protected void OnDestroy()
    {
        cacheManager.LoadingProgressChanged -= LoadingProgressChanged;
        cancellationTokenSource.Dispose();
    }

    protected override void BackButtonWasPressed(ViewController topViewController) =>
        mainFlowCoordinator.DismissFlowCoordinator(this);
}
