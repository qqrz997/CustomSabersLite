using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.ViewControllers;
using CustomSabersLite.Configuration;
using CustomSabersLite.Data;
using CustomSabersLite.UI.Managers;
using CustomSabersLite.Utilities;
using CustomSabersLite.Utilities.AssetBundles;
using CustomSabersLite.Utilities.Extensions;
using HMUI;
using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

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
        StartCoroutine(ScrollToSelectedCell());
        reloadButtonSelectable.interactable = true;
    }

    [UIAction("#post-parse")]
    public void PostParse() => SetupList();

    private void SetupList() // todo - smoother saber list refresh
    {
        customListTableData.data.Clear();
        var richTextRegex = new Regex(@"<[^>]*>");

        foreach (var metadata in cacheManager.SabersMetadata)
        {
            var (text, subtext) = metadata.LoadingError switch
            {
                SaberLoaderError.None => (metadata.SaberName, metadata.AuthorName),
                SaberLoaderError.Blacklist => ($"<color=#F77>Not loaded - </color> {metadata.SaberName}", "Incompatible after Beat Saber v1.29.1"),
                SaberLoaderError.InvalidFileType => ($"<color=#F77>Error - </color> {metadata.SaberName}", "File is not of a valid type"),
                SaberLoaderError.FileNotFound => ($"<color=#F77>Error - </color> {metadata.SaberName}", "Couldn't find file (was it deleted?)"),
                SaberLoaderError.LegacyWhacker => ($"<color=#F77>Not loaded - </color> {metadata.SaberName}", "Legacy whacker, incompatible with PC"),
                SaberLoaderError.NullBundle => ($"<color=#F77>Error - </color> {metadata.SaberName}", "Problem encountered when loading asset"),
                SaberLoaderError.NullAsset => ($"<color=#F77>Error - </color> {metadata.SaberName}", "Problem encountered when loading saber model"),
                _ => ($"<color=#F77>Error - </color> {metadata.SaberName}", "Unknown error encountered during loading")
            };

            customListTableData.data.Add(new(
                text,
                richTextRegex.Replace(subtext, string.Empty).Trim(),
                metadata.GetIcon()));
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
