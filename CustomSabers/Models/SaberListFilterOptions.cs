namespace CustomSabersLite.Models;

internal record SaberListFilterOptions(string SearchFilter, OrderBy OrderBy, bool ReverseOrder)
{
    public static SaberListFilterOptions Default { get; } = new(string.Empty, OrderBy.Name, false);
}
