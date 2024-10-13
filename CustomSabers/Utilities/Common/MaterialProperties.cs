using UnityEngine;

namespace CustomSabersLite.Utilities;

internal class MaterialProperties
{
    public static readonly int Color = Shader.PropertyToID("_Color");
    public static readonly int CustomColors = Shader.PropertyToID("_CustomColors");
    public static readonly int Glow = Shader.PropertyToID("_Glow");
    public static readonly int Bloom = Shader.PropertyToID("_Bloom");
}
