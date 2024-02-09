using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.ViewControllers;
using UnityEngine;
using TMPro;
using IPA.Utilities;
using HMUI;
using CustomSaber.Data;
using CustomSaber.Utilities;
using CustomSaber.Configuration;

namespace CustomSaber.UI
{
    internal class SaberListViewController : BSMLResourceViewController
    {
        public override string ResourceName => "CustomSaber.UI.Views.saberList.bsml";

        public static SaberListViewController Instance;

        public Action<CustomSaberData> customSaberChanged;

        [UIComponent("saberList")]
        public CustomListTableData customListTableData;

        [UIAction("saberSelect")]
        public void Select(TableView _, int row)
        {
            CustomSaberAssetLoader.SelectedSaber = row;
            CustomSaberConfig.Instance.CurrentlySelectedSaber = CustomSaberAssetLoader.CustomSaber[row].FileName;
            customSaberChanged?.Invoke(CustomSaberAssetLoader.CustomSaber[row]);
        }

        [UIAction("reloadSabers")]
        public void ReloadMaterials()
        {
            CustomSaberAssetLoader.Reload();
            SetupList();
            Select(customListTableData.tableView, CustomSaberAssetLoader.SelectedSaber);
        }

        [UIAction("#post-parse")]
        public void SetupList()
        {
            customListTableData.data.Clear();

            Plugin.Log.Debug("Showing list of selectable sabers");

            for (int i = 0; i < CustomSaberAssetLoader.CustomSaber.Count; i++)
            {
                CustomSaberData saber = CustomSaberAssetLoader.CustomSaber[i];
                Plugin.Log.Debug($"#{i+1} \"{saber.FileName}\"");
                var customCellInfo = new CustomListTableData.CustomCellInfo(saber.Descriptor.SaberName, saber.Descriptor.AuthorName, saber.Descriptor.CoverImage);
                customListTableData.data.Add(customCellInfo);
            }

            customListTableData.tableView.ReloadData();

            int selectedSaber = CustomSaberAssetLoader.SelectedSaber;
            customListTableData.tableView.SelectCellWithIdx(selectedSaber);

            if (!customListTableData.tableView.visibleCells.Where(x => x.selected).Any())
            {
                customListTableData.tableView.ScrollToCellWithIdx(selectedSaber, TableView.ScrollPositionType.Beginning, true);
            }
        }
    }
}
