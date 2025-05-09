using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Tags;
using HMUI;
using UnityEngine;
using UnityEngine.UI;
using VRUIControls;

namespace CustomSabersLite.Menu.Components;

// Source reference by nicoco007 and monkeymanboy
// https://github.com/monkeymanboy/BeatSaberMarkupLanguage/blob/master/BeatSaberMarkupLanguage/Tags/ListTag.cs

public class SaberListTag : BSMLTag
{
    private Canvas? canvasTemplate;
    private Canvas CanvasTemplate => canvasTemplate != null ? canvasTemplate 
        : canvasTemplate = DiContainer.Resolve<GameplaySetupViewController>()
            ._playerSettingsPanelController
            ._noteJumpStartBeatOffsetDropdown
            ._simpleTextDropdown
            ._tableView.GetComponent<Canvas>();

    public override string[] Aliases => ["saber-list"];

    public override GameObject CreateObject(Transform parent)
    {
        var containerObject = new GameObject("SaberList", typeof(LayoutElement)) { layer = 5 };

        var containerTransform = (RectTransform)containerObject.transform;
        containerTransform.SetParent(parent, false);
        containerTransform.anchorMin = Vector2.zero;
        containerTransform.anchorMax = Vector2.one;
        containerTransform.sizeDelta = Vector2.zero;
        containerTransform.anchoredPosition = Vector2.zero;

        var gameObject = new GameObject("SaberList") { layer = 5 };
        gameObject.SetActive(false);

        var transform = gameObject.AddComponent<RectTransform>();
        transform.SetParent(containerTransform, false);
        transform.anchorMin = Vector2.zero;
        transform.anchorMax = Vector2.one;
        transform.sizeDelta = Vector2.zero;
        transform.anchoredPosition = Vector2.zero;

        gameObject.AddComponent<ScrollRect>();
        gameObject.AddComponent(CanvasTemplate);
        DiContainer.InstantiateComponent<VRGraphicRaycaster>(gameObject);
        gameObject.AddComponent<Touchable>();
        gameObject.AddComponent<EventSystemListener>();
        var scrollView = DiContainer.InstantiateComponent<BSMLScrollView>(gameObject);

        var tableView = gameObject.AddComponent<BSMLTableView>();
        var tableData = containerObject.AddComponent<SaberListTableData>();
        tableData.Init(tableView);

        tableView._preallocatedCells = [];
        tableView._isInitialized = false;
        tableView._scrollView = scrollView;

        var viewportTransform = new GameObject("Viewport").AddComponent<RectTransform>();
        viewportTransform.SetParent(gameObject.GetComponent<RectTransform>(), false);
        viewportTransform.gameObject.AddComponent<RectMask2D>();
        gameObject.GetComponent<ScrollRect>().viewport = viewportTransform;

        var contentTransform = new GameObject("Content").AddComponent<RectTransform>();
        contentTransform.SetParent(viewportTransform, false);

        scrollView._contentRectTransform = contentTransform;
        scrollView._viewport = viewportTransform;

        viewportTransform.anchorMin = Vector2.zero;
        viewportTransform.anchorMax = Vector2.one;
        viewportTransform.sizeDelta = Vector2.zero;
        viewportTransform.anchoredPosition = Vector2.zero;

        tableView.SetDataSource(tableData, false);
        return containerObject;
    }
}