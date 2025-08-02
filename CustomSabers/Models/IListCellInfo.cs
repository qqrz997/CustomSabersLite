using SabersLib.Models;
using UnityEngine;

namespace CustomSabersLite.Models;

internal interface IListCellInfo
{
    public RichTextString NameText { get; }
    public RichTextString AuthorText { get; }
    public Sprite Icon { get; }
    public bool IsFavourite { get; }
}