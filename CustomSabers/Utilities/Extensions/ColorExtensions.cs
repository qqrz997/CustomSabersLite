using UnityEngine;

namespace CustomSabersLite.Utilities.Extensions
{
    internal static class ColorExtensions
    {
        public static Color ColorWithAlpha(this Color color, float alpha)
        {
            color.a = alpha;
            return color;
        }
    }
}
