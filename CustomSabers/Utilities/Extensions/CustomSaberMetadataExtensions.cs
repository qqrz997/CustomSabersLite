using CustomSabersLite.Data;
using UnityEngine;

using static CustomSabersLite.Utilities.ImageUtils;

namespace CustomSabersLite.Utilities.Extensions;

public static class CustomSaberMetadataExtensions
{
    // todo - need to separate the json metadata and the internal metadata but for now this works

    public static Sprite GetIcon(this CustomSaberMetadata metadata) => 
        metadata.RelativePath == null ? defaultCoverImage
        : metadata.LoadingError switch
        {
            SaberLoaderError.None => metadata.CoverImage?.LoadImage() ?? nullCoverImage,
            _ => defaultCoverImage
        };
}
