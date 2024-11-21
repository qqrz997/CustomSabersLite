using System;
using BeatSaberMarkupLanguage.Parser;
using HMUI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CustomSabersLite.UI.CustomTags;

public sealed class ToggleableSlider : MonoBehaviour
{
    private Button increaseButton = null!;
    private Button decreaseButton = null!;
    private TextMeshProUGUI sliderValueLabel = null!;
    private ImageView sliderHandleImage = null!;
    private Color sliderHandleImageDefaultColor; 
    
    public BSMLAction? Formatter { get; set; }

    public RangeValuesTextSlider Slider { get; set; } = null!;
    public BSMLValue? SliderAssociatedValue { get; set; } 
    public float SliderValue
    {
        get => Slider.value;
        set => Slider.value = value;
    }

    public Toggle Toggle { get; set; } = null!;
    public BSMLValue? ToggleAssociatedValue { get; set; }
    public bool ToggleValue
    {
        get => Toggle.isOn;
        set => Toggle.isOn = value;
    }

    public float Increment { get; set; }
    public bool IntOnly { get; set; }

    public ImageView Icon { get; set; } = null!;

    public void Setup()
    {
        if (Slider == null) throw new NullReferenceException($"{nameof(Slider)} is not set");
        if (Toggle == null) throw new NullReferenceException($"{nameof(Toggle)} is not set");
        
        increaseButton = Slider._incButton;
        decreaseButton = Slider._decButton;
        
        if (increaseButton == null) throw new NullReferenceException($"{nameof(increaseButton)} wasn't found");
        if (decreaseButton == null) throw new NullReferenceException($"{nameof(decreaseButton)} wasn't found");
        
        var slidingArea = Slider.transform.Find("SlidingArea");
        sliderHandleImage = slidingArea.Find("Handle").GetComponent<ImageView>();
        sliderHandleImageDefaultColor = sliderHandleImage.color;
        sliderValueLabel = slidingArea.Find("Value").GetComponent<TextMeshProUGUI>();
        
        if (sliderHandleImage == null) throw new NullReferenceException($"{nameof(sliderHandleImage)} wasn't found");
        if (sliderValueLabel == null) throw new NullReferenceException($"{nameof(sliderValueLabel)} wasn't found");
        
        Slider.numberOfSteps = (int)Math.Round((Slider.maxValue - Slider.minValue) / Increment) + 1;
        Slider.valueDidChangeEvent += SliderValueChanged;
        
        Toggle.onValueChanged.AddListener(ToggleValueChanged);
        
        ReceiveValues();
        
        ToggleValueChanged(ToggleValue);
        SliderValueChanged(Slider, SliderValue);
    }

    public void ApplyValue()
    {
        ApplySliderValue();
        ApplyToggleValue();
    }

    public void ReceiveValues()
    {
        ReceiveSliderValue();
        ReceiveToggleValue();
    }

    public void ReceiveSliderValue()
    {
        if (SliderAssociatedValue != null)
        {
            Slider.value = IntOnly ? (int)SliderAssociatedValue.GetValue() : (float)SliderAssociatedValue.GetValue();
        }
    }
    
    public void ReceiveToggleValue()
    {
        if (ToggleAssociatedValue != null)
        {
            ToggleValue = (bool)ToggleAssociatedValue.GetValue();
        }
    }

    private void ApplySliderValue()
    {
        if (IntOnly) SliderAssociatedValue?.SetValue((int)Math.Round(SliderValue));
        else SliderAssociatedValue?.SetValue(SliderValue);
    }
    
    private void ApplyToggleValue()
    {
        ToggleAssociatedValue?.SetValue(ToggleValue);
    }

    private void ToggleValueChanged(bool value)
    {
        ToggleValue = value;
        increaseButton.interactable = ToggleValue;
        decreaseButton.interactable = ToggleValue;
        Slider.interactable = ToggleValue;
        sliderHandleImage.color = ToggleValue ? sliderHandleImageDefaultColor : Color.white with { a = 0.2f };
        sliderValueLabel.color = ToggleValue ? Color.white : Color.white with { a = 0.2f };
        ApplyToggleValue();
    }
    
    private void SliderValueChanged(RangeValuesTextSlider slider, float value)
    {
        if (IntOnly) value = MathF.Round(value);
        
        sliderValueLabel.text = IntOnly switch
        {
            true when Formatter != null => (string)Formatter.Invoke((int)Math.Round(value)),
            false when Formatter != null => (string)Formatter.Invoke(value),
            true => ((int)Math.Round(value)).ToString(),
            false => value.ToString("N2"),
        };
        
        ApplySliderValue();
    }

    private void OnDestroy()
    {
        if (Slider != null) Slider.valueDidChangeEvent -= SliderValueChanged;
    }
}