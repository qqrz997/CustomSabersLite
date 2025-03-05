using BeatSaberMarkupLanguage.Parser;
using UnityEngine;
using UnityEngine.UI;

namespace CustomSabersLite.Menu.Components;

internal class FavouriteToggle : MonoBehaviour
{
    // Should only be used as a BSML component
    private Image activeIcon = null!;
    private Image inactiveIcon = null!;
    private Image blockedIcon = null!;
    private Toggle toggle = null!;
    
    public void Init(
        Image activeIcon,
        Image inactiveIcon,
        Image blockedIcon,
        Toggle toggle)
    {
        this.activeIcon = activeIcon;
        this.inactiveIcon = inactiveIcon;
        this.blockedIcon = blockedIcon;
        this.blockedIcon.gameObject.SetActive(false);
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
    public bool Interactable
    {
        set
        {
            toggle.gameObject.SetActive(value);
            activeIcon.gameObject.SetActive(value);
            inactiveIcon.gameObject.SetActive(value);
            blockedIcon.gameObject.SetActive(!value);
        }
    }

    public void UpdateVisuals()
    {
        activeIcon.rectTransform.sizeDelta = new(IconSize, IconSize);
        inactiveIcon.rectTransform.sizeDelta = new(IconSize, IconSize);
        blockedIcon.rectTransform.sizeDelta = new(IconSize, IconSize);
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