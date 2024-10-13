using BeatSaberMarkupLanguage.Components.Settings;
using HMUI;
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
}
