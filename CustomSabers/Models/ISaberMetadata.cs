namespace CustomSabersLite.Models;

internal interface ISaberMetadata
{
    SaberFileInfo FileInfo { get; }

    SaberLoaderError LoaderError { get; }

    Descriptor Descriptor { get; }
}