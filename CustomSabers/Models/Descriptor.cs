namespace CustomSabersLite.Models;

public class Descriptor(string saberName, string authorName, byte[] image)
{
    public string SaberName { get; } = !string.IsNullOrWhiteSpace(saberName) ? saberName : "Unknown";

    public string AuthorName { get; } = !string.IsNullOrWhiteSpace(authorName) ? authorName : "Unknown";

    public byte[] Image { get; } = image;
}