using CustomSabersLite.Utilities.Common;
using UnityEngine;

namespace CustomSabersLite.Models;

internal class ListFavouritesCellInfo : IListCellInfo
{
    public RichTextString NameText { get; } = RichTextString.Create("Favourites");
    public RichTextString AuthorText => RichTextString.Empty;
    public Sprite Icon => PluginResources.FolderFavouritesIcon;
    public bool IsFavourite => false;
}