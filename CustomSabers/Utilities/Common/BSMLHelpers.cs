using System;
using BeatSaberMarkupLanguage.Components.Settings;
using HMUI;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Object;

namespace CustomSabersLite.Utilities.Common;

internal static class BSMLHelpers
{
    public static void SetDropDownSettingWidth(this ListSetting list, float dropdownWidth, float labelWidth)
    {
        if (!list) return;
        var valuePicker = (RectTransform)list.transform.Find("ValuePicker").transform;
        var nameText = (RectTransform)list.transform.Find("NameText").transform;
        valuePicker.sizeDelta = valuePicker.sizeDelta with { x = dropdownWidth };
        nameText.sizeDelta = nameText.sizeDelta with { x = labelWidth };
    }

    // todo - try moving this to a custom tag
    public static Button CreateButton(this Button original, float size, Vector2 anchorMin, Vector2 anchorMax, Vector2 iconSize, float rotation, Sprite? icon = null, Action? onClick = null, Transform? parent = null)
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
        
        button.gameObject.SetActive(true);
        return button;
    }
}
