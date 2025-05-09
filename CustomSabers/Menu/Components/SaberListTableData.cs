using System;
using BeatSaberMarkupLanguage.Tags;
using CustomSabersLite.Models;
using CustomSabersLite.Utilities.Common;
using CustomSabersLite.Utilities.Extensions;
using HMUI;
using UnityEngine;
using UnityEngine.UI;

namespace CustomSabersLite.Menu.Components;

internal class SaberListTableData : MonoBehaviour, TableView.IDataSource
{
    private const string CellReuseIdentifier = "SaberListTableCell";
    
    [SerializeField] private float cellSize = SaberListCell.StandardCellSize;
    [SerializeField] private TableView tableView = null!;

    private GameObject? scrollbar;
    private Button? topButton;
    private Button? bottomButton;
    private ListStyle listStyle = ListStyle.Normal;

    public event Action? DidActivate;
    
    public void Init(TableView tv)
    {
        tableView = tv;
    }

    public void InitTableView()
    {
        tableView.gameObject.SetActive(true);
        tableView.LazyInit();
    }

    public ListStyle Style
    {
        get => listStyle;
        set
        {
            listStyle = value;
            cellSize = value switch
            {
                ListStyle.Normal => SaberListCell.StandardCellSize,
                ListStyle.Simple => SaberListCell.SimpleCellSize,
                _ => throw new ArgumentOutOfRangeException(nameof(value))
            };
        }
    }

    public void AddScrollBar(Transform parent)
    {
        var original = Instantiate(ScrollViewTag.ScrollViewTemplate);

        tableView.scrollView._pageUpButton = original._pageUpButton;
        tableView.scrollView._pageDownButton = original._pageDownButton;
        tableView.scrollView._verticalScrollIndicator = original._verticalScrollIndicator;
        
        var scrollBarTransform = (RectTransform)tableView.scrollView._verticalScrollIndicator.transform.parent;
        scrollBarTransform.offsetMin = Vector2.zero;
        scrollBarTransform.offsetMax = new(8, 0);
        scrollBarTransform.SetParent(parent, false);
        
        original.gameObject.Destroy();
        scrollbar = scrollBarTransform.gameObject;
    }

    public void AddExtraButtons()
    {
        if (scrollbar == null) return;
        var buttonBase = scrollbar.transform.Find("UpButton").GetComponent<Button>();
        topButton = CreateButton(buttonBase, 7f, new(0.5f, 1.0f), new(0.5f, 1.0f), new(2.5f, 2.5f), 180f, PluginResources.ExtremeArrowIcon, ScrollToTop, buttonBase.transform.parent);
        bottomButton = CreateButton(buttonBase, 7f, new(0.5f, 0f), new(0.5f, 0f), new(2.5f, 2.5f), 0f, PluginResources.ExtremeArrowIcon, ScrollToBottom, buttonBase.transform.parent);
        tableView.scrollView.scrollPositionChangedEvent += ScrollPositionChanged;
    }

    public void ResizeScrollBar(float yDelta)
    {
        if (scrollbar == null) return;
        var t = (RectTransform)scrollbar.transform;
        t.offsetMax = new(t.offsetMax.x, t.offsetMax.y + yDelta);
        t.offsetMin = new(t.offsetMin.x, t.offsetMin.y - yDelta);
    }

    public CustomListCollection Data { get; } = [];

    public event Action<TableView, int> DidSelectCellWithIdxEvent
    {
        add => tableView.didSelectCellWithIdxEvent += value;
        remove => tableView.didSelectCellWithIdxEvent -= value;
    }
    

    public void UpButtonPressed() => tableView.scrollView.PageUpButtonPressed();
    public void DownButtonPressed() => tableView.scrollView.PageDownButtonPressed();
    
    public void ScrollToTop() => tableView.ScrollToTop();
    public void ScrollToBottom() => tableView.ScrollToBottom();
    public void ScrollToCellWithIdx(int idx, TableView.ScrollPositionType positionType, bool animated) =>
        tableView.ScrollToCellWithIdx(idx, positionType, animated);
    public void SelectCellWithIdx(int idx) => tableView.SelectCellWithIdx(idx);
    public void ClearSelection() => tableView.ClearSelection();
    
    public void ReloadDataKeepingPosition() => tableView.ReloadDataKeepingPosition();
    public void ReloadData() => tableView.ReloadData();
    
    public float CellSize(int idx) => cellSize;
    public void SetCellSize(float val) => cellSize = val;
    public int NumberOfCells() => Data.Count;
    public TableCell CellForIdx(TableView tableView, int idx)
    {
        var tableCell = this.tableView.DequeueReusableCellForIdentifier(CellReuseIdentifier) as SaberListCell;

        if (tableCell == null)
        {
            tableCell = listStyle switch
            {
                ListStyle.Normal => SaberListCell.CreateStandardCell(),
                ListStyle.Simple => SaberListCell.CreateSimpleCell(),
                _ => throw new ArgumentOutOfRangeException(nameof(listStyle))
            };
            tableCell.reuseIdentifier = CellReuseIdentifier;
        }

        if (Data.TryGetElementAt(idx, out var saberListCell))
        {
            tableCell.SetInfo(saberListCell);
        }

        return tableCell;
    }

    private void ScrollPositionChanged(float currentPos)
    {
        float pos = tableView.scrollView._destinationPos;
        // Only should be called after creating the buttons
        topButton!.interactable = pos > 0.001f;
        bottomButton!.interactable = pos < tableView.scrollView.contentSize - tableView.scrollView.scrollPageSize - 0.001f;
    }

    private void OnEnable()
    {
        DidActivate?.Invoke();
    }

    private void OnDestroy()
    {
        if (tableView != null) tableView.scrollView.scrollPositionChangedEvent -= ScrollPositionChanged;
    }
    
    public enum ListStyle
    {
        Normal = 0,
        Simple
    }
    
    private static Button CreateButton(
        Button original,
        float size,
        Vector2 anchorMin,
        Vector2 anchorMax,
        Vector2 iconSize,
        float rotation,
        Sprite? icon = null,
        Action? onClick = null,
        Transform? parent = null)
    {
        var button = Instantiate(original);
        if (parent != null) button.transform.SetParent(parent, false);
        if (onClick != null) button.onClick.AddListener(onClick.Invoke);
        button.interactable = true;

        var buttonTransform = (RectTransform)button.transform;
        buttonTransform.anchorMin = anchorMin;
        buttonTransform.anchorMax = anchorMax;
        buttonTransform.sizeDelta = new(size, size);
        buttonTransform.offsetMin = new(size / -2f, size / -2f);
        buttonTransform.offsetMax = new(size / 2f, size / 2f);

        var iconImageView = button.GetComponentInChildren<ImageView>();
        if (icon != null) iconImageView.sprite = icon;

        var iconTransform = (RectTransform)iconImageView.transform;
        iconTransform.anchorMin = Vector2.zero;
        iconTransform.anchorMax = Vector2.one;
        iconTransform.anchorMin = new(0.5f, 0.5f);
        iconTransform.anchorMax = new(0.5f, 0.5f);
        iconTransform.offsetMin = Vector2.zero;
        iconTransform.offsetMax = Vector2.zero;
        iconTransform.sizeDelta = iconSize;
        iconTransform.localEulerAngles = new(0, 0, rotation);
        
        button.gameObject.SetActive(true);
        return button;
    }
}
