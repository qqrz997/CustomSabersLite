using System.Linq;
using BeatSaberMarkupLanguage.Tags;
using CustomSabersLite.Utilities;
using HMUI;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

namespace CustomSabersLite.UI.CustomTags;

[UsedImplicitly]
public class ToggleableSliderTag : BSMLTag
{
    public override string[] Aliases { get; } = ["toggleable-slider", "checkbox-slider", "checkbox-slider-setting"];

    public override GameObject CreateObject(Transform parent)
    {
        var settingsSubMenuInfos = DiContainer.Resolve<MainSettingsMenuViewController>()._settingsSubMenuInfos;

        var sliderTemplate = settingsSubMenuInfos
            .First(x => x.viewController is ControllersTransformSettingsViewController)
            .viewController
            .transform.Find("Content/PositionX")
            .gameObject;

        var toggleTemplate = settingsSubMenuInfos
            .First(x => x.viewController is AudioLatencyViewController)
            .viewController
            .transform.Find("OverrideAudioLatency/SwitchView")
            .gameObject;
        
        var gameObject = new GameObject($"{nameof(CustomSabersLite)}{nameof(ToggleableSlider)}")
        {
            layer = 5 // UI Layer
        };
        gameObject.SetActive(false);
        gameObject.transform.SetParent(parent, false);
        
        var horizontalLayout = gameObject.AddComponent<HorizontalLayoutGroup>();
        var layoutElement = gameObject.AddComponent<LayoutElement>();
        layoutElement.preferredWidth = 90f;
        layoutElement.preferredHeight = 7f;
        var toggleableSlider = gameObject.AddComponent<ToggleableSlider>();
        
        // Slider
        var slider = Object.Instantiate(sliderTemplate, gameObject.transform, false);
        slider.name = "SliderSetting";
        slider.transform.Find("Title").DestroyImmediate();
        toggleableSlider.Slider = slider.GetComponentInChildren<CustomFormatRangeValuesSlider>();
        toggleableSlider.Slider.name = "Slider";
        toggleableSlider.Slider._enableDragging = true;
        
        // Toggle
        var toggleObject = new GameObject("ToggleSetting")
        {
            layer = 5
        };
        toggleObject.transform.SetParent(gameObject.transform, false);
        toggleableSlider.Toggle = Object.Instantiate(toggleTemplate, toggleObject.transform, false).GetComponent<Toggle>();
        toggleableSlider.Toggle.interactable = true;
        
        var animatedSwitchView = toggleableSlider.Toggle.GetComponent<AnimatedSwitchView>();
        toggleableSlider.Toggle.onValueChanged.RemoveAllListeners();
        toggleableSlider.Toggle.onValueChanged.AddListener(animatedSwitchView.HandleOnValueChanged);
        toggleableSlider.Toggle.isOn = false;
        animatedSwitchView.enabled = true;
        
        // Parent scale
        var rectTransform = gameObject.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = new(0f, 0f);
        rectTransform.anchorMin = new (0f, 0f);
        rectTransform.anchorMax = new (1f, 1f);
        rectTransform.sizeDelta = new (70f, 7f);
        
        // Slider scale
        var sliderLayoutElement = toggleableSlider.Slider.gameObject.AddComponent<LayoutElement>();
        sliderLayoutElement.flexibleWidth = 500f;
        var sliderRectTransform = toggleableSlider.Slider.GetComponent<RectTransform>();
        sliderRectTransform.anchorMin = new(0f, 0f);
        sliderRectTransform.anchorMax = new(0f, 1f);
        sliderRectTransform.offsetMin = new(0f, 0f);
        sliderRectTransform.offsetMax = new(55f, 0f);
        
        // Toggle scale
        var toggleLayoutElement = toggleObject.AddComponent<LayoutElement>();
        toggleLayoutElement.minWidth = 20f;
        var toggleRectTransform = toggleableSlider.Toggle.GetComponent<RectTransform>();
        toggleRectTransform.anchorMin = new(1f, 0.5f);
        toggleRectTransform.anchorMax = new(1f, 0.5f);
        
        gameObject.SetActive(true);
        return gameObject;
    }
}