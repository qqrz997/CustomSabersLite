using CustomSabersLite.Utilities;
using System.IO;

namespace CustomSabersLite.Models;

internal sealed class NoMetadata(string relativePath, SaberLoaderError loaderError) : ISaberMetadata
{
    public SaberFileInfo FileInfo => new(relativePath, string.Empty, CustomSaberType.Default);

    public SaberLoaderError LoaderError { get; } = loaderError;

    public Descriptor Descriptor => new(Path.GetFileName(relativePath), "Unknown", CSLResources.DefaultCoverImage);
}
