using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Parser;
using BeatSaberMarkupLanguage.TypeHandlers;
using JetBrains.Annotations;

namespace CustomSabersLite.Menu.Components;

[ComponentHandler(typeof(ToggleableSlider)), UsedImplicitly]
public class ToggleableSliderHandler : TypeHandler<ToggleableSlider>
{
    public override Dictionary<string, string[]> Props => new()
    {
        // Slider props
        { "minValue", ["min", "min-value"] },
        { "maxValue", ["max", "max-value"] },
        { "increment", ["increment"] },
        { "intOnly", ["int-only", "integer-only"] },
        { "sliderValue", ["slider-value"] },
        { "format-string", ["format-string"] },
        // Toggle props
        { "toggleValue", ["toggle-value"] },
        // Image props
        { "imageSource", ["image-source", "img-source", "img-src", "source", "src"] },
        { "imageColor", ["image-color", "img-color"] },
        // Props
        { "bindValues", ["bind-values"] },
        { "setEvent", ["set-event"] },
        { "getEvent", ["get-event"] },
        { "text", ["text"] }
    };

    public override Dictionary<string, Action<ToggleableSlider, string>> Setters { get; } = [];

    public override void HandleType(BSMLParser.ComponentTypeWithData componentType, BSMLParserParams parserParams)
    {
        if (componentType.Component is not ToggleableSlider toggleableSlider) throw new($"Expected {typeof(ToggleableSlider)}");
        if (toggleableSlider.Slider == null) throw new("Toggleable slider has no slider");
        if (toggleableSlider.Toggle == null) throw new("Toggleable slider has no toggle");
        
        if (componentType.Data.TryGetValue("minValue", out string minValue))
            toggleableSlider.Slider.minValue = Parse.Float(minValue);

        if (componentType.Data.TryGetValue("maxValue", out string maxValue))
            toggleableSlider.Slider.maxValue = Parse.Float(maxValue);

        if (componentType.Data.TryGetValue("increment", out string increment))
            toggleableSlider.Increment = Parse.Float(increment);

        toggleableSlider.IntOnly = componentType.Data.TryGetValue("intOnly", out string intOnly) && Parse.Bool(intOnly);
        
        BSMLValue? sliderBsmlValue = null;
        BSMLValue? toggleBsmlValue = null;
        
        if (componentType.Data.TryGetValue("sliderValue", out string sliderValue))
        {
            if (!parserParams.Values.TryGetValue(sliderValue, out sliderBsmlValue))
                throw new("Slider value not found on BSML host");
            toggleableSlider.SliderAssociatedValue = sliderBsmlValue;
        }
        
        if (componentType.Data.TryGetValue("toggleValue", out string toggleValue))
        {
            if (!parserParams.Values.TryGetValue(toggleValue, out toggleBsmlValue))
                throw new("Toggle value not found on BSML host");
            toggleableSlider.ToggleAssociatedValue = toggleBsmlValue;
        }

        if (componentType.Data.TryGetValue("bindValues", out string bindValues))
        {
            if (sliderBsmlValue == null || toggleBsmlValue == null)
                throw new("Toggleable Slider needs both slider and toggle values in order to bind values");
            
            if (Parse.Bool(bindValues))
            {
                BindValue(componentType, parserParams, sliderBsmlValue, _ => toggleableSlider.ReceiveSliderValue());
                BindValue(componentType, parserParams, toggleBsmlValue, _ => toggleableSlider.ReceiveToggleValue());
            }
        }

        if (componentType.Data.TryGetValue("imageSource", out string imageSource))
        {
            toggleableSlider.Icon.SetImageAsync(imageSource)
                .ContinueWith(t => 
                    Logger.Error($"Failed to load image\n{t.Exception}"), TaskContinuationOptions.OnlyOnFaulted);
        }

        if (componentType.Data.TryGetValue("imageColor", out string imageColor))
        {
            if (toggleableSlider.Icon != null) toggleableSlider.Icon.color = Parse.Color(imageColor);
        }
            
        parserParams.AddEvent(componentType.Data.GetValueOrDefault("setEvent", "apply"), toggleableSlider.ApplyValues);
        parserParams.AddEvent(componentType.Data.GetValueOrDefault("getEvent", "apply"), toggleableSlider.ReceiveValues);

        if (componentType.Data.TryGetValue("text", out string text) && string.IsNullOrWhiteSpace(text))
        {
            toggleableSlider.Label.transform.parent.gameObject.SetActive(false);
        }

        if (componentType.Data.TryGetValue("format-string", out string formatString))
        {
            toggleableSlider.Slider._formatString = formatString;
        }
    }

    public override void HandleTypeAfterChildren(BSMLParser.ComponentTypeWithData componentType, BSMLParserParams p) =>
        ((ToggleableSlider)componentType.Component).Setup();
}