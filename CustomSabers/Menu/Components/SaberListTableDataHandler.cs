using System.Collections.Generic;
using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Parser;
using BeatSaberMarkupLanguage.TypeHandlers;
using CustomSabersLite.Models;
using UnityEngine.UI;

namespace CustomSabersLite.Menu.Components;

[ComponentHandler(typeof(SaberListTableData))]
internal class SaberListTableDataHandler : TypeHandler
{
    private const int DefaultCellCount = 7;
    private const int DefaultListWidth = 60;
    
    public override Dictionary<string, string[]> Props => new()
    {
        { "selectCell", ["select-cell"] },
        { "visibleCells", ["visible-cells"] },
        { "cellSize", ["cell-size"] },
        { "id", ["id"] },
        { "data", ["data", "content"] },
        { "listWidth", ["list-width"] },
        { "expandCell", ["expand-cell"] },
        { "alignCenter", ["align-to-center"] },
        { "showScrollbar", ["show-scrollbar"] },
        { "extraButtons", ["extra-buttons", "extra-scrollbar-buttons"] },
        { "scrollbarDelta", ["scrollbar-delta"] },
    };

    public override void HandleType(BSMLParser.ComponentTypeWithData componentType, BSMLParserParams parserParams)
    {
        var saberList = (SaberListTableData)componentType.Component;

        if (componentType.Data.TryGetValue("selectCell", out string selectCell)
            && parserParams.Actions.TryGetValue(selectCell, out var action))
        {
            saberList.DidSelectCellWithIdxEvent += (tableView, i) => action.Invoke(tableView, i);
        }

        if (componentType.Data.TryGetValue("cellSize", out string cellSize))
        {
            saberList.SetCellSize(Parse.Float(cellSize));
        }

        if (componentType.Data.TryGetValue("showScrollbar", out string showScrollbar) && Parse.Bool(showScrollbar))
        {
            saberList.AddScrollBar(componentType.Component.transform);

            if (componentType.Data.TryGetValue("extraButtons", out string extraButtons) && Parse.Bool(extraButtons))
            {
                saberList.AddExtraButtons();
            }

            if (componentType.Data.TryGetValue("scrollbarDelta", out string scrollbarDelta))
            {
                saberList.ResizeScrollBar(Parse.Float(scrollbarDelta));
            }
        }

        if (componentType.Data.TryGetValue("data", out string value))
        {
            if (parserParams.Values.TryGetValue(value, out var contents))
            {
                var data = (IEnumerable<object>)contents.GetValue();
                saberList.Data.AddRange(data);
                saberList.ReloadData();
            }
        }

        var visibleCells = componentType.Data.TryGetValue("visibleCells", out string c) ? Parse.Float(c) : DefaultCellCount;
        var listWidth = componentType.Data.TryGetValue("listWidth", out string w) ? Parse.Float(w) : DefaultListWidth;
        var listHeight = saberList.CellSize(0) * visibleCells;
        
        var layoutElement = componentType.Component.GetComponent<LayoutElement>();
        layoutElement.preferredHeight = listHeight;
        layoutElement.preferredWidth = listWidth;

        saberList.InitTableView();

        if (componentType.Data.TryGetValue("id", out string id))
        {
            parserParams.AddEvent(id + "#PageUp", saberList.UpButtonPressed);
            parserParams.AddEvent(id + "#PageDown", saberList.DownButtonPressed);
        }
    }
}