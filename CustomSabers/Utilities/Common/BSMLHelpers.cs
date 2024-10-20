using BeatSaberMarkupLanguage.Components.Settings;
using HMUI;
using UnityEngine;
using UnityEngine.UI;

namespace CustomSabersLite.Utilities;

internal class BSMLHelpers
{
    public static void SetSliderInteractable(SliderSetting slider, bool interactable)
    {
        if (!slider) return;
        var alpha = interactable ? 1f : 0.5f;
        var sliderText = slider.transform.Find("Title").GetComponent<CurvedTextMeshPro>();
        var valueText = slider.transform.Find("BSMLSlider/SlidingArea/Value").GetComponent<CurvedTextMeshPro>();
        sliderText.color = sliderText.color with { a = alpha };
        valueText.color = valueText.color with { a = alpha };
        slider.GetComponentsInChildren<Image>().ForEach(im => im.color = im.color with { a = alpha });
        slider.transform.Find("BSMLSlider").GetComponent<CustomFormatRangeValuesSlider>().interactable = interactable;
        slider.GetComponentsInChildren<Button>().ForEach(bt => bt.interactable = interactable);
    }

    public static void SetSliderWidth(SliderSetting sliderSetting, float sliderWidth, float labelWidth)
    {
        if (!sliderSetting) return;
        var slider = (RectTransform)sliderSetting.transform.Find("BSMLSlider").transform;
        var label = (RectTransform)sliderSetting.transform.Find("Title").transform;
        slider.sizeDelta = slider.sizeDelta with { x = sliderWidth };
        label.sizeDelta = label.sizeDelta with { x = labelWidth };
    }

    public static void SetDropDownSettingWidth(ListSetting list, float dropdownWidth, float labelWidth)
    {
        if (!list) return;
        var valuePicker = (RectTransform)list.transform.Find("ValuePicker").transform;
        var nameText = (RectTransform)list.transform.Find("NameText").transform;
        valuePicker.sizeDelta = valuePicker.sizeDelta with { x = dropdownWidth };
        nameText.sizeDelta = nameText.sizeDelta with { x = labelWidth };
    }
}
