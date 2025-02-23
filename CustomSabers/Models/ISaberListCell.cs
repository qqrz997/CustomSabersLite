using UnityEngine;

namespace CustomSabersLite.Models;

internal interface ISaberListCell
{
    public RichTextString NameText { get; }
    public RichTextString AuthorText { get; }
    public Sprite Icon { get; }
    public bool IsFavourite { get; }
}