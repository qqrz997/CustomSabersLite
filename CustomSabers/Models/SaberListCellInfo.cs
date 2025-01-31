using System;
using CustomSabersLite.Utilities.Common;
using CustomSabersLite.Utilities.Extensions;
using UnityEngine;

namespace CustomSabersLite.Models;

internal class SaberListCellInfo
{
    public RichTextString Text { get; }
    public RichTextString Subtext { get; }
    public Sprite Icon { get; }
    public string? SaberHash { get; }
    public DateTime DateAdded { get; }
    
    public SaberListCellInfo(CustomSaberMetadata meta)
    {
        SaberHash = meta.SaberFile.Hash;
        DateAdded = meta.SaberFile.DateAdded;
        if (meta.LoaderError == SaberLoaderError.None)
        {
            Text = meta.Descriptor.SaberName;
            Subtext = meta.Descriptor.AuthorName;
            Icon = meta.Descriptor.Icon;
        }
        else
        {
            Text = RichTextString.Create(meta.LoaderError.GetErrorMessage());
            Subtext = RichTextString.Create(meta.SaberFile.FileInfo.Name);
            Icon = CSLResources.DefaultCoverImage;
        }
    }
    
    public SaberListCellInfo(string text, string subtext, Sprite icon) =>
        (Text, Subtext, Icon) = (RichTextString.Create(text), RichTextString.Create(subtext), icon);
    
    public bool TextContains(string value) => 
        Text.Contains(value, StringComparison.OrdinalIgnoreCase) 
        || Subtext.Contains(value, StringComparison.OrdinalIgnoreCase);
}
