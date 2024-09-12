using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.ViewControllers;
using CustomSabersLite.Configuration;
using CustomSabersLite.Models;
using CustomSabersLite.UI.Managers;
using CustomSabersLite.Utilities.AssetBundles;
using HMUI;
using IPA.Utilities.Async;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
    [Inject] private readonly CSLConfig config;
    [Inject] private readonly CacheManager cacheManager;
    [Inject] private readonly SaberListManager saberListManager;
    [Inject] private readonly SaberPreviewManager previewManager;
    [Inject] private readonly GameplaySetupTab gameplaySetupTab;
    [Inject] private readonly PluginDirs directories;

    private int selectedSaberIndex;
    private CancellationTokenSource tokenSource;

    [UIComponent("saber-list")] private CustomListTableData saberList;
    [UIComponent("saber-list-loading")] private ImageView saberListLoadingIcon;
    [UIComponent("reload-button")] private Selectable reloadButtonSelectable;
    [UIComponent("delete-saber-modal")] private ModalView deleteSaberModal;
    [UIComponent("delete-saber-modal-text")] private TextMeshProUGUI deleteSaberModalText;

    [UIAction("#post-parse")]
    public void PostParse()
    {
        reloadButtonSelectable.interactable = false;
        if (cacheManager.InitializationFinished) OnCacheInitFinished();
        else cacheManager.LoadingComplete += OnCacheInitFinished;
    }

    private void OnCacheInitFinished()
    {
        cacheManager.LoadingComplete -= OnCacheInitFinished;
        SetupList();
        reloadButtonSelectable.interactable = true;
    }

    [UIAction("select-saber")]
    public async void SelectSaber(TableView tableView, int row)
    {
        Logger.Debug($"saber selected at row {row}");
        selectedSaberIndex = row;
        config.CurrentlySelectedSaber = saberListManager.PathForIndex(row);
        await GeneratePreview();
    }

    [UIAction("open-in-explorer")]
    public void OpenInExplorer() => Process.Start(directories.CustomSabers.FullName);

    [UIAction("show-delete-saber-modal")]
    public void ShowDeleteSaberModal()
    {
        if (config.CurrentlySelectedSaber == null)
        {
            Logger.Warn("You can't delete the default sabers! >:(");
            return;
        }

        deleteSaberModalText.text = $"Are you sure you want to delete\n{Path.GetFileNameWithoutExtension(config.CurrentlySelectedSaber)}?";
        deleteSaberModal.Show(true);
    }

    [UIAction("hide-delete-saber-modal")]
    public void HideDeleteSaberModal() => deleteSaberModal.Hide(true);

    [UIAction("delete-selected-saber")]
    public void DeleteSelectedSaber()
    {
        HideDeleteSaberModal();
        if (saberListManager.DeleteSaber(config.CurrentlySelectedSaber))
        {
            SetupList();
            gameplaySetupTab.SetupList();
        }
    }

    [UIAction("reload-sabers")]
    public async void ReloadSabers()
    {
        reloadButtonSelectable.interactable = false;
        saberList.Data.Clear();
        saberList.TableView.ReloadData();
        saberListLoadingIcon.gameObject.SetActive(true);

        await cacheManager.ReloadAsync();

        SetupList();
        gameplaySetupTab.SetupList();

        saberListLoadingIcon.gameObject.SetActive(false);
        reloadButtonSelectable.interactable = true;
    }

    private void SetupList()
    {
        var filterOptions = new SaberListFilterOptions(
            config.OrderByFilter);

        saberList.Data.Clear();
        saberListManager.GetList(filterOptions)
            .ForEach(i => saberList.Data.Add(i.ToCustomCellInfo()));

        saberList.TableView.ReloadData();
        StartCoroutine(ScrollToSelectedCell());
    }

    private async Task GeneratePreview()
    {
        try
        {
            Logger.Debug("Generating preview");

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

    [UIValue("order-by-choices")] private List<object> orderByChoices = [.. Enum.GetNames(typeof(OrderBy))];
    [UIValue("order-by-filter")]
    public string OrderByFilter
    {
        get => config.OrderByFilter.ToString();
        set
        {
            config.OrderByFilter = Enum.TryParse(value, out OrderBy orderBy) ? orderBy : config.OrderByFilter;
            SetupList();
        }
    }

    [UIValue("toggle-menu-sabers")]
    public bool EnableMenuSabers
    {
        get => config.EnableMenuSabers;
        set
        {
            config.EnableMenuSabers = value;
            previewManager.UpdateActivePreview();
        }
    }

    protected override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
    {
        base.DidActivate(firstActivation, addedToHierarchy, screenSystemEnabling);

        selectedSaberIndex = saberListManager.IndexForPath(config.CurrentlySelectedSaber);

        //saberList.TableView.SelectCellWithIdx(selectedSaberIndex);
        StartCoroutine(ScrollToSelectedCell());
        UnityMainThreadTaskScheduler.Factory.StartNew(GeneratePreview);

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
        base.OnDestroy();
        tokenSource?.Dispose();
        cacheManager.LoadingComplete -= OnCacheInitFinished;
    }
}
