using System.Linq;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Tags;
using BGLib.Polyglot;
using CustomSabersLite.Utilities;
using HMUI;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

using static BeatSaberMarkupLanguage.Utilities.ImageResources;

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
        
        var labelTemplate = sliderTemplate.transform.Find("Title").GetComponent<CurvedTextMeshPro>();

        var toggleTemplate = settingsSubMenuInfos
            .First(x => x.viewController is AudioLatencyViewController)
            .viewController
            .transform.Find("OverrideAudioLatency/SwitchView")
            .gameObject;
        
        // Parent
        var gameObject = new GameObject($"CustomSabersLiteToggleableSlider") { layer = 5 };
        gameObject.SetActive(false);
        
        var toggleableSlider = gameObject.AddComponent<ToggleableSlider>();
        var horizontalLayout = gameObject.AddComponent<HorizontalLayoutGroup>();
        var layoutElement = gameObject.AddComponent<LayoutElement>();
        horizontalLayout.childForceExpandWidth = false;
        layoutElement.preferredWidth = 90f;
        layoutElement.preferredHeight = 7f;
        
        var rectTransform = gameObject.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = new(0f, 0f);
        rectTransform.anchorMin = new (0f, 0f);
        rectTransform.anchorMax = new (1f, 1f);
        rectTransform.sizeDelta = new (70f, 7f);
        
        // Icon
        var iconObject = new GameObject("Icon") { layer = 5 };
        var iconLayoutElement = iconObject.AddComponent<LayoutElement>();
        iconLayoutElement.preferredWidth = 7f;
        var imageObject = new GameObject("Image") { layer = 5 };
        imageObject.transform.SetParent(iconObject.transform, false);
        toggleableSlider.Icon = imageObject.AddComponent<ImageView>();
        toggleableSlider.Icon.rectTransform.anchorMin = new(0.5f, 0.5f);
        toggleableSlider.Icon.rectTransform.anchorMax = new(0.5f, 0.5f);
        toggleableSlider.Icon.rectTransform.sizeDelta = new(6.5f, 6.5f);
        toggleableSlider.Icon._skew = 0.18f;
        toggleableSlider.Icon.preserveAspect = true;
        toggleableSlider.Icon.material = NoGlowMat;
        toggleableSlider.Icon.sprite = BlankSprite;
        
        // Label
        var labelObject = new GameObject("Label") { layer = 5 };
        var labelLayoutElement = labelObject.AddComponent<LayoutElement>();
        labelLayoutElement.preferredWidth = 16f;
        toggleableSlider.Label = Object.Instantiate(labelTemplate, labelObject.transform, false);
        toggleableSlider.GetComponent<LocalizedTextMeshProUGUI>().DestroyComponent();
        toggleableSlider.Label.enableWordWrapping = false;
        toggleableSlider.Label.fontSize = 4;
        toggleableSlider.Label.color = Color.white;
        toggleableSlider.Label.text = "Default Text";
        toggleableSlider.Label.rectTransform.anchorMin = Vector2.zero;
        toggleableSlider.Label.rectTransform.anchorMax = Vector2.one;
        
        // Slider
        var sliderObject = Object.Instantiate(sliderTemplate);
        sliderObject.name = "SliderSetting";
        toggleableSlider.Slider = sliderObject.GetComponentInChildren<CustomFormatRangeValuesSlider>();
        toggleableSlider.Slider.name = "Slider";
        toggleableSlider.Slider._enableDragging = true;
        
        var sliderRectTransform = toggleableSlider.Slider.GetComponent<RectTransform>();
        sliderRectTransform.anchorMin = new(0f, 0f);
        sliderRectTransform.anchorMax = new(1f, 1f);
        sliderRectTransform.offsetMin = new(0f, 0f);
        sliderRectTransform.offsetMax = new(0f, 0f);
        
        // Toggle
        var toggleObject = new GameObject("ToggleSetting") { layer = 5 };
        toggleableSlider.Toggle = Object.Instantiate(toggleTemplate, toggleObject.transform, false).GetComponent<Toggle>();
        toggleableSlider.Toggle.interactable = true;
        
        var animatedSwitchView = toggleableSlider.Toggle.GetComponent<AnimatedSwitchView>();
        toggleableSlider.Toggle.onValueChanged.RemoveAllListeners();
        toggleableSlider.Toggle.onValueChanged.AddListener(animatedSwitchView.HandleOnValueChanged);
        toggleableSlider.Toggle.isOn = false;
        animatedSwitchView.enabled = true;
        
        var toggleLayoutElement = toggleObject.AddComponent<LayoutElement>();
        toggleLayoutElement.preferredWidth = 20f;
        var toggleRectTransform = toggleableSlider.Toggle.GetComponent<RectTransform>();
        toggleRectTransform.anchorMin = new(1f, 0.5f);
        toggleRectTransform.anchorMax = new(1f, 0.5f);
        
        // Set layout group order
        gameObject.transform.SetParent(parent, false);  
        iconObject.transform.SetParent(gameObject.transform, false);
        labelObject.transform.SetParent(gameObject.transform, false);
        sliderObject.transform.SetParent(gameObject.transform, false);
        toggleObject.transform.SetParent(gameObject.transform, false);
        
        var externalComponents = gameObject.AddComponent<ExternalComponents>();
        externalComponents.Components.Add(toggleableSlider.Icon);
        externalComponents.Components.Add(toggleableSlider.Label);
        
        gameObject.SetActive(true);
        return gameObject;
    }
}