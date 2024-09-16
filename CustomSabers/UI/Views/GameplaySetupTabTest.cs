using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components.Settings;
using CustomSabersLite.Models;
using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using BeatSaberMarkupLanguage.Components;
using HMUI;
using BeatSaberMarkupLanguage.ViewControllers;

namespace CustomSabersLite.UI.Views;

[HotReload(RelativePathToLayout = "../BSML/gameplaySetup.bsml")]
[ViewDefinition("CustomSabersLite.UI.BSML.gameplaySetup.bsml")]
internal class TabTest : BSMLAutomaticViewController
{
    [UIAction("#post-parse")]
    public void PostParse()
    {
        // Saber Trail Type list setting 
        var trailTypePickerRect = trailTypeRT.gameObject.transform.Find("ValuePicker").GetComponent<RectTransform>();
        var trailTypeTextRect = trailTypeRT.gameObject.transform.Find("NameText").GetComponent<RectTransform>();
        trailTypePickerRect.sizeDelta = new(30, trailTypePickerRect.sizeDelta.y);
        trailTypeTextRect.sizeDelta = new(0, trailTypeTextRect.sizeDelta.y);

        // Trail duration and width slider setting
        var trailDurationRect = trailDurationText.transform.parent.transform.Find("BSMLSlider").GetComponent<RectTransform>();
        var trailWidthRect = trailWidthText.transform.parent.transform.Find("BSMLSlider").GetComponent<RectTransform>();
        trailDurationRect.sizeDelta = new(50, trailDurationRect.sizeDelta.y);
        trailWidthRect.sizeDelta = new(50, trailWidthRect.sizeDelta.y);
    }

    [UIValue("disable-white-trail")]
    public bool DisableWhiteTrail;

    [UIValue("override-trail-duration")]
    public bool OverrideTrailDuration;

    [UIValue("trail-duration")]
    public int TrailDuration;

    [UIValue("override-trail-width")]
    public bool OverrideTrailWidth;

    [UIValue("trail-width")]
    public int TrailWidth;

    [UIValue("trail-type")]
    public string TrailType = null!;

    [UIValue("trail-type-list")]
    public List<object> trailTypeList = [.. Enum.GetNames(typeof(TrailType))];

    [UIValue("enable-custom-events")]
    public bool EnableCustomEvents;

    [UIValue("forcefully-foolish")]
    public bool ForcefullyFoolish;

    [UIComponent("trail-duration")]
    private GenericInteractableSetting trailDurationInteractable = null!;

    [UIComponent("trail-duration")]
    private TextMeshProUGUI trailDurationText = null!;

    [UIComponent("trail-width")]
    private GenericInteractableSetting trailWidthInteractable = null!;

    [UIComponent("trail-width")]
    private TextMeshProUGUI trailWidthText = null!;

    [UIComponent("trail-type")]
    private RectTransform trailTypeRT = null!;

    [UIComponent("saber-list")]
    public CustomListTableData saberList = null!;

    [UIComponent("saber-list-loading")]
    public ImageView saberListLoadingIcon = null!;

    [UIAction("select-saber")]
    public void Select(TableView _, int row) { }
}
