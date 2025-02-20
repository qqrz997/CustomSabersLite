using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using CustomSabersLite.Menu.Views;
using CustomSabersLite.Utilities.Common;
using CustomSabersLite.Utilities.Extensions;
using UnityEngine;

namespace CustomSabersLite.Models;

internal interface ISaberListCell
{
    public RichTextString NameText { get; }
    public RichTextString AuthorText { get; }
    public Sprite Icon { get; }
    public string? Value { get; }
    public bool IsFavourite { get; }
}

internal class SaberListDirectoryCell : ISaberListCell
{
    public SaberListDirectoryCell(DirectoryInfo directoryInfo)
    {
        NameText = RichTextString.Create(directoryInfo.Name);
        DirectoryInfo = directoryInfo;
    }
    
    public RichTextString NameText { get; }
    public RichTextString AuthorText { get; } = RichTextString.Empty;
    public Sprite Icon => CSLResources.FolderIcon;
    public string? Value { get; } = null;
    public bool IsFavourite => false;
    
    public DirectoryInfo DirectoryInfo { get; }
}

internal class SaberListInfoCell : ISaberListCell, INotifyPropertyChanged
{
    private bool isFavourite;

    public RichTextString NameText { get; }
    public RichTextString AuthorText { get; }
    public Sprite Icon { get; }
    
    public string? Value { get; }

    public bool IsFavourite
    {
        get => isFavourite;
        set { isFavourite = value; NotifyPropertyChanged(); }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public SaberListInfoCell(CustomSaberMetadata meta)
    {
        Value = meta.SaberFile.Hash;
        IsFavourite = meta.IsFavourite;
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
    
    public SaberListInfoCell(string text, string subtext, Sprite icon, string? value)
    {
        NameText = RichTextString.Create(text);
        AuthorText = RichTextString.Create(subtext);
        Icon = icon;
        Value = value;
    }
    
    private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new(propertyName));
    }
}
