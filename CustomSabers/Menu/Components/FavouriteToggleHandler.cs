using System.Collections.Generic;
using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Parser;
using BeatSaberMarkupLanguage.TypeHandlers;

namespace CustomSabersLite.Menu.Components;

[ComponentHandler(typeof(FavouriteToggle))]
internal class FavouriteToggleHandler : TypeHandler
{
    public override Dictionary<string, string[]> Props => new()
    {
        { "iconSize", ["icon-size"] }
    };

    public override void HandleType(BSMLParser.ComponentTypeWithData componentType, BSMLParserParams parserParams)
    {
        if (componentType.Component is not FavouriteToggle favouriteToggle)
        {
            throw new($"Expected {typeof(FavouriteToggle)}");
        }

        if (componentType.Data.TryGetValue("iconSize", out var iconSize))
        {
            favouriteToggle.IconSize = Parse.Float(iconSize);
        }

        favouriteToggle.UpdateVisuals();
    }
}