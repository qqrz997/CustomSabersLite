using BeatSaberMarkupLanguage.Parser;
using HMUI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CustomSabersLite.Menu.Components;

// unused
public class ToggleButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    // Should only be used as a BSML component
    private ImageView background = null!;

    private bool highlighted;
    private bool value;
       
    public void Init(ImageView background, Button button)
    {
        this.background = background;
        button.onClick.AddListener(() => ToggleValue = !ToggleValue);
        ReceiveValue();
    }
    
    public BSMLValue? ToggleAssociatedValue { get; set; }
    public bool ToggleValue
    {
        get => value;
        set
        {
            this.value = value;
            ApplyValue();
            UpdateVisuals();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        highlighted = true;
        UpdateVisuals();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        highlighted = false;
        UpdateVisuals();
    }

    private void ApplyValue() => ToggleAssociatedValue?.SetValue(value);
    private void ReceiveValue() => ToggleValue = (bool)(ToggleAssociatedValue?.GetValue() ?? false);

    private void UpdateVisuals()
    {
        var (color, color0, color1) = (value, highlighted) switch
        {
            (false, false) => (new Color(0f, 0f, 0f, 0.5f), Color.white, Color.white),
            (false, true) => (new Color(1f, 1f, 1f, 0.3f), Color.white, new Color(1f, 1f, 1f, 0.5f)),
            (true, false) => (new Color(0f, 0.75f, 1f, 1f), new Color(1f, 1f, 1f, 0f), Color.white),
            (true, true) => (new Color(0f, 0.75f, 1f, 0.75f), new Color(1f, 1f, 1f, 0f), Color.white)
        };
        
        background.color = color;
        background.color0 = color0;
        background.color1 = color1;
    }
}