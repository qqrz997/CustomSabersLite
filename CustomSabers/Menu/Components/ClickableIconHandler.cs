using System;
using System.Collections.Generic;
using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Parser;
using BeatSaberMarkupLanguage.TypeHandlers;

namespace CustomSabersLite.Menu.Components;

[ComponentHandler(typeof(ClickableIcon))]
internal class ClickableIconHandler : TypeHandler<ClickableIcon>
{
    public override Dictionary<string, string[]> Props => new()
    {
        { "onClick", ["on-click"] },
        { "clickEvent", ["click-event", "event-click"] },
        { "highlightColor", ["highlight-color"] },
        { "defaultColor", ["default-color"] },
        { "iconSize", ["icon-size"] }
    };

    public override Dictionary<string, Action<ClickableIcon, string>> Setters => new()
    {
        { "highlightColor", (image, color) => image.HighlightedColor = Parse.Color(color) },
        { "defaultColor", (image, color) => image.NotHighlightedColor = Parse.Color(color) }
    };

    public override void HandleType(BSMLParser.ComponentTypeWithData componentType, BSMLParserParams parserParams)
    {
        if (componentType.Component is not ClickableIcon clickableIcon)
        {
            throw new($"Expected {typeof(ClickableIcon)}");
        }
        
        if (componentType.Data.TryGetValue("onClick", out string onClick)
            && parserParams.Actions.TryGetValue(onClick, out var onClickAction))
        {
            clickableIcon.OnClickEvent += _ => onClickAction.Invoke();
        }

        if (componentType.Data.TryGetValue("clickEvent", out string clickEvent))
        {
            clickableIcon.OnClickEvent += _ => parserParams.EmitEvent(clickEvent);
        }

        if (componentType.Data.TryGetValue("iconSize", out string iconSize))
        {
            clickableIcon.IconSize = Parse.Float(iconSize);
        }

        clickableIcon.UpdateVisuals();
    }
}