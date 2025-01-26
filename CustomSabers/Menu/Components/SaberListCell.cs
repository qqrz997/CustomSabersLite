using BeatSaberMarkupLanguage;
using CustomSabersLite.Models;
using HMUI;
using UnityEngine;

namespace CustomSabersLite.Menu.Views;

/// <summary>
/// The list cell that is used in the main saber list.
/// Do not call AddComponent with this. Instead, use <see cref="Instantiate"/>.
/// </summary>
internal class SaberListCell : TableCell
{
    // fields will be set on the prefab
    [SerializeField] private CanvasGroup canvasGroup = null!;
    [SerializeField] private ImageView backgroundImage = null!;
    [SerializeField] private CurvedTextMeshPro mainText = null!;
    [SerializeField] private CurvedTextMeshPro subText = null!;
    [SerializeField] private ImageView icon  = null!;
    [SerializeField] private ImageView favoriteImage = null!;

    public void SetInfo(SaberListCellInfo info)
    {
        mainText.text = info.Text;
        subText.text = info.Subtext;
        icon.sprite = info.Icon;
        
        RefreshVisuals();
    }
    
    public override void HighlightDidChange(TransitionType transitionType) => RefreshVisuals();
    public override void SelectionDidChange(TransitionType transitionType) => RefreshVisuals();

    private void RefreshVisuals()
    {
        backgroundImage.color = (selected, highlighted) switch
        {
            (true, true) => new(0f, 0.75f, 1f, 0.75f),
            (_, true) => new(1f, 1f, 1f, 0.2f),
            (true, _) => new(0f, 0.75f, 1f, 1f),
            _ => Color.white
        };
        backgroundImage.enabled = interactable && (selected || highlighted);
    }
    
    
    
    /// <summary>
    /// Makes a new table cell from a prefab. The first call to this method will create the prefab.
    /// </summary>
    public static SaberListCell Instantiate()
    {
        if (prefab != null) return Object.Instantiate(prefab);

        var levelCollectionViewController = BeatSaberUI.DiContainer.Resolve<LevelCollectionViewController>();
        
        var original = levelCollectionViewController.transform.Find("LevelsTableView/TableView/Viewport/Content/LevelListTableCell");
        var gameObject = Instantiate(original).gameObject;
        gameObject.name = "SaberListTableCell";
        var originalTableCell = gameObject.GetComponent<LevelListTableCell>();
        var saberListCell = gameObject.AddComponent<SaberListCell>();
        
        DestroyImmediate(originalTableCell._songBpmText.gameObject);
        DestroyImmediate(originalTableCell._songDurationText.gameObject);
        DestroyImmediate(originalTableCell._promoBadgeGo);
        DestroyImmediate(originalTableCell._updatedBadgeGo);
        DestroyImmediate(originalTableCell.transform.Find("BpmIcon").gameObject);
        
        saberListCell.canvasGroup = originalTableCell._canvasGroup;
        saberListCell.canvasGroup.alpha = 1f;
        saberListCell._wasPressedSignal = originalTableCell._wasPressedSignal;
        
        saberListCell.backgroundImage = saberListCell.transform.Find("Background").GetComponent<ImageView>();
        saberListCell.icon = saberListCell.transform.Find("CoverImage").GetComponent<ImageView>();
        
        saberListCell.mainText = saberListCell.transform.Find("SongName").GetComponent<CurvedTextMeshPro>();
        saberListCell.mainText.name = "SaberName";
        saberListCell.mainText.rectTransform.offsetMax = new(0, saberListCell.mainText.rectTransform.offsetMax.y);

        saberListCell.subText = saberListCell.transform.Find("SongAuthor").GetComponent<CurvedTextMeshPro>();
        saberListCell.subText.name = "AuthorName";
        saberListCell.subText.rectTransform.offsetMax = new(0, saberListCell.subText.rectTransform.offsetMax.y);
        saberListCell.subText.richText = true;
        saberListCell.subText.alpha = 0.75f;

        saberListCell.favoriteImage = saberListCell.transform.Find("FavoritesIcon").GetComponent<ImageView>();
        saberListCell.favoriteImage.enabled = false;
        var favouriteIconTransform = (RectTransform)saberListCell.favoriteImage.transform;
        favouriteIconTransform.offsetMin = new(-6.5f, -1.9f);
        favouriteIconTransform.offsetMax = new(-2.7f, 1.9f);
        
        DestroyImmediate(originalTableCell);

        prefab = saberListCell;
        return Object.Instantiate(prefab);
    }
    private static SaberListCell? prefab;
}
