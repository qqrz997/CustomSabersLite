using System.ComponentModel;
using BeatSaberMarkupLanguage;
using CustomSabersLite.Models;
using HMUI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CustomSabersLite.Menu.Components;

/// <summary>
/// The list cell that is used in the main saber list.
/// Do not call AddComponent with this. Instead, use <see cref="CreateStandardCell"/>.
/// </summary>
internal class SaberListCell : TableCell
{
    // fields will be set on the prefab
    [SerializeField] private CanvasGroup canvasGroup = null!;
    [SerializeField] private Image backgroundImage = null!;
    [SerializeField] private TextMeshProUGUI mainText = null!;
    [SerializeField] private TextMeshProUGUI subText = null!;
    [SerializeField] private Image icon  = null!;
    [SerializeField] private Image favoriteImage = null!;

    private IListCellInfo? listCellInfo;
    
    public void SetInfo(IListCellInfo cellInfo)
    {
        listCellInfo = cellInfo;

        if (cellInfo is ListInfoCellInfo infoCell)
        {
            infoCell.PropertyChanged -= CellInfoPropertyChanged;
            infoCell.PropertyChanged += CellInfoPropertyChanged;
        }
        
        UpdateInfo(cellInfo);
        RefreshVisuals();
    }

    private void CellInfoPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (listCellInfo != null) UpdateInfo(listCellInfo);
    }

    private void UpdateInfo(IListCellInfo cellInfo)
    {
        mainText.text = cellInfo.NameText.FullName;
        subText.text = cellInfo.AuthorText.FullName;
        icon.sprite = cellInfo.Icon;
        favoriteImage.enabled = cellInfo.IsFavourite;
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

    private void OnDestroy()
    {
        if (listCellInfo is ListInfoCellInfo infoCell) infoCell.PropertyChanged -= CellInfoPropertyChanged;
    }


    /// <summary>
    /// Makes a new table cell from a prefab. The first call to this method will create the prefab.
    /// </summary>
    public static SaberListCell CreateStandardCell()
    {
        if (StandardCellPrefab != null) return Instantiate(StandardCellPrefab);

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

        saberListCell.backgroundImage = originalTableCell._backgroundImage;
        saberListCell.icon = originalTableCell._coverImage;
        
        saberListCell.mainText = originalTableCell._songNameText;
        saberListCell.mainText.name = "SaberName";
        saberListCell.mainText.rectTransform.offsetMax = new(0, saberListCell.mainText.rectTransform.offsetMax.y);

        saberListCell.subText = originalTableCell._songAuthorText;
        saberListCell.subText.name = "AuthorName";
        saberListCell.subText.rectTransform.offsetMax = new(0, saberListCell.subText.rectTransform.offsetMax.y);
        saberListCell.subText.richText = true;
        saberListCell.subText.alpha = 0.75f;

        saberListCell.favoriteImage = originalTableCell._favoritesBadgeImage;
        saberListCell.favoriteImage.enabled = false;
        var favouriteIconTransform = (RectTransform)saberListCell.favoriteImage.transform;
        favouriteIconTransform.offsetMin = new(-6.5f, -1.9f);
        favouriteIconTransform.offsetMax = new(-2.7f, 1.9f);
        
        DestroyImmediate(originalTableCell);

        StandardCellPrefab = saberListCell;
        return Instantiate(StandardCellPrefab);
    }

    /// <summary>
    /// Makes a new table cell without a cover image or subtext from a prefab.
    /// The first call to this method will create the prefab.
    /// </summary>
    /// <returns></returns>
    public static SaberListCell CreateSimpleCell()
    {
        const float iconSize = SimpleCellSize - 0.8f;
        
        if (SimpleCellPrefab != null) return Instantiate(SimpleCellPrefab);

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
        
        saberListCell.backgroundImage = originalTableCell._backgroundImage;
        
        saberListCell.icon = originalTableCell._coverImage;
        saberListCell.icon.rectTransform.sizeDelta = new(iconSize, iconSize);
        
        saberListCell.mainText = originalTableCell._songNameText;
        saberListCell.mainText.name = "SaberName";
        saberListCell.mainText.fontSize = 3f;
        saberListCell.mainText.rectTransform.anchorMin = Vector2.zero;
        saberListCell.mainText.rectTransform.anchorMax = Vector2.one;
        saberListCell.mainText.rectTransform.offsetMin = new(iconSize + 2f, -1f);
        saberListCell.mainText.rectTransform.offsetMax = new(0f, 1f);

        saberListCell.subText = originalTableCell._songAuthorText;
        saberListCell.subText.gameObject.SetActive(false);

        saberListCell.favoriteImage = originalTableCell._favoritesBadgeImage;
        saberListCell.favoriteImage.enabled = false;
        var favouriteIconTransform = (RectTransform)saberListCell.favoriteImage.transform;
        favouriteIconTransform.offsetMin = new(-6.5f, -1.9f);
        favouriteIconTransform.offsetMax = new(-2.7f, 1.9f);
        
        DestroyImmediate(originalTableCell);

        SimpleCellPrefab = saberListCell;
        return Instantiate(SimpleCellPrefab);
    }

    public const float StandardCellSize = 8.5f;
    public const float SimpleCellSize = 5.0f;
    
    private static SaberListCell? StandardCellPrefab { get; set; }
    private static SaberListCell? SimpleCellPrefab { get; set; }
}
