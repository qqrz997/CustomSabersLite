using CustomSabersLite.Utilities;
using System;
using System.IO;

namespace CustomSabersLite.Models;

internal sealed class NoMetadata(string relativePath, DateTime date, SaberLoaderError loaderError) : ISaberMetadata
{
    public SaberFileInfo FileInfo => new(relativePath, string.Empty, date, CustomSaberType.Default);

    public SaberLoaderError LoaderError { get; } = loaderError;

    public Descriptor Descriptor => new(Path.GetFileName(relativePath), "Unknown", CSLResources.DefaultCoverImage);
}
