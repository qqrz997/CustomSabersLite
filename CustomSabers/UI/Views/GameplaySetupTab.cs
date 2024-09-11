﻿using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components.Settings;
using BeatSaberMarkupLanguage.GameplaySetup;
using CustomSabersLite.Configuration;
using CustomSabersLite.Data;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;
using Zenject;
using BeatSaberMarkupLanguage.Components;
using CustomSabersLite.Utilities.AssetBundles;
using HMUI;
using System.ComponentModel;
using System.Collections;
using CustomSabersLite.UI.Managers;
using CustomSabersLite.Utilities;
using CustomSabersLite.Models;

namespace CustomSabersLite.UI.Views;

internal class GameplaySetupTab : IInitializable, IDisposable, INotifyPropertyChanged
{
    [Inject] private readonly CSLConfig config;
    [Inject] private readonly CacheManager cacheManager;
    [Inject] private readonly ICoroutineStarter coroutineStarter;
    [Inject] private readonly SaberListManager saberListManager;

    public GameObject Root { get; private set; }

    public event PropertyChangedEventHandler PropertyChanged;

    private readonly string resourceName = "CustomSabersLite.UI.BSML.gameplaySetup.bsml";
    private int selectedSaberIndex;

    public void Initialize()
    {
        Logger.Debug("Creating tab");
        GameplaySetup.Instance.AddTab("Custom Sabers", resourceName, this);
    }

    public void Dispose()
    {
        cacheManager.LoadingComplete -= SetupList;
        GameplaySetup.Instance.RemoveTab("Custom Sabers");
    }

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

    [UIValue("trail-type-choices")] private List<object> trailTypeChoices = Enum.GetNames(typeof(TrailType)).ToList<object>();
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

    [UIComponent("trail-duration")] private readonly SliderSetting trailDurationSlider;
    [UIComponent("trail-duration")] private readonly TextMeshProUGUI trailDurationText;
    [UIComponent("trail-width")] private readonly SliderSetting trailWidthSlider;
    [UIComponent("trail-width")] private readonly TextMeshProUGUI trailWidthText;
    [UIComponent("trail-type")] private readonly RectTransform trailTypeRT;
    [UIComponent("saber-list")] private readonly CustomListTableData saberList;
    
    [UIAction("#post-parse")]
    private void PostParse()
    {
        Root = saberList.gameObject;

        BSMLHelpers.SetSliderInteractable(trailDurationSlider, OverrideTrailDuration);
        BSMLHelpers.SetSliderInteractable(trailWidthSlider, OverrideTrailWidth);

        // Saber Trail Type list setting 
        var trailTypePickerRect = trailTypeRT.gameObject.transform.Find("ValuePicker").GetComponent<RectTransform>();
        var trailTypeTextRect = trailTypeRT.gameObject.transform.Find("NameText").GetComponent<RectTransform>();
        trailTypePickerRect.sizeDelta = trailTypePickerRect.sizeDelta with { x = 30 };
        trailTypeTextRect.sizeDelta = trailTypeTextRect.sizeDelta with { x = 0 };

        // Trail duration and width slider setting
        var trailDurationRect = trailDurationText.transform.parent.transform.Find("BSMLSlider").GetComponent<RectTransform>();
        var trailWidthRect = trailWidthText.transform.parent.transform.Find("BSMLSlider").GetComponent<RectTransform>();
        trailDurationRect.sizeDelta = trailDurationRect.sizeDelta with { x = 50 };
        trailWidthRect.sizeDelta = trailWidthRect.sizeDelta with { x = 50 };

        if (cacheManager.InitializationFinished) SetupList();
        else cacheManager.LoadingComplete += SetupList;
    }

    [UIAction("select-saber")]
    private void SelectSaber(TableView tableView, int row)
    {
        Logger.Debug($"saber selected at row {row}");
        selectedSaberIndex = row;
        config.CurrentlySelectedSaber = saberListManager.PathForIndex(row);
    }

    public void SetupList()
    {
        var filterOptions = new SaberListFilterOptions(
            OrderBy.Name);

        saberList.Data.Clear();
        saberListManager.GetList(filterOptions)
            .ForEach(i => saberList.Data.Add(i.ToCustomCellInfo()));

        saberList.TableView.ReloadData();
        saberList.TableView.SelectCellWithIdx(selectedSaberIndex);
    }

    public void Activated()
    {
        SharedSaberSettings.PropertyNames.ForEach(n => PropertyChanged(this, new(n)));

        selectedSaberIndex = saberListManager.IndexForPath(config.CurrentlySelectedSaber);

        saberList.TableView.SelectCellWithIdx(selectedSaberIndex);
        coroutineStarter.StartCoroutine(ScrollToSelectedCell());
    }

    private IEnumerator ScrollToSelectedCell()
    {
        yield return new WaitUntil(() => saberList.gameObject.activeInHierarchy);
        yield return new WaitForEndOfFrame();
        saberList.TableView.ScrollToCellWithIdx(selectedSaberIndex, TableView.ScrollPositionType.Center, true);
    }
}
