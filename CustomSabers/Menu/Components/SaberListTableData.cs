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
    
    [SerializeField] private float cellSize = 8.5f;
    [SerializeField] private TableView tableView = null!;

    private GameObject? scrollbar;
    private Button? topButton;
    private Button? bottomButton;
    
    public void Init(TableView tableView) => this.tableView = tableView;

    public void InitTableView()
    {
        tableView.gameObject.SetActive(true);
        tableView.LazyInit();
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
        topButton = buttonBase.CreateButton(7f, new(0.5f, 1.0f), new(0.5f, 1.0f), new(2.5f, 2.5f), 180f, CSLResources.ExtremeArrowIcon, ScrollToTop, buttonBase.transform.parent);
        bottomButton = buttonBase.CreateButton(7f, new(0.5f, 0f), new(0.5f, 0f), new(2.5f, 2.5f), 0f, CSLResources.ExtremeArrowIcon, ScrollToBottom, buttonBase.transform.parent);
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
    
    public bool IsActive => tableView.gameObject.activeInHierarchy;
    
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
            tableCell = SaberListCell.Instantiate();
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

    private void OnDestroy()
    {
        if (tableView != null) tableView.scrollView.scrollPositionChangedEvent -= ScrollPositionChanged;
    }
}
