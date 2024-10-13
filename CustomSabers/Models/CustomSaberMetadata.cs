namespace CustomSabersLite.Models;

internal sealed record CustomSaberMetadata
    (SaberFileInfo FileInfo, SaberLoaderError LoaderError, Descriptor Descriptor, SaberModelFlags Flags) : ISaberMetadata
{
    public static CustomSaberMetadata Default { get; } = new(
        SaberFileInfo.DefaultSabers,
        SaberLoaderError.None,
        Descriptor.DefaultSabers,
        SaberModelFlags.None);
}
