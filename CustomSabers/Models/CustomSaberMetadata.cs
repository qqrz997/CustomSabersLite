﻿namespace CustomSabersLite.Models;

internal sealed class CustomSaberMetadata(SaberFileInfo fileInfo, SaberLoaderError loaderError, Descriptor descriptor, SaberModelFlags flags) : ISaberMetadata
{
    public SaberFileInfo FileInfo { get; } = fileInfo;

    public SaberLoaderError LoaderError { get; } = loaderError;

    public Descriptor Descriptor { get; } = descriptor;

    public SaberModelFlags Flags { get; } = flags;
}
