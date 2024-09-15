namespace CustomSabersLite.Models;

internal sealed class NoMetadata(string relativePath, SaberLoaderError loaderError) : ISaberMetadata
{
    public SaberFileInfo FileInfo => new(relativePath, CustomSaberType.Default);

    public SaberLoaderError LoaderError { get; } = loaderError;

    public Descriptor Descriptor => new("Unknown", "Unknown", null);

    public SaberModelFlags Flags => new(false, []);
}
