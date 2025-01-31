using System.Linq;
using CustomSabersLite.Models;

namespace CustomSabersLite.Utilities.Extensions;

internal static class SaberMetadataValidation
{
    public static CacheFileModel WithValidation(this CacheFileModel original) => original with
    {
        CachedMetadata = original.CachedMetadata.Where(meta => meta.IsValid()).ToArray(),
    };
    
    public static bool IsValid(this SaberMetadataModel meta) =>
        !string.IsNullOrWhiteSpace(meta.RelativePath)
        && !string.IsNullOrWhiteSpace(meta.Hash);
}