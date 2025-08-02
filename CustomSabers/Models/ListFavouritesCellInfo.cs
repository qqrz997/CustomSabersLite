using CustomSabersLite.Utilities.Common;
using SabersLib.Models;
using UnityEngine;

namespace CustomSabersLite.Models;

internal class ListFavouritesCellInfo : IListCellInfo
{
    public RichTextString NameText { get; } = RichTextString.Create("Favourites");
    public RichTextString AuthorText => RichTextString.Unknown;
    public Sprite Icon => PluginResources.FolderFavouritesIcon;
    public bool IsFavourite => false;
}