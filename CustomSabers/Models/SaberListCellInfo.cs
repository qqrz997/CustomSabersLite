using BeatSaberMarkupLanguage.Components;
using System;
using UnityEngine;

namespace CustomSabersLite.Models;

internal class SaberListCellInfo(CustomSaberMetadata meta, SaberListCellText info, Sprite icon)
{
    private SaberListCellText Info { get; } = info;
    private Sprite Icon { get; } = icon;

    public CustomSaberMetadata Metadata { get; } = meta;

    public CustomListTableData.CustomCellInfo ToCustomCellInfo()
    {
        var (text, subtext) = Info is SaberListCellText i ? (i.Text, i.Subtext) : ("Unknown", string.Empty);
        return new(text, subtext, Icon);
    }

    public bool Contains(string value) =>
        $"{Metadata.Descriptor.SaberName}{Metadata.Descriptor.AuthorName}".Contains(value, StringComparison.OrdinalIgnoreCase);
}
