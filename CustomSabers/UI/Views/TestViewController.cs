using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.ViewControllers;
using CustomSabersLite.Data;
using CustomSabersLite.Utilities;
using HMUI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CustomSabersLite.UI.Views
{
    [HotReload(RelativePathToLayout = "../BSML/test.bsml")]
    [ViewDefinition("CustomSabersLite.UI.BSML.test.bsml")]
    internal class TestViewController : BSMLAutomaticViewController
    {
        [UIValue("trail-type-list")]
        public List<object> trailType = Enum.GetNames(typeof(TrailType)).ToList<object>();

        /*[UIComponent("saber-list")]
        public CustomListTableData saberList;*/

        [UIAction("#post-parse")]
        public void PostParse()
        {
            /*saberList.data.Clear();
            for (int i = 0; i < SabersMetadata.Count; i++)
            {
                CustomSaberMetadata metadata = CustomSaberAssetLoader.SabersMetadata[i];
                string saberName = metadata.SaberName;

                if (metadata.SaberFileName != null)
                {
                    if (!File.Exists(Path.Combine(PluginDirs.CustomSabers.FullName, metadata.SaberFileName))) continue;
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

            int selectedSaber = CustomSaberAssetLoader.SelectedSaberIndex;
            saberList.tableView.SelectCellWithIdx(selectedSaber);

            if (!saberList.tableView.visibleCells.Where(x => x.selected).Any())
            {
                saberList.tableView.ScrollToCellWithIdx(selectedSaber, TableView.ScrollPositionType.Beginning, true);
            }*/
        }
    }
}
