namespace CustomSabersLite.Models;

internal sealed record CustomSaberMetadata
    (SaberFileInfo FileInfo, SaberLoaderError LoaderError, Descriptor Descriptor) : ISaberMetadata
{
    public static CustomSaberMetadata Default { get; } = new(
        SaberFileInfo.DefaultSabers,
        SaberLoaderError.None,
        Descriptor.DefaultSabers);
}
