using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Components.Settings;
using HMUI;
using System;
using UnityEngine;
using UnityEngine.UI;

using static UnityEngine.Object;

namespace CustomSabersLite.Utilities;

internal class BSMLHelpers
{
    public static void SetSliderInteractable(SliderSetting slider, bool interactable)
    {
        if (!slider) return;
        float alpha = interactable ? 1f : 0.5f;
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

    public static void ResizeVerticalScrollbar(CustomListTableData customListTableData, float delta)
    {
        if (!customListTableData) return;
        var scrollbar = (RectTransform)customListTableData.transform.Find("ScrollBar").transform;
        scrollbar.offsetMax = new(scrollbar.offsetMax.x, scrollbar.offsetMax.y + delta);
        scrollbar.offsetMin = new(scrollbar.offsetMin.x, scrollbar.offsetMin.y - delta);
    }

    // todo - try moving this to a custom tag
    public static Button CreateButton(Button original, float size, Vector2 anchorMin, Vector2 anchorMax, Vector2 iconSize, float rotation, Sprite? icon = null, Action? onClick = null, Transform? parent = null)
    {
        var button = Instantiate(original);
        if (parent != null) button.transform.SetParent(parent, false);
        if (onClick != null) button.onClick.AddListener(onClick.Invoke);
        button.interactable = true;

        var buttonTransform = (RectTransform)button.transform;
        buttonTransform.anchorMin = anchorMin;
        buttonTransform.anchorMax = anchorMax;
        buttonTransform.sizeDelta = new(size, size);
        buttonTransform.offsetMin = new(size / -2f, size / -2f);
        buttonTransform.offsetMax = new(size / 2f, size / 2f);

        var iconImageView = button.GetComponentInChildren<ImageView>();
        if (icon != null) iconImageView.sprite = icon;

        var iconTransform = (RectTransform)iconImageView.transform;
        iconTransform.anchorMin = Vector2.zero;
        iconTransform.anchorMax = Vector2.one;
        iconTransform.anchorMin = new(0.5f, 0.5f);
        iconTransform.anchorMax = new(0.5f, 0.5f);
        iconTransform.offsetMin = Vector2.zero;
        iconTransform.offsetMax = Vector2.zero;
        iconTransform.sizeDelta = iconSize;
        iconTransform.localEulerAngles = new(0, 0, rotation);

        return button;
    }

    // todo - try moving this to a custom tag
    public static ImageView CreateToggleButtonBackground(Button button)
    {
        var originalButtonBg = button.transform.Find("BG").GetComponentInChildren<ImageView>();
        var newBackground = Instantiate(originalButtonBg, originalButtonBg.transform.parent, false);
        newBackground.gameObject.name = "ToggleButtonBG";
        newBackground.color = new(1f, 1f, 1f, newBackground.color.a);
        newBackground.color0 = Color.black;
        newBackground.color1 = Color.black;
        return newBackground;
    }
}
