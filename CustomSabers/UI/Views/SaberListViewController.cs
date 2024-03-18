using System;
using System.Linq;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.ViewControllers;
using HMUI;
using CustomSabersLite.Data;
using CustomSabersLite.Utilities;
using CustomSabersLite.Configuration;
using UnityEngine;
using UnityEngine.UIElements;
using System.IO;
using BeatSaberMarkupLanguage.Components.Settings;
using UnityEngine.UI;
using TMPro;
using System.Diagnostics;
using BeatSaberMarkupLanguage.TypeHandlers;
using Zenject;

namespace CustomSabersLite.UI
{
    internal class SaberListViewController : BSMLResourceViewController
    {
        private PluginDirs pluginDirs;
        private CSLConfig config;
        private CSLAssetLoader assetLoader;

        [Inject]
        public void Construct(PluginDirs pluginDirs, CSLConfig config, CSLAssetLoader assetLoader)
        {
            this.pluginDirs = pluginDirs;
            this.config = config;
            this.assetLoader = assetLoader;
            Init();
        }

        private string saberAssetPath;
        private string deletedSabersPath;

        private void Init()
        {
            saberAssetPath = pluginDirs.CustomSabers.FullName;
            deletedSabersPath = pluginDirs.DeletedSabers.FullName;
        }

        public override string ResourceName => "CustomSabersLite.UI.BSML.saberList.bsml";

        private static readonly Sprite nullCoverImage = CSLUtils.GetNullCoverImage();

        [UIComponent("saber-list")]
        public CustomListTableData customListTableData;

        [UIComponent("reload-button")]
        public Selectable reloadButtonSelectable;

        [UIComponent("delete-saber-modal")]
        public ModalView deleteSaberModal;

        [UIComponent("delete-saber-modal-text")]
        public TextMeshProUGUI deleteSaberModalText;

        [UIAction("select-saber")]
        public void Select(TableView _, int row)
        {
            Logger.Debug($"saber selected at row {row}");
            assetLoader.SelectedSaberIndex = row;
            config.CurrentlySelectedSaber = assetLoader.SabersMetadata[row].SaberFileName;

            // currently loading saber on game load, probably should do it on saber select instead
            // that can be used for saber previewing
        }

        [UIAction("open-in-explorer")]
        public void OpenInExplorer()
        {
            Process.Start("explorer.exe", saberAssetPath);
        }

        [UIAction("show-delete-saber-modal")]
        public void ShowDeleteSaberModal()
        {
            if (config.CurrentlySelectedSaber == "Default")
            {
                Logger.Warn("You can't delete the default sabers! >:(");
                return;
            }

            deleteSaberModalText.text = $"Are you sure you want to delete\n{ Path.GetFileNameWithoutExtension(config.CurrentlySelectedSaber) }?";
            deleteSaberModal.Show(true);
        }

        [UIAction("hide-delete-saber-modal")]
        public void HideDeleteSaberModal()
        {
            deleteSaberModal.Hide(true);
        }

        [UIAction("delete-selected-saber")]
        public void DeleteSelectedSaber()
        {
            HideDeleteSaberModal();

            string selectedSaber = config.CurrentlySelectedSaber;
            if (selectedSaber != "Default")
            {
                string currentSaberPath = Path.Combine(saberAssetPath, selectedSaber);
                string destinationPath = Path.Combine(deletedSabersPath, selectedSaber);
                try
                {
                    if (File.Exists(destinationPath)) File.Delete(destinationPath);

                    if (File.Exists(currentSaberPath))
                    {
                        Logger.Debug($"Moving {currentSaberPath}\nto {deletedSabersPath}");

                        File.Move(currentSaberPath, destinationPath);

                        assetLoader.SabersMetadata.RemoveAt(assetLoader.SelectedSaberIndex);

                        Select(customListTableData.tableView, assetLoader.SelectedSaberIndex - 1);

                        SetupList();
                    }
                    else
                    {
                        Logger.Warn($"Saber doesn't exist at {currentSaberPath}");
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error("Problem encountered when trying to delete selected saber");
                    Logger.Error(ex.ToString());
                }
            }
        }

        [UIAction("reload-sabers")]
        public async void ReloadSabers()
        {
            reloadButtonSelectable.interactable = false;
            await assetLoader.ReloadAsync();
            SetupList();
            Select(customListTableData.tableView, assetLoader.SelectedSaberIndex);
            reloadButtonSelectable.interactable = true;
        }

        [UIAction("#post-parse")]
        public void PostParse()
        {
            SetupList();
        }

        private void SetupList() // todo - smoother saber list refresh
        {
            customListTableData.data.Clear();

            Logger.Debug("Showing list of selectable sabers");

            for (int i = 0; i < assetLoader.SabersMetadata.Count; i++)
            {
                CustomSaberMetadata metadata = assetLoader.SabersMetadata[i];
                
                if (metadata.SaberFileName != null)
                {
                    if (!File.Exists(Path.Combine(saberAssetPath, metadata.SaberFileName))) continue;
                }

                Logger.Debug($"#{i+1} \"{metadata.SaberName}\"");

                Sprite cover = null;
                if (metadata.CoverImage != null)
                {
                    Texture2D tex = new Texture2D(2, 2);
                    tex.LoadImage(metadata.CoverImage);
                    cover = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);
                }
                else
                {
                    cover = nullCoverImage;
                }

                if (metadata.SaberName == "Default") cover = CSLUtils.GetDefaultCoverImage(); 

                var customCellInfo = new CustomListTableData.CustomCellInfo(metadata.SaberName, metadata.AuthorName, cover);
                customListTableData.data.Add(customCellInfo);
            }

            customListTableData.tableView.ReloadData();

            int selectedSaber = assetLoader.SelectedSaberIndex;
            customListTableData.tableView.SelectCellWithIdx(selectedSaber);

            if (!customListTableData.tableView.visibleCells.Where(x => x.selected).Any())
            {
                customListTableData.tableView.ScrollToCellWithIdx(selectedSaber, TableView.ScrollPositionType.Beginning, true);
            }
        }
    }
}
