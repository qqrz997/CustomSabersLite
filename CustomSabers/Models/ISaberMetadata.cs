namespace CustomSabersLite.Models;

//internal record SaberMetadata(SaberFileInfo FileInfo, SaberLoaderError LoaderError, Descriptor Descriptor, SaberModelFlags Flags);

internal interface ISaberMetadata
{
    SaberFileInfo FileInfo { get; }

    SaberLoaderError LoaderError { get; }

    Descriptor Descriptor { get; }

    SaberModelFlags Flags { get; }
}