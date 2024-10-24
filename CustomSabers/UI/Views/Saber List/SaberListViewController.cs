using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.ViewControllers;
using CustomSabersLite.Configuration;
using CustomSabersLite.Models;
using CustomSabersLite.UI.Managers;
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
    [Inject] private readonly GameplaySetupTab gameplaySetupTab = null!;

    private CancellationTokenSource? tokenSource;

    [UIComponent("saber-list")] private CustomListTableData saberList = null!;
    [UIComponent("saber-list-loading")] private ImageView saberListLoadingIcon = null!;
    [UIComponent("reload-button")] private Selectable reloadButtonSelectable = null!;
    [UIComponent("search-keyboard")] private ModalView searchKeyboard = null!;
    [UIComponent("delete-saber-modal")] private ModalView deleteSaberModal = null!;
    [UIComponent("delete-saber-modal-text")] private TextMeshProUGUI deleteSaberModalText = null!;

    private Button upButton = null!;
    private Button downButton = null!;

    [UIAction("#post-parse")]
    public void PostParse()
    {
        saberListManager.SaberListUpdated += RefreshList;
        saberList.TableView.scrollView.scrollPositionChangedEvent += ScrollPositionChanged;

        BSMLHelpers.ResizeVerticalScrollbar(saberList, -4f);
        var buttonBase = saberList.transform.Find("ScrollBar/UpButton").GetComponent<Button>();
        upButton = BSMLHelpers.CreateButton(buttonBase, 7f, new(0.5f, 1.0f), new(0.5f, 1.0f), new(2.5f, 2.5f), 180f, CSLResources.ExtremeArrowIcon, ScrollToTop, buttonBase.transform.parent);
        downButton = BSMLHelpers.CreateButton(buttonBase, 7f, new(0.5f, 0f), new(0.5f, 0f), new(2.5f, 2.5f), 0f, CSLResources.ExtremeArrowIcon, ScrollToBottom, buttonBase.transform.parent);

        if (saberMetadataCache.CurrentProgress.Completed)
        {
            RefreshList();
        }
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

    [UIAction("toggle-menu-sabers")]
    public void ToggleMenuSabers()
    {
        config.EnableMenuSabers = !config.EnableMenuSabers;
        previewManager.UpdateActivePreview();
    }

    [UIAction("select-saber")]
    public async void SelectSaber(TableView tableView, int row)
    {
        config.CurrentlySelectedSaber = saberListManager.Select(row)?.Metadata.FileInfo.RelativePath;
        Logger.Debug($"Saber selected: {config.CurrentlySelectedSaber ?? "Default"}");
        await GeneratePreview();
    }

    [UIAction("open-in-explorer")]
    public void OpenInExplorer() => Process.Start(PluginDirs.CustomSabers.FullName);

    [UIAction("show-search-keyboard")]
    public void ShowSearchKeyboard() => searchKeyboard.Show(true);

    [UIAction("show-delete-saber-modal")]
    public void ShowDeleteSaberModal()
    {
        if (config.CurrentlySelectedSaber != null)
        {
            deleteSaberModalText.text = $"Are you sure you want to delete\n{Path.GetFileName(config.CurrentlySelectedSaber)}?";
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
            config.CurrentlySelectedSaber = saberListManager.Select(selectedSaberIndex - 1)?.Metadata.FileInfo.RelativePath;

            RefreshList();
            StartCoroutine(ScrollToSelectedCell());
        }
    }

    [UIAction("reload-sabers")]
    public async void ReloadSabers()
    {
        reloadButtonSelectable.interactable = false;
        saberList.Data.Clear();
        saberList.TableView.ReloadData();
        saberListLoadingIcon.gameObject.SetActive(true);

        await saberMetadataCache.ReloadAsync();

        saberListLoadingIcon.gameObject.SetActive(false);
        RefreshList();
    }

    private void RefreshList()
    {
        saberList.TableView.ClearSelection();

        saberListManager.Sort(new SaberListFilterOptions(
            config.SearchFilter,
            config.OrderByFilter));

        saberList.Data = saberListManager.List.Select(i => i.ToCustomCellInfo()).ToList();
        saberList.TableView.ReloadData();

        gameplaySetupTab.SetupList();

        if (saberListManager.Contains(config.CurrentlySelectedSaber))
        {
            saberList.TableView.SelectCellWithIdx(saberListManager.IndexForPath(config.CurrentlySelectedSaber));
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
        var pos = scrollView._destinationPos; 
        upButton.interactable = pos > 0.001f;
        downButton.interactable = pos < scrollView.contentSize - scrollView.scrollPageSize - 0.001f;
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

    private IEnumerator ScrollToSelectedCell()
    {
        yield return new WaitUntil(() => saberList.gameObject.activeInHierarchy);
        yield return new WaitForEndOfFrame();
        var selectedSaberIndex = saberListManager.IndexForPath(config.CurrentlySelectedSaber);
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
        StartCoroutine(ScrollToSelectedCell());
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
        saberListManager.SaberListUpdated -= RefreshList;
        saberList.TableView.scrollView.scrollPositionChangedEvent -= ScrollPositionChanged;
        tokenSource?.Dispose();
        base.OnDestroy();
    }
}
