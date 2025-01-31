using System;
using CustomSabersLite.Utilities.Common;

namespace CustomSabersLite.Models;

internal sealed record NoMetadata(string FileName, DateTime Date, SaberLoaderError LoaderError) : ISaberMetadata
{
    // TODO: is there a better null pattern?
    public SaberFileInfo SaberFile => new(new(""), "", DateTime.MinValue, CustomSaberType.Default);

    public Descriptor Descriptor => 
        new(RichTextString.Create(FileName),
            RichTextString.Create("Unknown"), 
            CSLResources.DefaultCoverImage);
}
