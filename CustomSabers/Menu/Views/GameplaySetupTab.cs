using System;
using System.Collections;
using System.ComponentModel;
using System.Linq;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Components.Settings;
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
    [Inject] private readonly CslConfig config = null!;
    [Inject] private readonly MetadataCacheLoader metadataCacheLoader = null!;
    [Inject] private readonly ICoroutineStarter coroutineStarter = null!;
    [Inject] private readonly SaberListManager saberListManager = null!;

    public event PropertyChangedEventHandler? PropertyChanged;

    [UIComponent("trail-duration")] private readonly ImageView trailDurationIcon = null!; 
    [UIComponent("trail-width")] private readonly ImageView trailWidthIcon = null!;
    [UIComponent("saber-length")] private readonly ImageView saberLengthIcon = null!;
    [UIComponent("saber-width")] private readonly ImageView saberWidthIcon = null!;
    [UIComponent("trail-type")] private readonly ListSetting trailTypeList = null!;
    [UIComponent("saber-list")] private readonly CustomListTableData saberList = null!;

    [UIValue("enabled")]
    private bool Enabled
    {
        get => config.Enabled;
        set => config.Enabled = value;
    }
    
    [UIValue("disable-white-trail")]
    public bool DisableWhiteTrail
    {
        get => config.DisableWhiteTrail;
        set => config.DisableWhiteTrail = value;
    }

    [UIValue("override-trail-duration")]
    public bool OverrideTrailDuration
    {
        get => config.OverrideTrailDuration;
        set => config.OverrideTrailDuration = value;
    }

    [UIValue("trail-duration")]
    public float TrailDuration
    {
        get => config.TrailDuration;
        set => config.TrailDuration = value;
    }

    [UIValue("override-trail-width")]
    public bool OverrideTrailWidth
    {
        get => config.OverrideTrailWidth;
        set => config.OverrideTrailWidth = value;
    }

    [UIValue("trail-width")]
    public float TrailWidth
    {
        get => config.TrailWidth;
        set => config.TrailWidth = value;
    }
    
    [UIValue("override-saber-length")]
    public bool OverrideSaberLength
    {
        get => config.OverrideSaberLength;
        set => config.OverrideSaberLength = value;
    }
    
    [UIValue("saber-length")]
    public float SaberLength
    {
        get => config.SaberLength;
        set => config.SaberLength = value;
    }
    
    [UIValue("override-saber-width")]
    public bool OverrideSaberWidth
    {
        get => config.OverrideSaberWidth;
        set => config.OverrideSaberWidth = value;
    }
    
    [UIValue("saber-width")]
    public float SaberWidth
    {
        get => config.SaberWidth;
        set => config.SaberWidth = value;
    }

    // [UIValue("trail-type-choices")] private List<object> trailTypeChoices = [.. Enum.GetNames(typeof(TrailType))];
    // [UIValue("trail-type")]
    // public string TrailType
    // {
    //     get => config.TrailType.ToString();
    //     set => config.TrailType = Enum.TryParse(value, out TrailType trailType) ? trailType : config.TrailType;
    // }

    [UIValue("enable-custom-events")]
    public bool EnableCustomEvents
    {
        get => config.EnableCustomEvents;
        set => config.EnableCustomEvents = value;
    }

    [UIAction("#post-parse")]
    private void PostParse()
    {
        metadataCacheLoader.LoadingProgressChanged += LoadingProgressChanged;
        
        trailDurationIcon.sprite = CSLResources.TrailDurationIcon;
        trailWidthIcon.sprite = CSLResources.TrailWidthIcon;
        saberLengthIcon.sprite = CSLResources.SaberLengthIcon;
        saberWidthIcon.sprite = CSLResources.SaberWidthIcon;

        trailTypeList.SetDropDownSettingWidth(25, 0);
        
        RefreshList();
    }

    [UIAction("select-saber")]
    private void SelectSaber(TableView tableView, int row)
    {
        config.CurrentlySelectedSaber = saberListManager.SelectFromUnsortedData(row)?.Value;
        Logger.Debug($"Saber selected: {config.CurrentlySelectedSaber ?? "Default"}");
    }
    
    public void RefreshList()
    {
        if (!metadataCacheLoader.CurrentProgress.Completed)
        {
            return;
        }
        
        saberList.Data.Clear();
        saberListManager.GetUnsortedData()
            .Select(info => new CustomListTableData.CustomCellInfo(info.NameText.FullName))
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
        yield return new WaitUntil(() => saberList.gameObject.activeInHierarchy);
        /* wait for some frames */ for (int i = 0; i < 2; i++) yield return null;
        int selectedSaberIndex = saberListManager.IndexForSaberHash(config.CurrentlySelectedSaber);
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
