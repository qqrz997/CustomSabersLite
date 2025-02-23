using CustomSabersLite.Utilities.Common;
using UnityEngine;

namespace CustomSabersLite.Models;

internal class SaberListFavouritesCell : ISaberListCell
{
    public RichTextString NameText { get; } = RichTextString.Create("Favourites");
    public RichTextString AuthorText => RichTextString.Empty;
    public Sprite Icon => CSLResources.FolderFavouritesIcon;
    public bool IsFavourite => false;
}