using BeatSaberMarkupLanguage.Components;

namespace CustomSabersLite.Models;

internal class SaberListCellInfo(CustomSaberMetadata meta, SaberListCellText info, IThumbnail thumbnail)
{
    private SaberListCellText Info { get; } = info;
    private IThumbnail Icon { get; } = thumbnail;

    public CustomSaberMetadata Metadata { get; } = meta;

    public CustomListTableData.CustomCellInfo ToCustomCellInfo()
    {
        var (text, subtext) = Info is SaberListCellText i ? (i.Text, i.Subtext) : ("Unknown", string.Empty);
        return new(text, subtext, Icon.GetSprite());
    }
}
