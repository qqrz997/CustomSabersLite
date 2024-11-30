using UnityEngine;

namespace CustomSabersLite.Utilities.Extensions;

internal static class MathExtensions
{
    public static float Clamp(this float value, float min, float max) => Mathf.Clamp(value, min, max);
    
    public static bool Approximately(this float a, float b) => Mathf.Approximately(a, b);
}