namespace CustomSabersLite.Models;

internal record SaberListFilterOptions(OrderBy OrderBy)
{
    public static SaberListFilterOptions Default { get; } = new(OrderBy.Name);
}
