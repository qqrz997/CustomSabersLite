using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.ViewControllers;
using CustomSabersLite.Configuration;
using CustomSabersLite.Menu.Components;
using CustomSabersLite.Models;
using CustomSabersLite.Services;
using CustomSabersLite.Utilities.Extensions;
using HMUI;
using TMPro;
using UnityEngine;
using Zenject;
using static CustomSabersLite.Utilities.Common.PluginResources;
using static CustomSabersLite.Utilities.Common.UnityAsync;

namespace CustomSabersLite.Menu.Views;

[HotReload(RelativePathToLayout = "../BSML/saberList.bsml")]
[ViewDefinition("CustomSabersLite.Menu.BSML.saberList.bsml")]
internal class SaberListViewController : BSMLAutomaticViewController
{
    [Inject] private readonly PluginConfig config = null!;
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
    [UIComponent("favourite-toggle")] private readonly FavouriteToggle favouriteToggle = null!;

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

    public string OrderByFormatter(string value) => !Enum.TryParse<OrderBy>(value, out var orderBy) ? string.Empty
        : orderBy switch
        {
            OrderBy.Name => "Name",
            OrderBy.Author => "Author",
            OrderBy.RecentlyAdded => "Most Recent",
            _ => throw new ArgumentOutOfRangeException(nameof(value))
        };

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

    public void ListSelected(SegmentedControl segmentedControl, int idx)
    {
        currentSaberList = (SaberListType)idx;
        RefreshList();
    }

    public async void ListCellSelected(TableView tableView, int row)
    {
        if (!saberListManager.TrySelectSorted(row, out var saberListCell)) return;
        if (saberListCell.TryGetCellDirectory(out var directoryInfo))
        {
            saberListManager.OpenFolder(directoryInfo);
            RefreshList();
        }
        else if (saberListCell is ListFavouritesCellInfo)
        {
            saberListManager.ShowFavourites = true;
            RefreshList();
        }
        else if (saberListCell.TryGetSaberValue(out var saberValue))
        {
            SelectedSaberValue = saberValue;
            favouriteToggle.Interactable = saberValue is SaberHash;
        }
        else
        {
            favouriteToggle.Interactable = false;
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
        get => SelectedSaberValue.TryGetSaberHash(out var saberHash) 
               && saberMetadataCache.TryGetMetadata(saberHash.Hash, out var meta)  
               && favouritesManager.IsFavourite(meta.SaberFile);
        set
        {
            if (!SelectedSaberValue.TryGetSaberHash(out var saberHash)
                || !saberMetadataCache.TryGetMetadata(saberHash.Hash, out var meta)) return;
            
            meta = meta with { IsFavourite = value };
            saberMetadataCache.Remove(meta.SaberFile.Hash);
            saberMetadataCache.TryAdd(meta);
            
            if (saberList.Data.TryGetElementAt(saberListManager.IndexForSaberValue(saberHash), out var cell)
                && cell is ListInfoCellInfo infoCell) infoCell.IsFavourite = value;
            
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
        if (SelectedSaberValue.TryGetSaberHash(out var saberHash))
        {
            var meta = saberMetadataCache.GetOrDefault(saberHash.Hash);
            deleteSaberModalText.text = meta?.Descriptor.SaberName.FullName ?? "Unknown";
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
        if (!SelectedSaberValue.TryGetSaberHash(out var deletedSaberHash)) return;
        
        int deletedSaberIndex = saberListManager.IndexForSaberValue(deletedSaberHash);
        
        saberListManager.DeleteSaber(deletedSaberHash.Hash);

        if (saberListManager.TrySelectSorted(deletedSaberIndex - 1, out var cell) 
            && cell.TryGetSaberValue(out var saberValue))
        {
            SelectedSaberValue = saberValue;
        }

        RefreshList();
    }

    public void PreviewButtonPressed()
    {
        config.EnableMenuSabers = !config.EnableMenuSabers;
        previewButtonImage.sprite = config.EnableMenuSabers ? PreviewHeldIcon : PreviewStaticIcon;
        previewManager.UpdateActivePreviewAnimated();
    }

    public async void ReloadButtonPressed()
    {
        saberListManager.Refresh();
        saberList.Data.Clear();
        saberList.ReloadDataKeepingPosition();
     
        previewManager.SetPreviewActive(false);
        
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

        if (saberListManager.CurrentListContains(SelectedSaberValue))
        {
            saberList.SelectCellWithIdx(saberListManager.IndexForSaberValue(SelectedSaberValue));
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
            previewManager.SetPreviewActive(true);
            
            saberPreviewTokenSource.CancelThenDispose();
            saberPreviewTokenSource = new();
            await previewManager.GeneratePreview(saberPreviewTokenSource.Token);
        }
        catch (OperationCanceledException) { }
    }

    private SaberValue SelectedSaberValue
    {
        get => currentSaberList switch
        {
            SaberListType.Sabers => config.CurrentlySelectedSaber,
            SaberListType.Trails => config.CurrentlySelectedTrail,
            _ => throw new ArgumentOutOfRangeException(nameof(currentSaberList))
        };
        set
        {
            if (currentSaberList == SaberListType.Sabers) config.CurrentlySelectedSaber = value;
            else config.CurrentlySelectedTrail = value;
        }
    }
    
    protected override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
    {
        base.DidActivate(firstActivation, addedToHierarchy, screenSystemEnabling);

        saberListManager.OpenFolder(directoryManager.CustomSabers);
        RefreshList();
        
        previewManager.SetPreviewActive(true);
    }

    protected override void DidDeactivate(bool removedFromHierarchy, bool screenSystemDisabling)
    {
        base.DidDeactivate(removedFromHierarchy, screenSystemDisabling);
        saberPreviewTokenSource.Cancel();
        previewManager.SetPreviewActive(false);
    }

    protected override void OnDestroy()
    {
        metadataCacheLoader.LoadingProgressChanged -= LoadingProgressChanged;
        saberPreviewTokenSource.Dispose();
        base.OnDestroy();
    }
}