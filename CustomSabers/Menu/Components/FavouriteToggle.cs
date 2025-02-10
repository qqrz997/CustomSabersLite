using UnityEngine;
using UnityEngine.UI;

namespace CustomSabersLite.Menu.Components;

internal class FavouriteToggle : MonoBehaviour
{
    // Should only be used as a BSML component
    private Image activeIcon = null!;
    private Image inactiveIcon = null!;

    public void Init(Image activeIcon, Image inactiveIcon)
    {
        this.activeIcon = activeIcon;
        this.inactiveIcon = inactiveIcon;
    }
    
    public float IconSize { get; set; } = 4.5f;
    
    public void UpdateVisuals()
    {
        activeIcon.rectTransform.sizeDelta = new(IconSize, IconSize);
        inactiveIcon.rectTransform.sizeDelta = new(IconSize, IconSize);
    }
}