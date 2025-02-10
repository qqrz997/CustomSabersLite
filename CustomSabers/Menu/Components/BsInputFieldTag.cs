using BeatSaberMarkupLanguage.Tags;
using HMUI;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Object;

namespace CustomSabersLite.Menu.Components;

internal class BsInputFieldTag : BSMLTag
{
    public override string[] Aliases => ["input-field"];
    
    public override GameObject CreateObject(Transform parent)
    {
        var levelSearchViewController = DiContainer.Resolve<LevelSearchViewController>();
        var searchTextInputFieldView = levelSearchViewController._searchTextInputFieldView;
        
        // Create the container
        var gameObject = new GameObject("BsInputField") { layer = 5 };
        var inputField = gameObject.AddComponent<BsInputField>();
        
        // Copy the search input
        var searchInputField = Instantiate(searchTextInputFieldView, gameObject.transform);
        inputField.Init(searchInputField);

        // Add skew
        var bgImage = searchInputField.transform.Find("BG").GetComponent<ImageView>();
        bgImage._skew = 0.18f;
        
        // Add additional components
        gameObject.AddComponent<LayoutElement>();
        gameObject.AddComponent<VerticalLayoutGroup>();
        
        gameObject.transform.SetParent(parent, false);
        return gameObject;
    }
}