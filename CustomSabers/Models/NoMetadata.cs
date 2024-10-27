using CustomSabersLite.Utilities;
using System;
using System.IO;

namespace CustomSabersLite.Models;

internal sealed class NoMetadata(string fullPath, DateTime date, SaberLoaderError loaderError) : ISaberMetadata
{
    public SaberFileInfo SaberFile => new(fullPath, string.Empty, date, CustomSaberType.Default);

    public SaberLoaderError LoaderError { get; } = loaderError;

    public Descriptor Descriptor => new(Path.GetFileName(fullPath), "Unknown", CSLResources.DefaultCoverImage);
}
