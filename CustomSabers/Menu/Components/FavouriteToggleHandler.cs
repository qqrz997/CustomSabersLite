using System;
using System.Collections.Generic;
using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Parser;
using BeatSaberMarkupLanguage.TypeHandlers;

namespace CustomSabersLite.Menu.Components;

[ComponentHandler(typeof(FavouriteToggle))]
internal class FavouriteToggleHandler : TypeHandler<FavouriteToggle>
{
    public override Dictionary<string, string[]> Props => new()
    {
        { "value", ["value"] },
        { "iconSize", ["icon-size"] },
        { "bindValue", ["bind-value"] }
    };

    public override Dictionary<string, Action<FavouriteToggle, string>> Setters => new()
    {
        { "iconSize", (toggle, val) => toggle.IconSize = Parse.Float(val) }
    };

    public override void HandleTypeAfterChildren(BSMLParser.ComponentTypeWithData componentType, BSMLParserParams parserParams)
    {
        if (componentType.Component is not FavouriteToggle favouriteToggle)
        {
            throw new($"Expected {typeof(FavouriteToggle)}");
        }

        if (componentType.Data.TryGetValue("value", out var value))
        {
            if (!parserParams.Values.TryGetValue(value, out var bsmlValue))
                throw new("Value not found on BSML host");
            favouriteToggle.AssociatedValue = bsmlValue;

            if (componentType.Data.TryGetValue("bindValue", out string bindValue) && Parse.Bool(bindValue))
            {
                BindValue(componentType, parserParams, bsmlValue, _ => favouriteToggle.ReceiveValue());
            }
        }
        
        favouriteToggle.UpdateVisuals();
        favouriteToggle.ReceiveValue();
    }
}