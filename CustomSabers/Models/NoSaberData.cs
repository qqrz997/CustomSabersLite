using CustomSabersLite.Utilities.Common;

namespace CustomSabersLite.Models;

internal class NoSaberData : ISaberData
{
    public CustomSaberMetadata Metadata { get; }
    public ISaberPrefab? Prefab => null;
    
    public NoSaberData(SaberFileInfo saberFile, SaberLoaderError loaderError)
    {
        Metadata = new(saberFile, loaderError, NoDescriptionDescriptor, false, false);
    }

    private Descriptor NoDescriptionDescriptor { get; } = new(
        RichTextString.Create(null),
        RichTextString.Create(null),
        PluginResources.DefaultCoverImage);
    
    public void Dispose() { }
}
