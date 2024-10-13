using CustomSabersLite.Utilities;
using UnityEngine;

namespace CustomSabersLite.Models;

internal class Descriptor(string? saberName, string? authorName, Sprite? icon)
{
    public RichTextSegment SaberName { get; } = RichTextSegment.Create(saberName);

    public RichTextSegment AuthorName { get; } = RichTextSegment.Create(authorName);

    public Sprite Icon { get; } = icon ?? CSLResources.NullCoverImage;

    public static Descriptor DefaultSabers { get; } = new("Default", "Beat Games", CSLResources.DefaultCoverImage);
}
