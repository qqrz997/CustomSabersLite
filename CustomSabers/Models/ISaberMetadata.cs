namespace CustomSabersLite.Models;

internal interface ISaberMetadata
{
    SaberFileInfo SaberFile { get; }

    SaberLoaderError LoaderError { get; }

    Descriptor Descriptor { get; }
}