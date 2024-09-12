namespace CustomSabersLite.Models;

public record CustomSaberMetadata (string SaberName, string AuthorName, string RelativePath, bool MissingShaders, byte[] CoverImage, SaberLoaderError LoadingError)
{
    public static CustomSaberMetadata DefaultSaber =>
        new("Default", "Beat Games", null, false, null, SaberLoaderError.None);

}