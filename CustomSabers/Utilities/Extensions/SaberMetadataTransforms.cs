using System.IO;
using CustomSabersLite.Models;
using CustomSabersLite.Utilities.Common;
using UnityEngine;

namespace CustomSabersLite.Utilities.Extensions;

internal static class SaberMetadataTransforms
{
    public static SaberMetadataModel ToJson(this CustomSaberMetadata meta) =>
        new(meta.SaberFile.RelativePath,
            meta.SaberFile.Hash,
            meta.SaberFile.Type,
            meta.LoaderError,
            meta.Descriptor.SaberName.FullName,
            meta.Descriptor.AuthorName.FullName,
            meta.SaberFile.DateAdded);

    public static CustomSaberMetadata ToSaberMetadata(this SaberMetadataModel meta, Sprite? icon) =>
        new(new(new(Path.Combine(PluginDirs.CustomSabers.FullName, meta.RelativePath)),
                meta.Hash,
                meta.DateAdded,
                meta.SaberType),
            meta.LoaderError,
            new(RichTextString.Create(meta.SaberName),
                RichTextString.Create(meta.AuthorName),
                icon != null ? icon : CSLResources.NullCoverImage));
}