using BeatSaberMarkupLanguage.Tags;
using CustomSabersLite.Utilities.Extensions;
using HMUI;
using UnityEngine;
using UnityEngine.UI;

namespace CustomSabersLite.Menu.Components;

// unused
public class ToggleButtonTag : ButtonWithIconTag
{
    public override string[] Aliases { get; } = ["toggle-button"];

    public override GameObject CreateObject(Transform parent)
    {
        var gameObject = base.CreateObject(parent);
        gameObject.name = "CustomSabersLiteToggleButton";
        
        // Icon
        var content = gameObject.transform.Find("Content");

        var contentLayoutGroup = content.GetComponent<StackLayoutGroup>();
        contentLayoutGroup.padding = new(3, 3, 1, 1);
        
        var iconLayoutElement = content.Find("Icon").gameObject.AddComponent<LayoutElement>();
        iconLayoutElement.preferredWidth = 3.5f;
        iconLayoutElement.preferredHeight = 3.74f;
        
        // Background
        var origBg = gameObject.transform.Find("BG").GetComponent<ImageView>();
        var background = Object.Instantiate(origBg, gameObject.transform, false);
        origBg.gameObject.Destroy();
        
        var button = gameObject.GetComponent<Button>();
        
        var toggleButton = gameObject.AddComponent<ToggleButton>();
        toggleButton.Init(background, button);
        
        return gameObject;
    }
}