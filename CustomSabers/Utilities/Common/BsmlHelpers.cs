using BeatSaberMarkupLanguage.Components.Settings;
using UnityEngine;

namespace CustomSabersLite.Utilities.Common;

internal static class BsmlHelpers
{
    public static void SetDropDownSettingWidth(this ListSetting list, float dropdownWidth, float labelWidth)
    {
        if (!list) return;
        var valuePicker = (RectTransform)list.transform.Find("ValuePicker").transform;
        var nameText = (RectTransform)list.transform.Find("NameText").transform;
        valuePicker.sizeDelta = valuePicker.sizeDelta with { x = dropdownWidth };
        nameText.sizeDelta = nameText.sizeDelta with { x = labelWidth };
    }
}
