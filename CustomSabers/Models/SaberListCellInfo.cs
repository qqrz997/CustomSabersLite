using System;
using CustomSabersLite.Utilities.Common;
using CustomSabersLite.Utilities.Extensions;
using UnityEngine;

namespace CustomSabersLite.Models;

internal class SaberListCellInfo
{
    public RichTextString NameText { get; }
    public RichTextString AuthorText { get; }
    public Sprite Icon { get; }
    public string? Value { get; }
    public DateTime DateAdded { get; }
    
    public SaberListCellInfo(CustomSaberMetadata meta)
    {
        Value = meta.SaberFile.Hash;
        DateAdded = meta.SaberFile.DateAdded;
        if (meta.LoaderError == SaberLoaderError.None)
        {
            NameText = meta.Descriptor.SaberName;
            AuthorText = meta.Descriptor.AuthorName;
            Icon = meta.Descriptor.Icon;
        }
        else
        {
            NameText = RichTextString.Create(meta.LoaderError.GetErrorMessage());
            AuthorText = RichTextString.Create(meta.SaberFile.FileInfo.Name);
            Icon = CSLResources.DefaultCoverImage;
        }
    }
    
    public SaberListCellInfo(string text, string subtext, Sprite icon, string? value)
    {
        NameText = RichTextString.Create(text);
        AuthorText = RichTextString.Create(subtext);
        Icon = icon;
        Value = value;
    }

    public bool TextContains(string value) => 
        NameText.Contains(value, StringComparison.OrdinalIgnoreCase) 
        || AuthorText.Contains(value, StringComparison.OrdinalIgnoreCase);
}
