using System.IO;
using CustomSabersLite.Utilities.Common;
using UnityEngine;

namespace CustomSabersLite.Models;

internal class SaberListDirectoryCell : ISaberListCell
{
    public SaberListDirectoryCell(DirectoryInfo directoryInfo)
    {
        NameText = RichTextString.Create(directoryInfo.Name);
        DirectoryInfo = directoryInfo;
    }
    
    public RichTextString NameText { get; }
    public RichTextString AuthorText => RichTextString.Empty;
    public Sprite Icon => CSLResources.FolderIcon;
    public string? Value => null;
    public bool IsFavourite => false;
    
    public DirectoryInfo DirectoryInfo { get; }
}