using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.ViewControllers;
using CustomSabersLite.Configuration;
using CustomSabersLite.Data;
using CustomSabersLite.UI.Managers;
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

    private CancellationTokenSource tokenSource;
     
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

    [UIComponent("saber-list")] public CustomListTableData customListTableData;
    [UIComponent("saber-list-loading")] public ImageView saberListLoadingIcon;
    [UIComponent("saber-preview-loading")] public ImageView saberPreviewLoadingIcon;
    [UIComponent("reload-button")] public Selectable reloadButtonSelectable;
    [UIComponent("delete-saber-modal")] public ModalView deleteSaberModal;
    [UIComponent("delete-saber-modal-text")] public TextMeshProUGUI deleteSaberModalText;

    [UIAction("select-saber")]
    public async void OnSelect(TableView _, int row)
    {
        if (!cacheManager.InitializationFinished) return;

        Logger.Debug($"saber selected at row {row}");
        cacheManager.SelectedSaberIndex = row;
        config.CurrentlySelectedSaber = cacheManager.SabersMetadata[row].RelativePath;

        try
        {
            Logger.Debug("Generating preview");

            tokenSource?.Cancel();
            tokenSource?.Dispose();
            tokenSource = new();

            saberPreviewLoadingIcon.gameObject.SetActive(true);
            await previewManager.GeneratePreview(tokenSource.Token);
        }
        catch (OperationCanceledException) { }
        finally
        {
            saberPreviewLoadingIcon.gameObject.SetActive(false);
        }
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

        customListTableData.data.Clear();
        customListTableData.tableView.ReloadData();
        saberListLoadingIcon.gameObject.SetActive(true);

        await cacheManager.ReloadAsync();

        SetupList();
        gameplaySetupTab.SetupList();

        reloadButtonSelectable.interactable = true;
    }

    private void SetupList()
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
        saberListLoadingIcon.gameObject.SetActive(false);
        StartCoroutine(ScrollToSelectedCell());
        OnSelect(customListTableData.tableView, cacheManager.SelectedSaberIndex);
    }

    private IEnumerator ScrollToSelectedCell()
    {
        yield return new WaitUntil(() => customListTableData.gameObject.activeInHierarchy);
        yield return new WaitForEndOfFrame();
        customListTableData.tableView.SelectCellWithIdx(cacheManager.SelectedSaberIndex);
        customListTableData.tableView.ScrollToCellWithIdx(cacheManager.SelectedSaberIndex, TableView.ScrollPositionType.Center, true);
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
        base.OnDestroy();
        tokenSource?.Dispose();
        cacheManager.LoadingComplete -= OnCacheInitFinished;
    }
}
