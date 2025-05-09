using CustomSabersLite.Models;
using CustomSabersLite.Utilities.Common;

namespace CustomSabersLite.Services;

internal class SaberMetadataConverter
{
    private readonly DirectoryManager directoryManager;
    private readonly SpriteCache spriteCache;
    private readonly FavouritesManager favouritesManager;

    public SaberMetadataConverter(
        DirectoryManager directoryManager,
        SpriteCache spriteCache,
        FavouritesManager favouritesManager)
    {
        this.directoryManager = directoryManager;
        this.spriteCache = spriteCache;
        this.favouritesManager = favouritesManager;
    }

    public CustomSaberMetadata ConvertJson(SaberMetadataModel meta, SaberFileInfo saberFileInfo)
    {
        // var fullPath = Path.Combine(directoryManager.CustomSabers.FullName, meta.RelativePath);
        // var fileInfo = new FileInfo(fullPath);
        // var saberFileInfo = new SaberFileInfo(fileInfo, meta.Hash, meta.DateAdded);
        
        var saberName = RichTextString.Create(meta.SaberName);
        var authorName = RichTextString.Create(meta.AuthorName);
        var icon = spriteCache.GetSprite(meta.Hash);
        if (icon == null) icon = CSLResources.NullCoverImage;
        var descriptor = new Descriptor(saberName, authorName, icon);
        
        var isFavourite = favouritesManager.IsFavourite(saberFileInfo);
        
        return new(
            saberFileInfo,
            meta.LoaderError,
            descriptor,
            meta.Trails,
            isFavourite);
    }

    public SaberMetadataModel CreateJson(CustomSaberMetadata meta)
    {
        // var relativePath = TrimPath(meta.SaberFile.FileInfo.FullName, directoryManager.CustomSabers.FullName);

        return new(
            meta.SaberFile.Hash,
            meta.SaberFile.DateAdded,
            meta.Descriptor.SaberName.FullName,
            meta.Descriptor.AuthorName.FullName,
            meta.HasTrails,
            meta.LoaderError);
    }
}