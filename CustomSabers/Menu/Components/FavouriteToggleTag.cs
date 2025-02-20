using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Tags;
using HMUI;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Object;

namespace CustomSabersLite.Menu.Components;

internal class FavouriteToggleTag : BSMLTag
{
    public override string[] Aliases => ["favorite-toggle"];

    public override GameObject CreateObject(Transform parent)
    {
        var baseFavoriteToggle = DiContainer.Resolve<StandardLevelDetailViewController>()
            ._standardLevelDetailView
            ._favoriteToggle;

        var gameObject = new GameObject { name = "CustomFavoriteToggle", layer = 5 };
        gameObject.transform.SetParent(parent, false);
        
        
        var toggle = Instantiate(baseFavoriteToggle, gameObject.transform, false);
        toggle.name = "Toggle";

        var toggleTransform = (RectTransform)toggle.transform;
        toggleTransform.anchorMin = Vector2.zero;
        toggleTransform.anchorMax = Vector2.one;
        toggleTransform.offsetMin = Vector2.zero;
        toggleTransform.offsetMax = Vector2.zero;
        
        var inactiveIcon = toggle.transform.Find("Placeholder").GetComponent<ImageView>();
        inactiveIcon.rectTransform.anchorMin = new(0.5f, 0.5f);
        inactiveIcon.rectTransform.anchorMax = new(0.5f, 0.5f);
        inactiveIcon.rectTransform.pivot = new(0.5f, 0.5f);
        inactiveIcon.rectTransform.offsetMin = Vector2.zero;
        inactiveIcon.rectTransform.offsetMax = Vector2.zero;
        
        var activeIcon = toggle.transform.Find("Checkmark").GetComponent<ImageView>();
        activeIcon.rectTransform.anchorMin = new(0.5f, 0.5f);
        activeIcon.rectTransform.anchorMax = new(0.5f, 0.5f);
        activeIcon.rectTransform.pivot = new(0.5f, 0.5f);
        activeIcon.rectTransform.offsetMin = Vector2.zero;
        activeIcon.rectTransform.offsetMax = Vector2.zero;
        
        var favouriteToggle = gameObject.AddComponent<FavouriteToggle>();
        favouriteToggle.Init(activeIcon, inactiveIcon, toggle);
        
        var externalComponents = gameObject.AddComponent<ExternalComponents>();
        externalComponents.Components.Add(toggle);
        
        // Additional components
        gameObject.AddComponent<LayoutElement>();
        gameObject.AddComponent<Backgroundable>();
        
        return gameObject;
    }
}