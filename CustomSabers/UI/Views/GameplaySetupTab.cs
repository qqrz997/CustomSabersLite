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
using System.IO;
using HMUI;
using System.Text.RegularExpressions;
using System.ComponentModel;
using System.Threading.Tasks;
using CustomSabersLite.Utilities.UI;

namespace CustomSabersLite.UI.Views
{
    internal class GameplaySetupTab : IInitializable, IDisposable, INotifyPropertyChanged
    {
        private readonly PluginDirs pluginDirs;
        private readonly CSLConfig config;
        private readonly CSLAssetLoader assetLoader;

        public GameplaySetupTab(PluginDirs pluginDirs, CSLConfig config, CSLAssetLoader assetLoader)
        {
            this.pluginDirs = pluginDirs;
            this.config = config;
            this.assetLoader = assetLoader;
        }

        public GameObject Root;

        public event PropertyChangedEventHandler PropertyChanged;

        private readonly string resourceName = "CustomSabersLite.UI.BSML.gameplaySetup.bsml";
        private bool parsed;
        private string saberAssetPath;

        public void Initialize()
        {
            saberAssetPath = pluginDirs.CustomSabers.FullName;

            Logger.Debug("Creating tab");
            GameplaySetup.instance.AddTab("Custom Sabers", resourceName, this);
        }

        public void Dispose()
        {
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

        [UIAction("select-saber")]
        public void Select(TableView _, int row)
        {
            Logger.Debug($"saber selected at row {row}");
            assetLoader.SelectedSaberIndex = row;
            config.CurrentlySelectedSaber = assetLoader.SabersMetadata[row].RelativePath;
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
            parsed = true;
            Root = saberList.gameObject;

            BSMLHelpers.SetSliderInteractable(trailDurationInteractable, OverrideTrailDuration);
            BSMLHelpers.SetSliderInteractable(trailWidthInteractable, OverrideTrailWidth);

            // Saber Trail Type list setting 
            RectTransform trailTypePickerRect = trailTypeRT.gameObject.transform.Find("ValuePicker").GetComponent<RectTransform>();
            RectTransform trailTypeTextRect = trailTypeRT.gameObject.transform.Find("NameText").GetComponent<RectTransform>();
            trailTypePickerRect.sizeDelta = new Vector2(30, trailTypePickerRect.sizeDelta.y);
            trailTypeTextRect.sizeDelta = new Vector2(0, trailTypeTextRect.sizeDelta.y);

            // Trail duration and width slider setting
            RectTransform trailDurationRect = trailDurationText.transform.parent.transform.Find("BSMLSlider").GetComponent<RectTransform>();
            RectTransform trailWidthRect = trailWidthText.transform.parent.transform.Find("BSMLSlider").GetComponent<RectTransform>();
            trailDurationRect.sizeDelta = new Vector2(50, trailDurationRect.sizeDelta.y);
            trailWidthRect.sizeDelta = new Vector2(50, trailWidthRect.sizeDelta.y);

            SetupList();
        }

        public void SetupList()
        {
            saberList.data.Clear();

            foreach (CustomSaberMetadata metadata in assetLoader.SabersMetadata)
            {
                string saberName = metadata.SaberName;

                if (metadata.RelativePath != null && metadata.RelativePath != "Default")
                {
                    if (!File.Exists(Path.Combine(saberAssetPath, metadata.RelativePath)))
                    {
                        continue;
                    }
                }

                // Remove TMPro rich text tags
                Regex regex = new Regex(@"<[^>]*>");
                if (regex.IsMatch(saberName))
                {
                    saberName = regex.Replace(saberName, string.Empty);
                    saberName.Trim();
                }

                int maxLength = 21;
                saberName = saberName.Length <= maxLength ? saberName : saberName.Substring(0, maxLength - 1).Trim() + "...";

                var customCellInfo = new CustomListTableData.CustomCellInfo(saberName);
                saberList.data.Add(customCellInfo);
            }

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

            foreach (string name in SharedSaberSettings.PropertyNames)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
            }

            if (config.Fooled)
            {
                foolishSetting.gameObject.SetActive(true);
            }

            Task.Run(() => ScrollToSelectedCell());
        }

        private async void ScrollToSelectedCell()
        {
            while (!saberList.gameObject.activeInHierarchy)
            {
                await Task.Delay(25);
            }
            await Task.Delay(100);
            int selectedSaber = assetLoader.SelectedSaberIndex;
            saberList.tableView.SelectCellWithIdx(selectedSaber);
            saberList.tableView.ScrollToCellWithIdx(selectedSaber, TableView.ScrollPositionType.Center, true);
        }
    }
}
