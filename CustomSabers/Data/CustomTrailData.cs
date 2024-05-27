using UnityEngine;

namespace CustomSabersLite.Data
{
    /// <summary>
    /// Class that declares the neccessary information to create a <see cref="Components.CSLSaberTrail"/>
    /// </summary>
    internal class CustomTrailData
    {
        // Probably should attach an array of this to CustomSaberData and handle it when the saber is loaded so that we know preemptively if a saber has trails, or even has secondary trails

        public Transform TrailTop { get; }
        public Transform TrailBottom { get; }
        public Material TrailMaterial { get; }
        public Color TrailColor { get; } // todo - find out what the CustomTrail colors and colortype actually mean. for now i will use the saber's color here
        // public Color TrailColorMultiplier { get; }  todo ^

        public CustomTrailData(Transform trailTop, Transform trailBottom, Material trailMaterial, Color trailColor)
        {
            TrailTop = trailTop;
            TrailBottom = trailBottom;
            TrailMaterial = trailMaterial;
            TrailColor = trailColor;
        }
    }
}
