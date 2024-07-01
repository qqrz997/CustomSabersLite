using BeatSaberMarkupLanguage.Attributes;
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
using System.Text.RegularExpressions;
using System.ComponentModel;
using CustomSabersLite.Utilities.UI;
using System.Collections;

namespace CustomSabersLite.UI.Views;

internal class GameplaySetupTab(CSLConfig config, CacheManager cacheManager, ICoroutineStarter coroutineStarter) : IInitializable, IDisposable, INotifyPropertyChanged
{
    private readonly CSLConfig config = config;
    private readonly CacheManager cacheManager = cacheManager;
    private readonly ICoroutineStarter coroutineStarter = coroutineStarter;

    public GameObject Root;

    public event PropertyChangedEventHandler PropertyChanged;

    private readonly string resourceName = "CustomSabersLite.UI.BSML.gameplaySetup.bsml";

    public void Initialize()
    {
        Logger.Debug("Creating tab");
        GameplaySetup.instance.AddTab("Custom Sabers", resourceName, this);
    }

    public void Dispose()
    {
        cacheManager.LoadingComplete -= SetupList;
        GameplaySetup.instance.RemoveTab("Custom Sabers");
    }

    #region trail settings

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
        set
        {
            config.OverrideTrailDuration = value;
            BSMLHelpers.SetSliderInteractable(trailDurationInteractable, value);
        }
    }

    [UIValue("trail-duration")]
    public int TrailDuration
    {
        get => config.TrailDuration;
        set => config.TrailDuration = value;
    }

    [UIValue("override-trail-width")]
    public bool OverrideTrailWidth
    {
        get => config.OverrideTrailWidth;
        set
        {
            config.OverrideTrailWidth = value;
            BSMLHelpers.SetSliderInteractable(trailWidthInteractable, value);
        }
    }

    [UIValue("trail-width")]
    public int TrailWidth
    {
        get => config.TrailWidth;
        set => config.TrailWidth = value;
    }

    [UIValue("trail-type")]
    public string TrailType
    {
        get => config.TrailType.ToString();
        set => config.TrailType = Enum.TryParse(value, out TrailType trailType) ? trailType : config.TrailType;
    }

    [UIValue("trail-type-list")]
    public List<object> trailTypeList = Enum.GetNames(typeof(TrailType)).ToList<object>();

    #endregion

    #region saber settings

    [UIValue("enable-custom-events")]
    public bool EnableCustomEvents
    {
        get => config.EnableCustomEvents;
        set => config.EnableCustomEvents = value;
    }

    [UIValue("enable-custom-color-scheme")]
    public bool EnableCustomColorScheme
    {
        get => config.EnableCustomColorScheme;
        set => config.EnableCustomColorScheme = value;
    }

    [UIValue("left-saber-color")]
    public Color LeftSaberColor
    {
        get => config.LeftSaberColor;
        set => config.LeftSaberColor = value;
    }

    [UIValue("right-saber-color")]
    public Color RightSaberColor
    {
        get => config.RightSaberColor;
        set => config.RightSaberColor = value;
    }

    [UIValue("forcefully-foolish")]
    public bool ForcefullyFoolish
    {
        get => config.ForcefullyFoolish;
        set => config.ForcefullyFoolish = value;
    }

    #endregion

    [UIComponent("trail-duration")]
    private GenericInteractableSetting trailDurationInteractable;

    [UIComponent("trail-duration")]
    private TextMeshProUGUI trailDurationText;

    [UIComponent("trail-width")]
    private GenericInteractableSetting trailWidthInteractable;

    [UIComponent("trail-width")]
    private TextMeshProUGUI trailWidthText;

    [UIComponent("forcefully-foolish")]
    private Transform foolishSetting;

    [UIComponent("trail-type")]
    private RectTransform trailTypeRT;

    [UIComponent("saber-list")]
    public CustomListTableData saberList;

    [UIComponent("saber-list-loading")]
    public ImageView saberListLoadingIcon;

    [UIAction("select-saber")]
    public void Select(TableView _, int row)
    {
        Logger.Debug($"saber selected at row {row}");
        cacheManager.SelectedSaberIndex = row;
        config.CurrentlySelectedSaber = cacheManager.SabersMetadata[row].RelativePath;
    }

    #region tabs
    [UIComponent("settings-title")]
    TextMeshProUGUI settingsTitleText;

    [UIComponent("trail-settings-panel")]
    RectTransform trailSettingsPanel;

    [UIComponent("saber-settings-panel")]
    RectTransform saberSettingsPanel;

    private enum ActiveSettingsTab
    {
        None,
        Trail,
        Saber
    }

    private ActiveSettingsTab activeSettingsTab = ActiveSettingsTab.None;

    [UIAction("show-trail-settings")]
    public void ShowTrailSettings()
    {
        if (!activeSettingsTab.Equals(ActiveSettingsTab.Trail))
        {
            activeSettingsTab = ActiveSettingsTab.Trail;
            settingsTitleText.text = "Trail Settings";
            saberSettingsPanel.gameObject.SetActive(false);
            trailSettingsPanel.gameObject.SetActive(true);
        }
    }

    [UIAction("show-saber-settings")]
    public void ShowSaberSettings()
    {
        if (!activeSettingsTab.Equals(ActiveSettingsTab.Saber))
        {
            activeSettingsTab = ActiveSettingsTab.Saber;
            settingsTitleText.text = "Saber Settings";
            trailSettingsPanel.gameObject.SetActive(false);
            saberSettingsPanel.gameObject.SetActive(true);
        }
    }
    #endregion

    [UIAction("#post-parse")]
    public void PostParse()
    {
        Root = saberList.gameObject;

        BSMLHelpers.SetSliderInteractable(trailDurationInteractable, OverrideTrailDuration);
        BSMLHelpers.SetSliderInteractable(trailWidthInteractable, OverrideTrailWidth);

        // Saber Trail Type list setting 
        var trailTypePickerRect = trailTypeRT.gameObject.transform.Find("ValuePicker").GetComponent<RectTransform>();
        var trailTypeTextRect = trailTypeRT.gameObject.transform.Find("NameText").GetComponent<RectTransform>();
        trailTypePickerRect.sizeDelta = new(30, trailTypePickerRect.sizeDelta.y);
        trailTypeTextRect.sizeDelta = new(0, trailTypeTextRect.sizeDelta.y);

        // Trail duration and width slider setting
        var trailDurationRect = trailDurationText.transform.parent.transform.Find("BSMLSlider").GetComponent<RectTransform>();
        var trailWidthRect = trailWidthText.transform.parent.transform.Find("BSMLSlider").GetComponent<RectTransform>();
        trailDurationRect.sizeDelta = new(50, trailDurationRect.sizeDelta.y);
        trailWidthRect.sizeDelta = new(50, trailWidthRect.sizeDelta.y);

        if (cacheManager.InitializationFinished) SetupList();
        else cacheManager.LoadingComplete += SetupList;
    }

    public void SetupList()
    {
        saberList.data.Clear();
        var richTextRegex = new Regex(@"<[^>]*>");

        foreach (var metadata in cacheManager.SabersMetadata)
        {
            if (metadata.LoadingError != SaberLoaderError.None)
                continue;

            var saberName = richTextRegex.Replace(metadata.SaberName, string.Empty);

            var maxLength = 24;

            if (saberName.Length > maxLength)
                saberName = saberName.Substring(0, maxLength - 1).Trim() + "...";

            saberList.data.Add(new(saberName));
        }

        saberListLoadingIcon.gameObject.SetActive(false);
        saberList.tableView.ReloadData();
    }

    private bool firstActivation = true;

    public void Activated()
    {
        if (firstActivation)
        {
            ShowTrailSettings();
            firstActivation = false;
        }

        foreach (var name in SharedSaberSettings.PropertyNames)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        if (config.Fooled)
        {
            foolishSetting.gameObject.SetActive(true);
        }

        coroutineStarter.StartCoroutine(ScrollToSelectedCell());
    }

    private IEnumerator ScrollToSelectedCell()
    {
        yield return new WaitUntil(() => saberList.gameObject.activeInHierarchy);
        yield return new WaitForEndOfFrame();
        var selectedSaber = cacheManager.SelectedSaberIndex;
        saberList.tableView.SelectCellWithIdx(selectedSaber);
        saberList.tableView.ScrollToCellWithIdx(selectedSaber, TableView.ScrollPositionType.Center, true);
    }
}
