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
        public override string ResourceName => "CustomSabersLite.UI.BSML.saberList.bsml";

        private PluginDirs pluginDirs;

        public SaberListViewController(PluginDirs pluginDirs)
        {
            this.pluginDirs = pluginDirs;
        }

        private string saberAssetPath;
        private string deletedSabersPath;

        public void Start()
        {
            saberAssetPath = pluginDirs.CustomSabers.FullName;
            deletedSabersPath = pluginDirs.DeletedSabers.FullName;
        }

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
            Plugin.Log.Debug($"saber selected at row {row}");
            CSLAssetLoader.SelectedSaberIndex = row;
            CSLConfig.Instance.CurrentlySelectedSaber = CSLAssetLoader.SabersMetadata[row].SaberFileName;

            // currently loading saber on game load, probably should do it on saber select instead
            // that can be used for saber previewing
        }

        [UIAction("open-in-explorer")]
        public void OpenInExplorer()
        {
            System.Diagnostics.Process.Start("explorer.exe", saberAssetPath);
        }

        [UIAction("show-delete-saber-modal")]
        public void ShowDeleteSaberModal()
        {
            if (CSLConfig.Instance.CurrentlySelectedSaber == "Default")
            {
                Plugin.Log.Warn("You can't delete the default sabers! >:(");
                return;
            }

            deleteSaberModalText.text = $"Are you sure you want to delete\n{ Path.GetFileNameWithoutExtension(CSLConfig.Instance.CurrentlySelectedSaber) }?";
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

            string selectedSaber = CSLConfig.Instance.CurrentlySelectedSaber;
            if (selectedSaber != "Default")
            {
                string currentSaberPath = Path.Combine(saberAssetPath, selectedSaber);
                string destinationPath = Path.Combine(deletedSabersPath, selectedSaber);
                try
                {
                    if (File.Exists(destinationPath)) File.Delete(destinationPath);

                    if (File.Exists(currentSaberPath))
                    {
                        Plugin.Log.Debug($"Moving {currentSaberPath}\nto {deletedSabersPath}");

                        File.Move(currentSaberPath, destinationPath);

                        CSLAssetLoader.SabersMetadata.RemoveAt(CSLAssetLoader.SelectedSaberIndex);

                        Select(customListTableData.tableView, CSLAssetLoader.SelectedSaberIndex - 1);

                        SetupList();
                    }
                    else
                    {
                        Plugin.Log.Warn($"Saber doesn't exist at {currentSaberPath}");
                    }
                }
                catch (Exception ex)
                {
                    Plugin.Log.Error("Problem encountered when trying to delete selected saber");
                    Plugin.Log.Error(ex);
                }
            }
        }

        [UIAction("reload-sabers")]
        public async void ReloadSabers()
        {
            reloadButtonSelectable.interactable = false;
            await CSLAssetLoader.ReloadAsync();
            SetupList();
            Select(customListTableData.tableView, CSLAssetLoader.SelectedSaberIndex);
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

            Plugin.Log.Debug("Showing list of selectable sabers");

            for (int i = 0; i < CSLAssetLoader.SabersMetadata.Count; i++)
            {
                CustomSaberMetadata metadata = CSLAssetLoader.SabersMetadata[i];
                
                if (metadata.SaberFileName != null)
                {
                    if (!File.Exists(Path.Combine(saberAssetPath, metadata.SaberFileName))) continue;
                }

                Plugin.Log.Debug($"#{i+1} \"{metadata.SaberName}\"");

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

            int selectedSaber = CSLAssetLoader.SelectedSaberIndex;
            customListTableData.tableView.SelectCellWithIdx(selectedSaber);

            if (!customListTableData.tableView.visibleCells.Where(x => x.selected).Any())
            {
                customListTableData.tableView.ScrollToCellWithIdx(selectedSaber, TableView.ScrollPositionType.Beginning, true);
            }
        }
    }
}
