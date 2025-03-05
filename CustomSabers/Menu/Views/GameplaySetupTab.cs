using System;
using System.Collections;
using System.ComponentModel;
using System.Linq;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using CustomSabersLite.Configuration;
using CustomSabersLite.Utilities.Common;
using CustomSabersLite.Utilities.Extensions;
using CustomSabersLite.Utilities.Services;
using HMUI;
using JetBrains.Annotations;
using UnityEngine;
using Zenject;

namespace CustomSabersLite.Menu.Views;

[UsedImplicitly]
internal class GameplaySetupTab : IDisposable, INotifyPropertyChanged, ISharedSaberSettings
{
    [Inject] private readonly PluginConfig config = null!;
    [Inject] private readonly MetadataCacheLoader metadataCacheLoader = null!;
    [Inject] private readonly ICoroutineStarter coroutineStarter = null!;
    [Inject] private readonly SaberListManager saberListManager = null!;

    public event PropertyChangedEventHandler? PropertyChanged;

    [UIComponent("trail-duration")] private readonly ImageView trailDurationIcon = null!; 
    [UIComponent("trail-width")] private readonly ImageView trailWidthIcon = null!;
    [UIComponent("saber-length")] private readonly ImageView saberLengthIcon = null!;
    [UIComponent("saber-width")] private readonly ImageView saberWidthIcon = null!;
    [UIComponent("saber-list")] private readonly CustomListTableData saberList = null!;

    [UIAction("#post-parse")]
    private void PostParse()
    {
        metadataCacheLoader.LoadingProgressChanged += LoadingProgressChanged;
        
        trailDurationIcon.sprite = CSLResources.TrailDurationIcon;
        trailWidthIcon.sprite = CSLResources.TrailWidthIcon;
        saberLengthIcon.sprite = CSLResources.SaberLengthIcon;
        saberWidthIcon.sprite = CSLResources.SaberWidthIcon;

        RefreshList();
    }
    
    private bool Enabled
    {
        get => config.Enabled;
        set => config.Enabled = value;
    }
    
    public bool DisableWhiteTrail
    {
        get => config.DisableWhiteTrail;
        set => config.DisableWhiteTrail = value;
    }

    public bool OverrideTrailDuration
    {
        get => config.OverrideTrailDuration;
        set => config.OverrideTrailDuration = value;
    }

    public float TrailDuration
    {
        get => config.TrailDuration;
        set => config.TrailDuration = value;
    }

    public bool OverrideTrailWidth
    {
        get => config.OverrideTrailWidth;
        set => config.OverrideTrailWidth = value;
    }

    public float TrailWidth
    {
        get => config.TrailWidth;
        set => config.TrailWidth = value;
    }
    
    public bool OverrideSaberLength
    {
        get => config.OverrideSaberLength;
        set => config.OverrideSaberLength = value;
    }
    
    public float SaberLength
    {
        get => config.SaberLength;
        set => config.SaberLength = value;
    }
    
    public bool OverrideSaberWidth
    {
        get => config.OverrideSaberWidth;
        set => config.OverrideSaberWidth = value;
    }
    
    public float SaberWidth
    {
        get => config.SaberWidth;
        set => config.SaberWidth = value;
    }

    public bool EnableCustomEvents
    {
        get => config.EnableCustomEvents;
        set => config.EnableCustomEvents = value;
    }

    public void SelectSaber(TableView tableView, int row)
    {
        if (saberListManager.SelectFromUnsortedData(row)?.TryGetSaberValue(out var saberValue) ?? false)
        {
            config.CurrentlySelectedSaber = saberValue;
        }
    }
    
    public void RefreshList()
    {
        if (!metadataCacheLoader.CurrentProgress.Completed)
        {
            return;
        }
        
        saberList.Data.Clear();
        saberListManager.UpdateUnsortedList()
            // todo: temporarily converting saber list cells to BSML cells
            .Select(saberListCell => new CustomListTableData.CustomCellInfo(saberListCell.NameText.FullName))
            .ForEach(saberList.Data.Add);
        saberList.TableView.ReloadData();
    }

    public void Activated(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
    {
        ISharedSaberSettings.PropertyNames.ForEach(NotifyPropertyChanged);
        RefreshList();
        coroutineStarter.StartSingleCoroutine(ref scrollToSelectedCellCoroutine, ScrollToSelectedCell());
    }

    private Coroutine? scrollToSelectedCellCoroutine;
    private IEnumerator ScrollToSelectedCell()
    {
        if (!config.CurrentlySelectedSaber.TryGetSaberHash(out var saberHash)) yield break;
        yield return new WaitUntil(() => saberList.gameObject.activeInHierarchy);
        /* wait for some frames */ for (int i = 0; i < 2; i++) yield return null;
        int selectedSaberIndex = saberListManager.IndexForSaberValue(saberHash);
        saberList.TableView.SelectCellWithIdx(selectedSaberIndex);
        saberList.TableView.ScrollToCellWithIdx(selectedSaberIndex, TableView.ScrollPositionType.Center, true);
    }

    private void LoadingProgressChanged(MetadataCacheLoader.Progress progress)
    {
        if (progress.Completed) RefreshList();
    }

    private void NotifyPropertyChanged(string propertyName) =>
        PropertyChanged?.Invoke(this, new(propertyName));

    public void Dispose() =>
        metadataCacheLoader.LoadingProgressChanged -= LoadingProgressChanged;
}
