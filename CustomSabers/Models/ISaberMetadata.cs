namespace CustomSabersLite.Models;

internal interface ISaberMetadata
{
    // how do i fix this damn model?
    SaberFileInfo SaberFile { get; }
    SaberLoaderError LoaderError { get; }
    Descriptor Descriptor { get; }
}