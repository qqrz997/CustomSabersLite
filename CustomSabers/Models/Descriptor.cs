namespace CustomSabersLite.Models;

internal class Descriptor(string? saberName, string? authorName, byte[]? image)
{
    public RichTextSegment SaberName { get; } = RichTextSegment.Create(saberName);

    public RichTextSegment AuthorName { get; } = RichTextSegment.Create(authorName);

    public byte[]? Image { get; } = image;
}
