﻿using System;
using System.Collections;
using System.ComponentModel;
using BeatSaberMarkupLanguage.Attributes;
using CustomSabersLite.Configuration;
using CustomSabersLite.Menu.Components;
using CustomSabersLite.Models;
using CustomSabersLite.Services;
using CustomSabersLite.Utilities.Extensions;
using HMUI;
using JetBrains.Annotations;
using UnityEngine;

namespace CustomSabersLite.Menu.Views;

[UsedImplicitly]
internal class GameplaySetupTab : IDisposable, INotifyPropertyChanged, ISharedSaberSettings
{
    private readonly PluginConfig config;
    private readonly MetadataCacheLoader metadataCacheLoader;
    private readonly ICoroutineStarter coroutineStarter;
    private readonly SaberListManager saberListManager;

    [UIComponent("saber-list")] private readonly SaberListTableData saberList = null!;

    public GameplaySetupTab(
        PluginConfig config,
        MetadataCacheLoader metadataCacheLoader,
        ICoroutineStarter coroutineStarter,
        SaberListManager saberListManager)
    {
        this.config = config;
        this.metadataCacheLoader = metadataCacheLoader;
        this.coroutineStarter = coroutineStarter;
        this.saberListManager = saberListManager;
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    private Coroutine? scrollToSelectedCellCoroutine;

    [UIAction("#post-parse")]
    private void PostParse()
    {
        metadataCacheLoader.LoadingProgressChanged += LoadingProgressChanged;
        saberList.DidActivate += Activated;
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

    public void ListCellSelected(TableView tableView, int row)
    {
        if (!saberListManager.TrySelectUnsorted(row, out var saberListCell)) return;
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
            // todo: consider introducing trail selection to the tab 
            config.CurrentlySelectedSaber = saberValue;
        }
    }

    private void RefreshList()
    {
        saberList.Data.Clear();
        saberList.Data.AddRange(saberListManager.UpdateUnsortedList());
        saberList.ReloadData();

        // todo: consider introducing trail selection to the tab 
        if (saberListManager.UnsortedListContains(config.CurrentlySelectedSaber))
            saberList.SelectCellWithIdx(saberListManager.IndexForSaberValueUnsorted(config.CurrentlySelectedSaber));
        else
            saberList.ClearSelection();
    }

    private void Activated()
    {
        ISharedSaberSettings.PropertyNames.ForEach(NotifyPropertyChanged);
        RefreshList();
        coroutineStarter.StartSingleCoroutine(ref scrollToSelectedCellCoroutine, ScrollToSelectedCell());
    }
    
    private IEnumerator ScrollToSelectedCell()
    {
        if (!config.CurrentlySelectedSaber.TryGetSaberHash(out var saberHash)) yield break;
        int selectedSaberIndex = saberListManager.IndexForSaberValueUnsorted(saberHash);
        saberList.SelectCellWithIdx(selectedSaberIndex);
        saberList.ScrollToCellWithIdx(selectedSaberIndex, TableView.ScrollPositionType.Center, true);
    }

    private void LoadingProgressChanged(MetadataCacheLoader.Progress progress)
    {
        if (progress.Completed) RefreshList();
    }

    private void NotifyPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new(propertyName));
    }

    public void Dispose()
    {
        metadataCacheLoader.LoadingProgressChanged -= LoadingProgressChanged;
        if (saberList != null) saberList.DidActivate -= Activated;
    }
}
