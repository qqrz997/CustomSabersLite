using System;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.ViewControllers;
using HMUI;
using CustomSabersLite.Data;
using CustomSabersLite.Utilities;
using CustomSabersLite.Utilities.AssetBundles;
using CustomSabersLite.Configuration;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using TMPro;
using System.Diagnostics;
using Zenject;
using System.Collections;
using CustomSabersLite.UI.Managers;
using System.Threading;

namespace CustomSabersLite.UI.Views;

[HotReload(RelativePathToLayout = "../BSML/saberList.bsml")]
[ViewDefinition("CustomSabersLite.UI.BSML.saberList.bsml")]
internal class SaberListViewController : BSMLAutomaticViewController
{
    private CSLConfig config;
    private CacheManager cacheManager;
    private SaberPreviewManager previewManager;
    private GameplaySetupTab gameplaySetupTab;

    private string saberAssetPath;
    private string deletedSabersPath;

    [Inject]
    public void Construct(PluginDirs pluginDirs, CSLConfig config, CacheManager cacheManager, SaberPreviewManager previewManager, GameplaySetupTab gameplaySetupTab)
    {
        this.config = config;
        this.cacheManager = cacheManager;
        this.previewManager = previewManager;
        this.gameplaySetupTab = gameplaySetupTab;

        saberAssetPath = pluginDirs.CustomSabers.FullName;
        deletedSabersPath = pluginDirs.DeletedSabers.FullName;
    }

    [UIComponent("saber-list")]
    public CustomListTableData customListTableData;

    [UIComponent("reload-button")]
    public Selectable reloadButtonSelectable;

    [UIComponent("delete-saber-modal")]
    public ModalView deleteSaberModal;

    [UIComponent("delete-saber-modal-text")]
    public TextMeshProUGUI deleteSaberModalText;

    private CancellationTokenSource tokenSource;

    [UIAction("select-saber")]
    public async void Select(TableView _, int row)
    {
        Logger.Debug($"saber selected at row {row}");

        tokenSource?.Cancel();
        tokenSource?.Dispose();
        tokenSource = new();

        cacheManager.SelectedSaberIndex = row;
        config.CurrentlySelectedSaber = cacheManager.SabersMetadata[row].RelativePath;

        try
        {
            await previewManager.GeneratePreview(tokenSource.Token);
        }
        catch (OperationCanceledException) { }
    }

    [UIAction("open-in-explorer")]
    public void OpenInExplorer() => Process.Start(saberAssetPath);

    [UIAction("show-delete-saber-modal")]
    public void ShowDeleteSaberModal()
    {
        if (config.CurrentlySelectedSaber == null)
        {
            Logger.Warn("You can't delete the default sabers! >:(");
            return;
        }

        deleteSaberModalText.text = $"Are you sure you want to delete\n{ Path.GetFileNameWithoutExtension(config.CurrentlySelectedSaber) }?";
        deleteSaberModal.Show(true);
    }

    [UIAction("hide-delete-saber-modal")]
    public void HideDeleteSaberModal() => deleteSaberModal.Hide(true);

    [UIAction("delete-selected-saber")]
    public void DeleteSelectedSaber()
    {
        HideDeleteSaberModal();

        var selectedSaber = config.CurrentlySelectedSaber;
        if (selectedSaber != null)
        {
            var currentSaberPath = Path.Combine(saberAssetPath, selectedSaber);
            var destinationPath = Path.Combine(deletedSabersPath, selectedSaber);
            try
            {
                if (File.Exists(destinationPath)) File.Delete(destinationPath);

                if (File.Exists(currentSaberPath))
                {
                    Logger.Debug($"Moving {currentSaberPath}\nto {deletedSabersPath}");
                    File.Move(currentSaberPath, destinationPath);

                    cacheManager.SabersMetadata.RemoveAt(cacheManager.SelectedSaberIndex);
                    SetupList();
                    gameplaySetupTab.SetupList();
                    cacheManager.SelectedSaberIndex--;
                    StartCoroutine(ScrollToSelectedCell());
                }
                else
                {
                    Logger.Warn($"Saber doesn't exist at {currentSaberPath}");
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Problem encountered when trying to delete selected saber");
                Logger.Error(ex.Message);
            }
        }
    }

    [UIAction("reload-sabers")]
    public async void ReloadSabers()
    {
        reloadButtonSelectable.interactable = false;
        await cacheManager.ReloadAsync();
        SetupList();
        gameplaySetupTab.SetupList();
        Select(customListTableData.tableView, cacheManager.SelectedSaberIndex);
        reloadButtonSelectable.interactable = true;
    }

    [UIAction("#post-parse")]
    public void PostParse() => SetupList();

    private void SetupList() // todo - smoother saber list refresh
    {
        customListTableData.data.Clear();

        foreach (var metadata in cacheManager.SabersMetadata)
        {
            if (metadata.RelativePath == null)
            {
                customListTableData.data.Add(new CustomListTableData.CustomCellInfo(metadata.SaberName, metadata.AuthorName, ImageUtils.defaultCoverImage));
            }
            else if (!File.Exists(Path.Combine(saberAssetPath, metadata.RelativePath)))
            {
                continue;
            }
            else
            {
                customListTableData.data.Add(new CustomListTableData.CustomCellInfo(
                    metadata.SaberName,
                    metadata.AuthorName,
                    metadata.CoverImage is null ? ImageUtils.nullCoverImage : metadata.CoverImage.LoadImage()
                ));
            }
        }

        customListTableData.tableView.ReloadData();
    }

    private IEnumerator ScrollToSelectedCell()
    {
        yield return new WaitUntil(() => customListTableData.gameObject.activeInHierarchy);
        yield return new WaitForEndOfFrame();
        var selectedSaber = cacheManager.SelectedSaberIndex;
        customListTableData.tableView.SelectCellWithIdx(selectedSaber);
        customListTableData.tableView.ScrollToCellWithIdx(selectedSaber, TableView.ScrollPositionType.Center, true);
        Select(customListTableData.tableView, cacheManager.SelectedSaberIndex);
    }

    protected override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
    {
        base.DidActivate(firstActivation, addedToHierarchy, screenSystemEnabling);
        StartCoroutine(ScrollToSelectedCell());
    }

    protected override void DidDeactivate(bool removedFromHierarchy, bool screenSystemDisabling)
    {
        base.DidDeactivate(removedFromHierarchy, screenSystemDisabling);
        tokenSource?.Cancel();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        tokenSource?.Dispose();
    }
}
