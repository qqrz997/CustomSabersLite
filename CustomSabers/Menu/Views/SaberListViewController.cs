using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.ViewControllers;
using CustomSabersLite.Configuration;
using CustomSabersLite.Menu.Components;
using CustomSabersLite.Models;
using CustomSabersLite.Utilities.Common;
using CustomSabersLite.Utilities.Services;
using HMUI;
using TMPro;
using UnityEngine;
using Zenject;
using static CustomSabersLite.Utilities.Common.UnityAsync;

namespace CustomSabersLite.Menu.Views;

[HotReload(RelativePathToLayout = "../BSML/saberList.bsml")]
[ViewDefinition("CustomSabersLite.Menu.BSML.saberList.bsml")]
internal class SaberListViewController : BSMLAutomaticViewController
{
    [Inject] private readonly CslConfig config = null!;
    [Inject] private readonly MetadataCacheLoader metadataCacheLoader = null!;
    [Inject] private readonly SaberMetadataCache saberMetadataCache = null!;
    [Inject] private readonly SaberListManager saberListManager = null!;
    [Inject] private readonly SaberPreviewManager previewManager = null!;
    
    private CancellationTokenSource? saberPreviewTokenSource;
    private SaberListType currentSaberList = SaberListType.Sabers;

    [UIComponent("saber-list")] private readonly SaberListTableData saberList = null!;
    [UIComponent("delete-saber-modal")] private readonly ModalView deleteSaberModal = null!;
    [UIComponent("delete-saber-modal-text")] private readonly TextMeshProUGUI deleteSaberModalText = null!;
    [UIComponent("search-input")] private readonly BsInputField searchBsInputField = null!;
    [UIObject("loading-icon")] private readonly GameObject loadingIcon = null!;

    [UIAction("#post-parse")]
    public void PostParse()
    {
        metadataCacheLoader.LoadingProgressChanged += LoadingProgressChanged;

        searchBsInputField.Text = SearchFilter;
        searchBsInputField.AddInputChangedListener(inp => SearchFilter = inp.text);

        loadingIcon.SetActive(!metadataCacheLoader.CurrentProgress.Completed);
    }

    private List<object> orderByChoices = [.. Enum.GetNames(typeof(OrderBy))];
    public string OrderByFilter
    {
        get => config.OrderByFilter.ToString();
        set
        {
            config.OrderByFilter = Enum.TryParse(value, out OrderBy orderBy) ? orderBy : config.OrderByFilter;
            RefreshList();
            saberList.ScrollToTop();
        }
    }

    public string SearchFilter
    {
        get => config.SearchFilter;
        set
        {
            config.SearchFilter = value;
            RefreshList();
            saberList.ScrollToTop();
        }
    }

    public void ToggleSortDirection()
    {
        // TODO: change sort direction icon
        config.ReverseSort = !config.ReverseSort;
        RefreshList();
    }

    public bool EnableMenuSabers
    {
        get => config.EnableMenuSabers;
        set
        {
            config.EnableMenuSabers = value;
            previewManager.UpdateActivePreview();
        }
    }

    public void ScrollToSelectedCell()
    {
        var selectedCellIdx = saberListManager.IndexForSaberHash(config.CurrentlySelectedSaber);
        saberList.ScrollToCellWithIdx(selectedCellIdx, TableView.ScrollPositionType.Center, true);
    }

    public async void ListCellSelected(TableView tableView, int row)
    {
        var selectedCell = saberListManager.SelectFromCurrentList(row);
        if (currentSaberList == SaberListType.Sabers)
        {
            config.CurrentlySelectedSaber = selectedCell?.Value;
        }
        else if (currentSaberList == SaberListType.Trails)
        {
            config.CurrentlySelectedTrail = selectedCell?.Value;
        }
        await GeneratePreview();
    }

    public void ListSelected(SegmentedControl segmentedControl, int idx)
    {
        currentSaberList = (SaberListType)idx;
        RefreshList();
    }

    public void OpenSabersFolder() => Process.Start(PluginDirs.CustomSabers.FullName);

    public void ShowDeleteSaberModal()
    {
        if (config.CurrentlySelectedSaber != null)
        {
            deleteSaberModalText.text = saberMetadataCache.GetOrDefault(config.CurrentlySelectedSaber)?
                .Descriptor.SaberName.FullName ?? "Unknown";
            deleteSaberModal.Show(true);
        }
    }

    public void HideDeleteSaberModal() => deleteSaberModal.Hide(true);

    public void DeleteSelectedSaber()
    {
        HideDeleteSaberModal();
        if (config.CurrentlySelectedSaber is null) return;
        
        int selectedSaberIndex = saberListManager.IndexForSaberHash(config.CurrentlySelectedSaber);
        saberListManager.DeleteSaber(config.CurrentlySelectedSaber);
        
        config.CurrentlySelectedSaber = saberListManager.SelectFromCurrentList(selectedSaberIndex - 1)?.Value;

        RefreshList();
        StartCoroutine(SelectSelectedAndScrollTo());
    }

    public void ToggleHeldSabers()
    {
        config.EnableMenuSabers = !config.EnableMenuSabers;
        previewManager.UpdateActivePreview();
    }

    public async void ReloadSabers()
    {
        // TODO: interactable
        // reloadButtonSelectable.Interactable = false;
        saberList.Data.Clear();
        saberList.ReloadDataKeepingPosition();
        
        saberListManager.Clear();
        
        // this will invoke an event on completion that gets used to refresh the list
        await metadataCacheLoader.ReloadAsync();
    }

    private void RefreshList()
    {
        var filterOptions = new SaberListFilterOptions(
            config.SearchFilter, 
            config.OrderByFilter, 
            config.ReverseSort, 
            currentSaberList);
        
        saberList.Data.Clear();
        saberList.Data.AddRange(saberListManager.UpdateList(filterOptions));
        saberList.ReloadData();
        
        if (currentSaberList == SaberListType.Sabers 
            && saberListManager.CurrentListContains(config.CurrentlySelectedSaber))
        {
            saberList.SelectCellWithIdx(saberListManager.IndexForSaberHash(config.CurrentlySelectedSaber));
        }
        else if (currentSaberList == SaberListType.Trails
                 && saberListManager.CurrentListContains(config.CurrentlySelectedTrail))
        {
            saberList.SelectCellWithIdx(saberListManager.IndexForSaberHash(config.CurrentlySelectedTrail));
        }
        else
        {
            saberList.ClearSelection();
        }

        StartUnitySafeTask(GeneratePreview);
        // TODO: interactable
        // reloadButtonSelectable.Interactable = true;
    }

    private void LoadingProgressChanged(MetadataCacheLoader.Progress progress)
    {
        if (progress.Completed) RefreshList();
        loadingIcon.SetActive(!progress.Completed);
    }

    private async Task GeneratePreview()
    {
        try
        {
            saberPreviewTokenSource?.Cancel();
            saberPreviewTokenSource?.Dispose();
            saberPreviewTokenSource = new();
            await previewManager.GeneratePreview(saberPreviewTokenSource.Token);
        }
        catch (OperationCanceledException) { }
    }

    private IEnumerator SelectSelectedAndScrollTo()
    {
        yield return new WaitUntil(() => saberList.IsActive);
        yield return new WaitForEndOfFrame();
        int selectedSaberIndex = saberListManager.IndexForSaberHash(config.CurrentlySelectedSaber);
        saberList.SelectCellWithIdx(selectedSaberIndex);
        saberList.ScrollToCellWithIdx(selectedSaberIndex, TableView.ScrollPositionType.Center, true);
    }

    protected override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
    {
        base.DidActivate(firstActivation, addedToHierarchy, screenSystemEnabling);

        if (!firstActivation)
        {
            StartUnitySafeTask(GeneratePreview);
        }

        RefreshList();
        StartCoroutine(SelectSelectedAndScrollTo());
        previewManager.SetPreviewActive(true);
    }

    protected override void DidDeactivate(bool removedFromHierarchy, bool screenSystemDisabling)
    {
        base.DidDeactivate(removedFromHierarchy, screenSystemDisabling);
        previewManager.SetPreviewActive(false);
        saberPreviewTokenSource?.Cancel();
    }

    protected override void OnDestroy()
    {
        metadataCacheLoader.LoadingProgressChanged -= LoadingProgressChanged;
        saberPreviewTokenSource?.Dispose();
        base.OnDestroy();
    }
}