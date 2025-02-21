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
using static CustomSabersLite.Utilities.Common.CSLResources;
using static CustomSabersLite.Utilities.Common.UnityAsync;

namespace CustomSabersLite.Menu.Views;

[HotReload(RelativePathToLayout = "../BSML/saberList.bsml")]
[ViewDefinition("CustomSabersLite.Menu.BSML.saberList.bsml")]
internal class SaberListViewController : BSMLAutomaticViewController
{
    [Inject] private readonly CslConfig config = null!;
    [Inject] private readonly DirectoryManager directoryManager = null!;
    [Inject] private readonly MetadataCacheLoader metadataCacheLoader = null!;
    [Inject] private readonly SaberMetadataCache saberMetadataCache = null!;
    [Inject] private readonly FavouritesManager favouritesManager = null!;
    [Inject] private readonly SaberListManager saberListManager = null!;
    [Inject] private readonly SaberPreviewManager previewManager = null!;

    [UIComponent("saber-list")] private readonly SaberListTableData saberList = null!;
    [UIComponent("delete-saber-modal")] private readonly ModalView deleteSaberModal = null!;
    [UIComponent("delete-saber-modal-text")] private readonly TextMeshProUGUI deleteSaberModalText = null!;
    [UIComponent("search-input")] private readonly BsInputField searchBsInputField = null!;

    [UIComponent("sort-direction-button")] private readonly ImageView sortDirectionButtonImage = null!;
    [UIComponent("preview-button")] private readonly ImageView previewButtonImage = null!;
    
    [UIObject("loading-icon")] private readonly GameObject loadingIcon = null!;

    private CancellationTokenSource saberPreviewTokenSource = new();
    private SaberListType currentSaberList = SaberListType.Sabers;

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

    public bool EnableMenuSabers
    {
        get => config.EnableMenuSabers;
        set
        {
            config.EnableMenuSabers = value;
            previewManager.UpdateActivePreview();
        }
    }

    public void ListSelected(SegmentedControl segmentedControl, int idx)
    {
        currentSaberList = (SaberListType)idx;
        RefreshList();
    }

    public async void ListCellSelected(TableView tableView, int row)
    {
        var saberListCell = saberListManager.SelectFromCurrentList(row);

        if (saberListCell is SaberListDirectoryCell directoryCell)
        {
            saberListManager.OpenFolder(directoryCell);
            RefreshList();
        }
        else if (saberListCell is SaberListUpDirectoryCell upDirectoryCell)
        {
            saberListManager.OpenFolder(upDirectoryCell);
            RefreshList();
        }
        else if (saberListCell is SaberListFavouritesCell favouritesCell)
        {
            saberListManager.ShowFavourites = true;
            RefreshList();
        }
        else if (currentSaberList == SaberListType.Sabers)
        {
            config.CurrentlySelectedSaber = saberListCell?.Value;
        }
        else if (currentSaberList == SaberListType.Trails)
        {
            config.CurrentlySelectedTrail = saberListCell?.Value;
        }

        NotifyPropertyChanged(nameof(FavouriteButtonValue));
        
        await GeneratePreview();
    }

    public void ListDirectionButtonPressed()
    {
        config.ReverseSort = !config.ReverseSort;
        sortDirectionButtonImage.sprite = config.ReverseSort ? SortAscendingIcon : SortDescendingIcon;        
        RefreshList();
    }

    public bool FavouriteButtonValue
    {
        get => saberMetadataCache.TryGetMetadata(GetSelectedSaberHash(), out var meta)  
               && favouritesManager.IsFavourite(meta.SaberFile);
        set
        {
            var meta = saberMetadataCache.GetOrDefault(GetSelectedSaberHash());
            if (meta is null) return;
        
            if (saberList.Data.TryGetElementAt(saberListManager.IndexForSaberHash(GetSelectedSaberHash()), out var cell)
                && cell is SaberListInfoCell infoCell) 
                infoCell.IsFavourite = value;
            
            if (value) favouritesManager.AddFavourite(meta.SaberFile);
            else favouritesManager.RemoveFavourite(meta.SaberFile);
        }
    }

    public void FolderButtonPressed()
    {
        Process.Start(directoryManager.CustomSabers.FullName);
    }

    public void DeleteButtonPressed()
    {
        if (config.CurrentlySelectedSaber != null)
        {
            deleteSaberModalText.text = saberMetadataCache.GetOrDefault(config.CurrentlySelectedSaber)?
                .Descriptor.SaberName.FullName ?? "Unknown";
            deleteSaberModal.Show(true);
        }
    }

    public void DeleteCancelPressed()
    {
        deleteSaberModal.Hide(true);
    }

    public void DeleteConfirmPressed()
    {
        deleteSaberModal.Hide(true);
        saberListManager.DeleteSaber(config.CurrentlySelectedSaber);
        
        int selectedSaberIndex = saberListManager.IndexForSaberHash(config.CurrentlySelectedSaber);
        config.CurrentlySelectedSaber = saberListManager.SelectFromCurrentList(selectedSaberIndex - 1)?.Value;

        RefreshList();
        StartCoroutine(SelectSelectedAndScrollTo());
    }

    public void PreviewButtonPressed()
    {
        config.EnableMenuSabers = !config.EnableMenuSabers;
        previewButtonImage.sprite = config.EnableMenuSabers ? PreviewHeldIcon : PreviewStaticIcon;
        previewManager.UpdateActivePreview();
    }

    public async void ReloadButtonPressed()
    {
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
            currentSaberList == SaberListType.Trails,
            false);
        
        saberList.Data.Clear();
        saberList.Data.AddRange(saberListManager.UpdateList(filterOptions));
        saberList.ReloadData();

        var selectedSaberHash = GetSelectedSaberHash();
        if (saberListManager.CurrentListContains(selectedSaberHash))
        {
            // todo: test this
            saberList.SelectCellWithIdx(saberListManager.IndexForSaberHash(selectedSaberHash));
        }
        else
        {
            saberList.ClearSelection();
        }

        StartUnitySafeTask(GeneratePreview);
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
            saberPreviewTokenSource.Cancel();
            saberPreviewTokenSource.Dispose();
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

    private string? GetSelectedSaberHash() => currentSaberList switch
    {
        SaberListType.Sabers => config.CurrentlySelectedSaber,
        SaberListType.Trails => config.CurrentlySelectedTrail,
        _ => null
    };
    
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
        saberPreviewTokenSource.Cancel();
    }

    protected override void OnDestroy()
    {
        metadataCacheLoader.LoadingProgressChanged -= LoadingProgressChanged;
        saberPreviewTokenSource.Dispose();
        base.OnDestroy();
    }
}