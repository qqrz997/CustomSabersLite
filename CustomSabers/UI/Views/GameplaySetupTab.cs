using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components.Settings;
using CustomSabersLite.Configuration;
using CustomSabersLite.Models;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Zenject;
using BeatSaberMarkupLanguage.Components;
using HMUI;
using System.ComponentModel;
using System.Collections;
using CustomSabersLite.Utilities;

namespace CustomSabersLite.UI.Views;

internal class GameplaySetupTab : IDisposable, INotifyPropertyChanged
{
    [Inject] private readonly CSLConfig config = null!;
    [Inject] private readonly SaberMetadataCache saberMetadataCache = null!;
    [Inject] private readonly ICoroutineStarter coroutineStarter = null!;
    [Inject] private readonly SaberListManager saberListManager = null!;

    public event PropertyChangedEventHandler? PropertyChanged;

    [UIComponent("trail-duration")] private readonly SliderSetting trailDurationSlider = null!;
    [UIComponent("trail-width")] private readonly SliderSetting trailWidthSlider = null!;
    [UIComponent("trail-type")] private readonly ListSetting trailTypeList = null!;
    [UIComponent("saber-list")] private readonly CustomListTableData saberList = null!;

    [UIValue("disable-white-trail")]
    private bool DisableWhiteTrail
    {
        get => config.DisableWhiteTrail;
        set => config.DisableWhiteTrail = value;
    }

    [UIValue("override-trail-duration")]
    private bool OverrideTrailDuration
    {
        get => config.OverrideTrailDuration;
        set
        {
            config.OverrideTrailDuration = value;
            BSMLHelpers.SetSliderInteractable(trailDurationSlider, value);
        }
    }

    [UIValue("trail-duration")]
    private int TrailDuration
    {
        get => config.TrailDuration;
        set => config.TrailDuration = value;
    }

    [UIValue("override-trail-width")]
    private bool OverrideTrailWidth
    {
        get => config.OverrideTrailWidth;
        set
        {
            config.OverrideTrailWidth = value;
            BSMLHelpers.SetSliderInteractable(trailWidthSlider, value);
        }
    }

    [UIValue("trail-width")]
    private int TrailWidth
    {
        get => config.TrailWidth;
        set => config.TrailWidth = value;
    }

    [UIValue("trail-type-choices")] private List<object> trailTypeChoices = [.. Enum.GetNames(typeof(TrailType))];
    [UIValue("trail-type")]
    private string TrailType
    {
        get => config.TrailType.ToString();
        set => config.TrailType = Enum.TryParse(value, out TrailType trailType) ? trailType : config.TrailType;
    }

    [UIValue("enable-custom-events")]
    private bool EnableCustomEvents
    {
        get => config.EnableCustomEvents;
        set => config.EnableCustomEvents = value;
    }

    [UIAction("#post-parse")]
    private void PostParse()
    {
        saberMetadataCache.LoadingProgressChanged += LoadingProgressChanged;

        BSMLHelpers.SetSliderInteractable(trailDurationSlider, OverrideTrailDuration);
        BSMLHelpers.SetSliderInteractable(trailWidthSlider, OverrideTrailWidth);

        BSMLHelpers.SetSliderWidth(trailDurationSlider, 50, 0);
        BSMLHelpers.SetSliderWidth(trailWidthSlider, 50, 0);

        BSMLHelpers.SetDropDownSettingWidth(trailTypeList, 25, 0);
        
        RefreshList();
    }

    [UIAction("select-saber")]
    private void SelectSaber(TableView tableView, int row)
    {
        config.CurrentlySelectedSaber = saberListManager.Select(row)?.Metadata.SaberFile.RelativePath;
        Logger.Debug($"Saber selected: {config.CurrentlySelectedSaber ?? "Default"}");
    }

    public void RefreshList()
    {
        if (!saberMetadataCache.CurrentProgress.Completed)
        {
            return;
        }
        
        saberList.Data = saberListManager.UpdateList(SaberListFilterOptions.Default).Select(i => i.ToCustomCellInfo()).ToList();
        saberList.TableView.ReloadData();
    }

    public void Activated(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
    {
        SharedSaberSettings.PropertyNames.ForEach(NotifyPropertyChanged);
        RefreshList();
        coroutineStarter.StartSingleCoroutine(ref scrollToSelectedCellCoroutine, ScrollToSelectedCell());
    }

    private Coroutine? scrollToSelectedCellCoroutine;
    private IEnumerator ScrollToSelectedCell()
    {
        yield return new WaitUntil(() => saberList.gameObject.activeInHierarchy);
        /* wait for some frames */ for (int i = 0; i < 2; i++) yield return null;
        int selectedSaberIndex = saberListManager.IndexForPath(config.CurrentlySelectedSaber);
        saberList.TableView.SelectCellWithIdx(selectedSaberIndex);
        saberList.TableView.ScrollToCellWithIdx(selectedSaberIndex, TableView.ScrollPositionType.Center, true);
    }

    private void LoadingProgressChanged(SaberMetadataCache.Progress progress)
    {
        if (progress.Completed) RefreshList();
    }

    private void NotifyPropertyChanged(string propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    public void Dispose() =>
        saberMetadataCache.LoadingProgressChanged -= LoadingProgressChanged;
}
