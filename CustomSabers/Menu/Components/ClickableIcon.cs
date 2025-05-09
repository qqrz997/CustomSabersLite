using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CustomSabersLite.Menu.Components;

internal class ClickableIcon 
    : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    // Should only be used as a BSML component
    private Image image = null!;
    private Signal buttonClickedSignal = null!;
    private bool highlighted;
    
    public void Init(Image image, Signal buttonClickedSignal)
    {
        this.image = image;
        this.buttonClickedSignal = buttonClickedSignal;
    }

    public event Action<PointerEventData>? OnClickEvent;
    public event Action<PointerEventData>? PointerEnterEvent;
    public event Action<PointerEventData>? PointerExitEvent;
    
    public bool Interactable { get; set; } = true;
    public Color NotHighlightedColor { get; set; } = new(1f, 1f, 1f, 0.75f);
    public Color HighlightedColor { get; set; } = Color.white;
    public float IconSize { get; set; } = 4.5f;

    public void OnPointerClick(PointerEventData eventData)
    {
        OnClickEvent?.Invoke(eventData);
        buttonClickedSignal.Raise();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        PointerEnterEvent?.Invoke(eventData);
        highlighted = true;
        UpdateVisuals();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        PointerExitEvent?.Invoke(eventData);
        highlighted = false;
        UpdateVisuals();
    }

    public void UpdateVisuals()
    {
        image.rectTransform.sizeDelta = new(IconSize, IconSize);
        image.color = highlighted ? HighlightedColor : NotHighlightedColor;
    }
}