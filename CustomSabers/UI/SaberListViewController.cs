using System;
using System.Linq;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.ViewControllers;
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

        [UIComponent("SaberList")]
        public CustomListTableData customListTableData;

        [UIAction("SelectSaber")]
        public void Select(TableView _, int row)
        {
            CustomSaberAssetLoader.SelectedSaber = row;
            CustomSaberConfig.Instance.CurrentlySelectedSaber = CustomSaberAssetLoader.CustomSabers[row].FileName;
            customSaberChanged?.Invoke(CustomSaberAssetLoader.CustomSabers[row]);
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
            await CustomSaberAssetLoader.ReloadAsync();
            SetupList();
            Select(customListTableData.tableView, CustomSaberAssetLoader.SelectedSaber);
        }

        

        [UIAction("#post-parse")]
        public void SetupList()
        {
            customListTableData.data.Clear();

            Plugin.Log.Debug("Showing list of selectable sabers");

            for (int i = 0; i < CustomSaberAssetLoader.CustomSabers.Count; i++)
            {
                CustomSaberData saber = CustomSaberAssetLoader.CustomSabers[i];
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
