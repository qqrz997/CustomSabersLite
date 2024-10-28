using UnityEngine;

namespace CustomSabersLite.Utilities.Extensions;

internal static class DefaultSaberColoring
{
    public static void SetNewColor(this SetSaberGlowColor setSaberGlowColor, Color color)
    {
        var materialPropertyBlock = setSaberGlowColor._materialPropertyBlock ?? new MaterialPropertyBlock();

        setSaberGlowColor._propertyTintColorPairs.ForEach(pair =>
            materialPropertyBlock.SetColor(pair.property, color * pair.tintColor));

        setSaberGlowColor._meshRenderer.SetPropertyBlock(materialPropertyBlock);
    }

    public static void SetNewColor(this SetSaberFakeGlowColor setSaberFakeGlowColor, Color color)
    {
        setSaberFakeGlowColor._parametric3SliceSprite.color = color;
        setSaberFakeGlowColor._parametric3SliceSprite.Refresh();
    }
}
