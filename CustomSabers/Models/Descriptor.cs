namespace CustomSabersLite.Models;

public class Descriptor(string? saberName, string? authorName, byte[]? image)
{
    public string SaberName { get; } = saberName is not null ? saberName : "Unknown";

    public string AuthorName { get; } = authorName is not null ? authorName : "Unknown";

    public byte[]? Image { get; } = image;
}