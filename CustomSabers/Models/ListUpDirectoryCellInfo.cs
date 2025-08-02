using System.IO;
using CustomSabersLite.Utilities.Common;
using SabersLib.Models;
using UnityEngine;

namespace CustomSabersLite.Models;

internal class ListUpDirectoryCellInfo : IListCellInfo
{
    public ListUpDirectoryCellInfo(DirectoryInfo directoryInfo)
    {
        AuthorText = RichTextString.Create(directoryInfo.Name);
        DirectoryInfo = directoryInfo;
    }
    
    public RichTextString NameText { get; } = RichTextString.Create("Go back...");
    public RichTextString AuthorText { get; }
    public Sprite Icon => PluginResources.EllipsisIcon;
    public bool IsFavourite => false;
    
    public DirectoryInfo DirectoryInfo { get; }
}