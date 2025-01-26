using HMUI;

namespace CustomSabersLite.Utilities.Extensions;

internal static class TableViewExtensions
{
    public static void ScrollToTop(this TableView tableView) => tableView.ScrollToTop(false);
    public static void ScrollToTop(this TableView tableView, bool animated) =>
        tableView.ScrollToPosition(0, animated);

    public static void ScrollToBottom(this TableView tableView) => tableView.ScrollToBottom(false);
    public static void ScrollToBottom(this TableView tableView, bool animated) =>
        tableView.ScrollToPosition(
            tableView.dataSource.NumberOfCells() * (tableView.cellSize + tableView.spacing) + tableView.paddingStart,
            animated);
}