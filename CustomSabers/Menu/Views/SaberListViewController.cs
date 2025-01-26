using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
using IPA.Utilities.Async;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using Logger = CustomSabersLite.Logger;

namespace CustomSabersLite.Menu.Views;

[HotReload(RelativePathToLayout = "../BSML/saberList.bsml")]
[ViewDefinition("CustomSabersLite.Menu.BSML.saberList.bsml")]
internal class SaberListViewController : BSMLAutomaticViewController
{
    [Inject] private readonly CslConfig config = null!;
    [Inject] private readonly SaberMetadataCache saberMetadataCache = null!;
    [Inject] private readonly SaberListManager saberListManager = null!;
    [Inject] private readonly SaberPreviewManager previewManager = null!;
    
    private CancellationTokenSource? saberPreviewTokenSource;

    [UIComponent("saber-list")] private readonly SaberListTableData saberList = null!;
    [UIComponent("reload-button")] private readonly Selectable reloadButtonSelectable = null!;
    [UIComponent("delete-saber-modal")] private readonly ModalView deleteSaberModal = null!;
    [UIComponent("delete-saber-modal-text")] private readonly TextMeshProUGUI deleteSaberModalText = null!;
    [UIComponent("search-input")] private readonly BsInputField searchBsInputField = null!;
    [UIComponent("sort-direction-button")] private readonly ImageView sortDirectionIcon = null!;
    [UIObject("loading-icon")] private readonly GameObject loadingIcon = null!;

    [UIAction("#post-parse")]
    public void PostParse()
    {
        saberMetadataCache.LoadingProgressChanged += LoadingProgressChanged;
        saberList.DidSelectCellWithIdxEvent += SelectSaber;

        searchBsInputField.Text = SearchFilter;
        searchBsInputField.AddInputChangedListener(inp => SearchFilter = inp.text);

        sortDirectionIcon.rectTransform.localRotation = Quaternion.Euler(0f, 0f, config.ReverseSort ? 180f : 0f);

        loadingIcon.SetActive(!saberMetadataCache.CurrentProgress.Completed);
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
        config.ReverseSort = !config.ReverseSort;
        sortDirectionIcon.rectTransform.localRotation = Quaternion.Euler(0f, 0f, config.ReverseSort ? 180f : 0f);
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
        var selectedCellIdx = saberListManager.IndexForPath(config.CurrentlySelectedSaber);
        saberList.ScrollToCellWithIdx(selectedCellIdx, TableView.ScrollPositionType.Center, true);
    }

    public async void SelectSaber(TableView tableView, int row)
    {
        config.CurrentlySelectedSaber = saberListManager.Select(row)?.Metadata.SaberFile.RelativePath;
        Logger.Debug($"Saber selected: {config.CurrentlySelectedSaber ?? "Default"}");
        await GeneratePreview();
    }

    public void OpenSabersFolder() => Process.Start(PluginDirs.CustomSabers.FullName);

    public void ShowDeleteSaberModal()
    {
        if (config.CurrentlySelectedSaber != null)
        {
            deleteSaberModalText.text = $"{Path.GetFileName(config.CurrentlySelectedSaber)}?";
            deleteSaberModal.Show(true);
        }
    }

    public void HideDeleteSaberModal() => deleteSaberModal.Hide(true);

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

    public async void ReloadSabers()
    {
        reloadButtonSelectable.interactable = false;
        saberList.Data.Clear();
        saberList.ReloadDataKeepingPosition();

        // this will invoke an event on completion that gets used to refresh the list
        await saberMetadataCache.ReloadAsync();
    }

    private void RefreshList()
    {
        var filterOptions = new SaberListFilterOptions(config.SearchFilter, config.OrderByFilter, config.ReverseSort);
        saberList.Data.Clear();
        saberList.Data.AddRange(saberListManager.UpdateList(filterOptions));
        saberList.ReloadData();

        if (saberListManager.CurrentListContains(config.CurrentlySelectedSaber))
        {
            saberList.SelectCellWithIdx(saberListManager.IndexForPath(config.CurrentlySelectedSaber));
        }
        else
        {
            saberList.ClearSelection();
        }

        UnityMainThreadTaskScheduler.Factory.StartNew(GeneratePreview);
        reloadButtonSelectable.interactable = true;
    }

    private void LoadingProgressChanged(SaberMetadataCache.Progress progress)
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
        int selectedSaberIndex = saberListManager.IndexForPath(config.CurrentlySelectedSaber);
        saberList.SelectCellWithIdx(selectedSaberIndex);
        saberList.ScrollToCellWithIdx(selectedSaberIndex, TableView.ScrollPositionType.Center, true);
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
        saberPreviewTokenSource?.Cancel();
    }

    protected override void OnDestroy()
    {
        saberMetadataCache.LoadingProgressChanged -= LoadingProgressChanged;
        saberPreviewTokenSource?.Dispose();
        base.OnDestroy();
    }
}
