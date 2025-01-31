using System.Threading;
using System.Threading.Tasks;
using CustomSabersLite.Menu.Views;
using CustomSabersLite.Utilities.Services;
using HMUI;
using IPA.Utilities.Async;
using Zenject;

namespace CustomSabersLite.Menu;

internal class CSLFlowCoordinator : FlowCoordinator
{
    [Inject] private readonly MainFlowCoordinator mainFlowCoordinator = null!;
    [Inject] private readonly SaberListViewController saberList = null!;
    [Inject] private readonly SaberSettingsViewController saberSettings = null!;
    [Inject] private readonly MetadataCacheLoader cacheLoaderManager = null!;

    [InjectOptional] private readonly TabTest? tabTest = null;

    private MetadataCacheLoader.Progress? currentCacheProgress;
    private CancellationTokenSource cancellationTokenSource = new();


    private void Awake() =>
        cacheLoaderManager.LoadingProgressChanged += LoadingProgressChanged;

    public override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
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

    private void LoadingProgressChanged(MetadataCacheLoader.Progress progress)
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

    private string FormatProgress(MetadataCacheLoader.Progress progress) => 
        !progress.StagePercent.HasValue ? $"{progress.Stage}"
        : $"{progress.Stage} {progress.StagePercent.Value}%";

    protected void OnDestroy()
    {
        cacheLoaderManager.LoadingProgressChanged -= LoadingProgressChanged;
        cancellationTokenSource.Dispose();
    }

    public override void BackButtonWasPressed(ViewController topViewController) =>
        mainFlowCoordinator.DismissFlowCoordinator(this);
}
