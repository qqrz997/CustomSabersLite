using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.ViewControllers;
using CustomSabersLite.Configuration;
using CustomSabersLite.Models;
using CustomSabersLite.Utilities;
using HMUI;
using IPA.Utilities.Async;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace CustomSabersLite.UI.Views;

[HotReload(RelativePathToLayout = "../../BSML/saberList.bsml")]
[ViewDefinition("CustomSabersLite.UI.BSML.saberList.bsml")]
internal class SaberListViewController : BSMLAutomaticViewController
{
    [Inject] private readonly CSLConfig config = null!;
    [Inject] private readonly SaberMetadataCache saberMetadataCache = null!;
    [Inject] private readonly SaberListManager saberListManager = null!;
    [Inject] private readonly SaberPreviewManager previewManager = null!;
    [Inject] private readonly LevelSearchViewController levelSearchViewController = null!;

    private CancellationTokenSource? tokenSource;

    [UIComponent("saber-list")] readonly CustomListTableData saberList = null!;
    [UIComponent("reload-button")] readonly Selectable reloadButtonSelectable = null!;
    [UIComponent("delete-saber-modal")] readonly ModalView deleteSaberModal = null!;
    [UIComponent("delete-saber-modal-text")] readonly TextMeshProUGUI deleteSaberModalText = null!;
    [UIComponent("search-input-container")] readonly RectTransform searchInputContainer = null!;
    [UIComponent("menu-sabers-toggle-button")] readonly Button menuSabersToggleButton = null!;
    [UIComponent("sort-direction-button")] readonly ImageView sortDirectionIcon = null!;
    [UIObject("loading-icon")] readonly GameObject loadingIcon = null!;

    private Button upButton = null!;
    private Button downButton = null!;
    private ImageView menuSabersToggleBackground = null!;

    [UIAction("#post-parse")]
    public void PostParse()
    {
        saberMetadataCache.LoadingProgressChanged += LoadingProgressChanged;
        saberList.TableView.scrollView.scrollPositionChangedEvent += ScrollPositionChanged;

        var searchInputFieldView = Instantiate(levelSearchViewController._searchTextInputFieldView, searchInputContainer, false);
        searchInputFieldView.text = SearchFilter;
        searchInputFieldView.onValueChanged.AddListener(ifw => SearchFilter = ifw.text);

        BSMLHelpers.ResizeVerticalScrollbar(saberList, -4f);
        var buttonBase = saberList.transform.Find("ScrollBar/UpButton").GetComponent<Button>();
        upButton = BSMLHelpers.CreateButton(buttonBase, 7f, new(0.5f, 1.0f), new(0.5f, 1.0f), new(2.5f, 2.5f), 180f, CSLResources.ExtremeArrowIcon, ScrollToTop, buttonBase.transform.parent);
        downButton = BSMLHelpers.CreateButton(buttonBase, 7f, new(0.5f, 0f), new(0.5f, 0f), new(2.5f, 2.5f), 0f, CSLResources.ExtremeArrowIcon, ScrollToBottom, buttonBase.transform.parent);

        menuSabersToggleBackground = BSMLHelpers.CreateToggleButtonBackground(menuSabersToggleButton);
        menuSabersToggleBackground.color1 = config.EnableMenuSabers ? new(0f, 0.753f, 1f) : Color.black;

        sortDirectionIcon.rectTransform.localRotation = Quaternion.Euler(0f, 0f, config.ReverseSort ? 180f : 0f);

        RefreshList();
        loadingIcon.SetActive(!saberMetadataCache.CurrentProgress.Completed);
    }

    [UIValue("order-by-choices")] private List<object> orderByChoices = [.. Enum.GetNames(typeof(OrderBy))];
    [UIValue("order-by-filter")]
    public string OrderByFilter
    {
        get => config.OrderByFilter.ToString();
        set
        {
            config.OrderByFilter = Enum.TryParse(value, out OrderBy orderBy) ? orderBy : config.OrderByFilter;
            RefreshList();
            ScrollToTop();
        }
    }

    [UIValue("search-filter")]
    public string SearchFilter
    {
        get => config.SearchFilter;
        set
        {
            config.SearchFilter = value;
            RefreshList();
            ScrollToTop();
        }
    }

    [UIAction("toggle-sort-direction")]
    public void ToggleSortDirection()
    {
        config.ReverseSort = !config.ReverseSort;
        sortDirectionIcon.rectTransform.localRotation = Quaternion.Euler(0f, 0f, config.ReverseSort ? 180f : 0f);
        RefreshList();
    }

    [UIAction("toggle-menu-sabers")]
    public void ToggleMenuSabers()
    {
        config.EnableMenuSabers = !config.EnableMenuSabers;
        menuSabersToggleBackground.color1 = config.EnableMenuSabers ? new(0f, 0.753f, 1f) : Color.black;
        previewManager.UpdateActivePreview();
    }

    [UIAction("scroll-to-selected-cell")]
    private void ScrollToSelectedCell() => 
        saberList.TableView.ScrollToCellWithIdx(saberListManager.IndexForPath(config.CurrentlySelectedSaber), TableView.ScrollPositionType.Center, true);

    [UIAction("select-saber")]
    public async void SelectSaber(TableView tableView, int row)
    {
        config.CurrentlySelectedSaber = saberListManager.Select(row)?.Metadata.SaberFile.RelativePath;
        Logger.Debug($"Saber selected: {config.CurrentlySelectedSaber ?? "Default"}");
        await GeneratePreview();
    }

    [UIAction("open-in-explorer")]
    public void OpenInExplorer() => Process.Start(PluginDirs.CustomSabers.FullName);

    [UIAction("show-delete-saber-modal")]
    public void ShowDeleteSaberModal()
    {
        if (config.CurrentlySelectedSaber != null)
        {
            deleteSaberModalText.text = $"{Path.GetFileName(config.CurrentlySelectedSaber)}?";
            deleteSaberModal.Show(true);
        }
    }

    [UIAction("hide-delete-saber-modal")]
    public void HideDeleteSaberModal() => deleteSaberModal.Hide(true);

    [UIAction("delete-selected-saber")]
    public void DeleteSelectedSaber()
    {
        HideDeleteSaberModal();
        int selectedSaberIndex = saberListManager.IndexForPath(config.CurrentlySelectedSaber);

        if (saberListManager.DeleteSaber(config.CurrentlySelectedSaber))
        {
            Logger.Debug("Saber deleted");
            config.CurrentlySelectedSaber = saberListManager.Select(selectedSaberIndex - 1)?.Metadata.SaberFile.RelativePath;

            RefreshList();
            StartCoroutine(SelectSelectedAndScrollTo());
        }
    }

    [UIAction("reload-sabers")]
    public async void ReloadSabers()
    {
        reloadButtonSelectable.interactable = false;
        saberList.Data.Clear();
        saberList.TableView.ReloadData();

        // this will invoke an event on completion that gets used to refresh the list
        await saberMetadataCache.ReloadAsync();
    }

    private void RefreshList()
    {
        var filterOptions = new SaberListFilterOptions(config.SearchFilter, config.OrderByFilter, config.ReverseSort);
        saberList.Data = saberListManager.UpdateList(filterOptions).Select(i => i.ToCustomCellInfo()).ToList();
        saberList.TableView.ReloadData();

        if (saberListManager.CurrentListContains(config.CurrentlySelectedSaber))
        {
            saberList.TableView.SelectCellWithIdx(saberListManager.IndexForPath(config.CurrentlySelectedSaber));
        }
        else
        {
            saberList.TableView.ClearSelection();
        }

        UnityMainThreadTaskScheduler.Factory.StartNew(GeneratePreview);
        reloadButtonSelectable.interactable = true;
    }

    private void ScrollToTop() =>
        saberList.TableView.ScrollToPosition(0, false);

    private void ScrollToBottom() =>
        saberList.TableView.ScrollToPosition(
            saberList.Data.Count * (saberList.TableView.cellSize + saberList.TableView.spacing) + saberList.TableView.paddingStart,
            false);

    private void ScrollPositionChanged(float _)
    {
        var scrollView = saberList.TableView.scrollView;
        float pos = scrollView._destinationPos; 
        upButton.interactable = pos > 0.001f;
        downButton.interactable = pos < scrollView.contentSize - scrollView.scrollPageSize - 0.001f;
    }

    private void LoadingProgressChanged(SaberMetadataCache.Progress progress)
    {
        if (progress.Completed) RefreshList();
        loadingIcon.SetActive(!progress.Completed);
    }

    private async Task GeneratePreview()
    {
        Logger.Debug("Generating preview");
        try
        {
            tokenSource?.Cancel();
            tokenSource?.Dispose();
            tokenSource = new();
            await previewManager.GeneratePreview(tokenSource.Token);
        }
        catch (OperationCanceledException) { }
    }

    private IEnumerator SelectSelectedAndScrollTo()
    {
        yield return new WaitUntil(() => saberList.gameObject.activeInHierarchy);
        yield return new WaitForEndOfFrame();
        int selectedSaberIndex = saberListManager.IndexForPath(config.CurrentlySelectedSaber);
        saberList.TableView.SelectCellWithIdx(selectedSaberIndex);
        saberList.TableView.ScrollToCellWithIdx(selectedSaberIndex, TableView.ScrollPositionType.Center, true);
    }

    protected override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
    {
        base.DidActivate(firstActivation, addedToHierarchy, screenSystemEnabling);

        if (!firstActivation)
        {
            UnityMainThreadTaskScheduler.Factory.StartNew(GeneratePreview);
        }

        RefreshList();
        StartCoroutine(SelectSelectedAndScrollTo());
        previewManager.SetPreviewActive(true);
    }

    protected override void DidDeactivate(bool removedFromHierarchy, bool screenSystemDisabling)
    {
        base.DidDeactivate(removedFromHierarchy, screenSystemDisabling);
        previewManager.SetPreviewActive(false);
        tokenSource?.Cancel();
    }

    protected override void OnDestroy()
    {
        saberMetadataCache.LoadingProgressChanged -= LoadingProgressChanged;
        saberList.TableView.scrollView.scrollPositionChangedEvent -= ScrollPositionChanged;
        tokenSource?.Dispose();
        base.OnDestroy();
    }
}
