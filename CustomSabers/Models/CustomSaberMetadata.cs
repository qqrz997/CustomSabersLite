namespace CustomSabersLite.Models;

internal record CustomSaberMetadata(
    SaberFileInfo SaberFile,
    SaberLoaderError LoaderError,
    Descriptor Descriptor,
    TrailsInfo TrailsInfo);