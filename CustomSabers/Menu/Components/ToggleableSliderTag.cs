using System.Linq;
using BeatSaber.GameSettings;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Tags;
using BGLib.Polyglot;
using CustomSabersLite.Utilities.Extensions;
using HMUI;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static BeatSaberMarkupLanguage.Utilities.ImageResources;

namespace CustomSabersLite.Menu.Components;

[UsedImplicitly]
public class ToggleableSliderTag : BSMLTag
{
    public override string[] Aliases { get; } = ["toggleable-slider", "checkbox-slider", "checkbox-slider-setting"];

    public override GameObject CreateObject(Transform parent)
    {
        var settingsSubMenuInfos = DiContainer.Resolve<MainSettingsMenuViewController>()._settingsSubMenuInfos;

        var sliderTemplate = Object.FindObjectsOfType<ViewController>(true)
            .First(x => x is ControllerProfilesSettingsViewController)
            .transform.Find("Content/MainContent/Sliders/PositionX")
            .gameObject;
        
        var labelTemplate = sliderTemplate.transform.Find("Title").GetComponent<CurvedTextMeshPro>();

        var toggleTemplate = settingsSubMenuInfos
            .First(x => x.viewController is AudioLatencyViewController)
            .viewController
            .transform.Find("OverrideAudioLatency/SwitchView")
            .gameObject;
        
        // Parent
        var gameObject = new GameObject("CustomSabersLiteToggleableSlider") { layer = 5 };
        gameObject.transform.SetParent(parent, false);  
        gameObject.SetActive(false);
        
        var toggleableSlider = gameObject.AddComponent<ToggleableSlider>();
        var horizontalLayout = gameObject.AddComponent<HorizontalLayoutGroup>();
        var layoutElement = gameObject.AddComponent<LayoutElement>();
        horizontalLayout.childForceExpandWidth = false;
        layoutElement.preferredWidth = 90f;
        layoutElement.preferredHeight = 7f;
        
        var rectTransform = gameObject.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = Vector2.zero;
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.sizeDelta = new(70f, 7f);
        
        // Icon
        var iconObject = new GameObject("Icon") { layer = 5 };
        iconObject.transform.SetParent(gameObject.transform, false);
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
        labelObject.transform.SetParent(gameObject.transform, false);
        var labelLayoutElement = labelObject.AddComponent<LayoutElement>();
        labelLayoutElement.preferredWidth = 28f;
        toggleableSlider.Label = Object.Instantiate(labelTemplate, labelObject.transform, false);
        toggleableSlider.Label.GetComponent<LocalizedTextMeshProUGUI>().DestroyComponent();
        toggleableSlider.Label.enableWordWrapping = false;
        toggleableSlider.Label.fontSize = 4;
        toggleableSlider.Label.alignment = TextAlignmentOptions.CaplineLeft;
        toggleableSlider.Label.color = Color.white;
        toggleableSlider.Label.text = "Default Text";
        toggleableSlider.Label.rectTransform.anchorMin = Vector3.zero;
        toggleableSlider.Label.rectTransform.anchorMax = Vector3.one;
        toggleableSlider.Label.rectTransform.offsetMin = Vector3.zero;
        toggleableSlider.Label.rectTransform.offsetMax = new(-52, 0);
        
        // Slider
        var sliderObject = Object.Instantiate(sliderTemplate, gameObject.transform, false);
        sliderObject.GetComponentInChildren<LocalizedTextMeshProUGUI>().DestroyComponent();
        sliderObject.GetComponentInChildren<TextMeshProUGUI>().DestroyComponent();
        sliderObject.GetComponentInChildren<CanvasGroup>().DestroyComponent();
        sliderObject.transform.Find("SliderLeft").Destroy();
        sliderObject.name = "SliderSetting";

        var customFormatRangeValuesSlider = sliderObject.GetComponentInChildren<CustomFormatRangeValuesSlider>();
        customFormatRangeValuesSlider._formatString = "{0} %";
        toggleableSlider.Slider = customFormatRangeValuesSlider;
        toggleableSlider.Slider.name = "Slider";
        toggleableSlider.Slider._enableDragging = true;
        
        var sliderRectTransform = toggleableSlider.Slider.GetComponent<RectTransform>();
        sliderRectTransform.anchoredPosition = Vector2.zero;
        sliderRectTransform.anchorMin = Vector2.zero;
        sliderRectTransform.anchorMax = Vector2.one;
        sliderRectTransform.offsetMin = Vector2.zero;
        sliderRectTransform.offsetMax = Vector2.zero;
        
        // Toggle
        var toggleObject = new GameObject("ToggleSetting") { layer = 5 };
        toggleObject.transform.SetParent(gameObject.transform, false);
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
        
        var externalComponents = gameObject.AddComponent<ExternalComponents>();
        externalComponents.Components.Add(toggleableSlider.Icon);
        externalComponents.Components.Add(toggleableSlider.Label);
        
        gameObject.SetActive(true);
        return gameObject;
    }
}