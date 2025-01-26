using System;
using UnityEngine;

namespace CustomSabersLite.Models;

internal class SaberListCellInfo(CustomSaberMetadata meta, SaberListCellText info, Sprite icon)
{
    public string Text { get; } = info.Text;
    public string Subtext { get; } = info.Subtext;
    public Sprite Icon { get; } = icon;
    public CustomSaberMetadata Metadata { get; } = meta;

    public bool Contains(string value) => 
        Metadata.Descriptor.SaberName.Contains(value, StringComparison.OrdinalIgnoreCase) 
        || Metadata.Descriptor.AuthorName.Contains(value, StringComparison.OrdinalIgnoreCase);
}
