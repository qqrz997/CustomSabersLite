using System;
using System.Linq;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.ViewControllers;
using HMUI;
using CustomSaber.Data;
using CustomSaber.Utilities;
using CustomSaber.Configuration;
using UnityEngine;
using UnityEngine.UIElements;
using System.IO;
using BeatSaberMarkupLanguage.Components.Settings;
using UnityEngine.UI;

namespace CustomSaber.UI
{
    internal class SaberListViewController : BSMLResourceViewController
    {
        public override string ResourceName => "CustomSaber.UI.Views.saberList.bsml";

        public static SaberListViewController Instance;

        public Action<CustomSaberMetadata> CustomSaberChanged;

        [UIComponent("SaberList")]
        public CustomListTableData customListTableData;

        [UIComponent("ReloadButton")]
        public Selectable reloadButtonSelectable;

        [UIAction("SelectSaber")]
        public void Select(TableView _, int row)
        {
            CustomSaberAssetLoader.SelectedSaberIndex = row;
            CustomSaberConfig.Instance.CurrentlySelectedSaber = CustomSaberAssetLoader.SabersMetadata[row].SaberFileName;
            CustomSaberChanged?.Invoke(CustomSaberAssetLoader.SabersMetadata[row]);
            //currently loading saber on game load, probably should do it on saber select instead
            //that can be used for saber previewing
        }

        [UIAction("OpenInExplorer")]
        public void OpenInExplorer()
        {
            //todo - open custom sabers folder
        }

        [UIAction("DeleteSelectedSaber")]
        public void DeleteSelectedSaber()
        {
            //todo - saber deletion
        }

        [UIAction("ReloadSabers")]
        public async void ReloadMaterials()
        {
            reloadButtonSelectable.interactable = false;
            await CustomSaberAssetLoader.ReloadAsync();
            SetupList();
            Select(customListTableData.tableView, CustomSaberAssetLoader.SelectedSaberIndex);
            reloadButtonSelectable.interactable = true;
        }

        [UIAction("#post-parse")]
        public void SetupList()
        {
            customListTableData.data.Clear();

            Plugin.Log.Debug("Showing list of selectable sabers");

            for (int i = 0; i < CustomSaberAssetLoader.SabersMetadata.Count; i++)
            {
                CustomSaberMetadata metadata = CustomSaberAssetLoader.SabersMetadata[i];
                Plugin.Log.Debug($"#{i+1} \"{metadata.SaberFileName}\"");

                Sprite cover = null;
                if (metadata.CoverImage != null)
                {
                    Texture2D tex = new Texture2D(2, 2);
                    tex.LoadImage(metadata.CoverImage);
                    cover = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);
                }
                else
                {
                    cover = CustomSaberUtils.GetNullCoverImage();
                }

                if (metadata.SaberFileName == "Default") cover = CustomSaberUtils.GetDefaultCoverImage(); 

                var customCellInfo = new CustomListTableData.CustomCellInfo(Path.GetFileNameWithoutExtension(metadata.SaberFileName), metadata.AuthorName, cover);
                customListTableData.data.Add(customCellInfo);
            }

            customListTableData.tableView.ReloadData();

            int selectedSaber = CustomSaberAssetLoader.SelectedSaberIndex;
            customListTableData.tableView.SelectCellWithIdx(selectedSaber);

            if (!customListTableData.tableView.visibleCells.Where(x => x.selected).Any())
            {
                customListTableData.tableView.ScrollToCellWithIdx(selectedSaber, TableView.ScrollPositionType.Beginning, true);
            }
        }
    }
}
