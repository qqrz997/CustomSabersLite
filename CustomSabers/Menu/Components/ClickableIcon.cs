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
    
    public void Init(Image image, Signal buttonClickedSignal)
    {
        this.image = image;
        this.buttonClickedSignal = buttonClickedSignal;
    }

    public event Action<PointerEventData>? OnClickEvent;
    public event Action<PointerEventData>? PointerEnterEvent;
    public event Action<PointerEventData>? PointerExitEvent;
    
    public bool Interactable { get; set; } = true;
    public Color NotHighlightedColor { get; set; } = Color.white;
    public Color HighlightedColor { get; set; } = new(0.7f, 0.8f, 1f);
    public float IconSize { get; set; } = 4.5f;

    public void OnPointerClick(PointerEventData eventData)
    {
        OnClickEvent?.Invoke(eventData);
        buttonClickedSignal.Raise();
        image.color = NotHighlightedColor;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        PointerEnterEvent?.Invoke(eventData);
        image.color = HighlightedColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        PointerExitEvent?.Invoke(eventData);
        image.color = NotHighlightedColor;
    }

    public void UpdateVisuals()
    {
        image.rectTransform.sizeDelta = new(IconSize, IconSize);
    }
}