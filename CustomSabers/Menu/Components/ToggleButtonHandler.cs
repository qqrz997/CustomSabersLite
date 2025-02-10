using System;
using System.Collections.Generic;
using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Parser;
using BeatSaberMarkupLanguage.TypeHandlers;

namespace CustomSabersLite.Menu.Components;

// unused
[ComponentHandler(typeof(ToggleButton))]
public class ToggleButtonHandler : TypeHandler<ToggleButton>
{
    public override Dictionary<string, string[]> Props => new()
    {
        { "value", ["value"] }
    };
    
    public override Dictionary<string, Action<ToggleButton, string>> Setters { get; } = [];

    public override void HandleType(BSMLParser.ComponentTypeWithData componentType, BSMLParserParams parserParams)
    {
        if (componentType.Component is not ToggleButton toggleButton) throw new($"Expected {typeof(ToggleButton)}");
        if (componentType.Data.TryGetValue("value", out string value))
        {
            if (!parserParams.Values.TryGetValue(value, out var bsmlValue))
                throw new("Slider value not found on BSML host");
            toggleButton.ToggleAssociatedValue = bsmlValue;
        }
    }

}