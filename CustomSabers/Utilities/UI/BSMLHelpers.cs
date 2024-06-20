using BeatSaberMarkupLanguage.Components.Settings;
using TMPro;

namespace CustomSabersLite.Utilities.UI;

internal class BSMLHelpers
{
    public static void SetSliderInteractable(GenericInteractableSetting sliderInteractableComponent, bool interactable, TextMeshProUGUI sliderTextComponent = null)
    {
        if (sliderTextComponent) sliderTextComponent.color = new(1f, 1f, 1f, interactable ? 1f : 0.5f);
        sliderInteractableComponent.interactable = interactable;
    }
}
