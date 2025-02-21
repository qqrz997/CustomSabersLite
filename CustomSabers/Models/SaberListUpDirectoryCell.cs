using System.IO;
using CustomSabersLite.Utilities.Common;
using UnityEngine;

namespace CustomSabersLite.Models;

internal class SaberListUpDirectoryCell : ISaberListCell
{
    public SaberListUpDirectoryCell(DirectoryInfo directoryInfo)
    {
        AuthorText = RichTextString.Create(directoryInfo.Name);
        DirectoryInfo = directoryInfo;
    }
    
    public RichTextString NameText { get; } = RichTextString.Create("Go back...");
    public RichTextString AuthorText { get; }
    public Sprite Icon => CSLResources.TrailDurationIcon;
    public string? Value => null;
    public bool IsFavourite => false;
    
    public DirectoryInfo DirectoryInfo { get; }
}