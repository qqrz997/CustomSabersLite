using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Tags;
using HMUI;
using UnityEngine;
using UnityEngine.UI;
using static BeatSaberMarkupLanguage.Utilities.ImageResources;

namespace CustomSabersLite.Menu.Components;

internal class ClickableIconTag : BSMLTag
{
    public override string[] Aliases => ["clickable-icon"];

    public override GameObject CreateObject(Transform parent)
    {
        var signal = DiContainer.Resolve<StandardLevelDetailViewController>()
            .transform.Find("LevelDetail/FavoriteToggle")
            .GetComponent<SignalOnPointerClick>()
            ._inputFieldClickedSignal;
        
        var gameObject = new GameObject { name = "ClickableIcon", layer = 5 };
        var imageObject = new GameObject { name = "Image", layer = 5 };
        imageObject.transform.SetParent(gameObject.transform, false);

        var image = imageObject.AddComponent<ImageView>();
        image.material = NoGlowMat;
        image.sprite = BlankSprite;
        image.rectTransform.anchorMin = new(0.5f, 0.5f);
        image.rectTransform.anchorMax = new(0.5f, 0.5f);

        var customClickableImage = gameObject.AddComponent<ClickableIcon>();
        customClickableImage.Init(image, signal);
        
        var externalComponents = gameObject.AddComponent<ExternalComponents>();
        externalComponents.Components.Add(image);
        
        gameObject.AddComponent<LayoutElement>();
        gameObject.AddComponent<Backgroundable>();
        
        gameObject.transform.SetParent(parent, false);
        return gameObject;
    }
}