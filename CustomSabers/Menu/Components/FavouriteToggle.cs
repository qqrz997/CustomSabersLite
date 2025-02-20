using BeatSaberMarkupLanguage.Parser;
using UnityEngine;
using UnityEngine.UI;

namespace CustomSabersLite.Menu.Components;

internal class FavouriteToggle : MonoBehaviour
{
    // Should only be used as a BSML component
    private Image activeIcon = null!;
    private Image inactiveIcon = null!;
    private Toggle toggle = null!;
    
    public void Init(
        Image activeIcon,
        Image inactiveIcon,
        Toggle toggle)
    {
        this.activeIcon = activeIcon;
        this.inactiveIcon = inactiveIcon;
        this.toggle = toggle;
        
        toggle.onValueChanged.AddListener(ToggleValueChanged);
    }

    public BSMLValue? AssociatedValue { get; set; }

    public bool ToggleValue
    {
        get => toggle.isOn;
        set
        {
            toggle.isOn = value;
            AssociatedValue?.SetValue(value);
        }
    }

    public float IconSize { get; set; } = 4.5f;
    
    public void UpdateVisuals()
    {
        activeIcon.rectTransform.sizeDelta = new(IconSize, IconSize);
        inactiveIcon.rectTransform.sizeDelta = new(IconSize, IconSize);
    }

    public void ReceiveValue()
    {
        if (AssociatedValue != null)
        {
            ToggleValue = (bool)AssociatedValue.GetValue();
        }
    }

    private void ToggleValueChanged(bool value) => AssociatedValue?.SetValue(value);
}