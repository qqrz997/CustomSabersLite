using CustomSabersLite.Data;
using UnityEngine;

namespace CustomSabersLite.UI
{
    internal class SharedProperties : ISharedSaberSettings
    {
        public bool DisableWhiteTrail { get; set; }

        public bool OverrideTrailDuration { get; set; }

        public int TrailDuration { get; set; }

        public TrailType TrailType { get; set; }

        public bool EnableCustomEvents { get; set; }

        public bool ForcefullyFoolish { get; set; }

        public bool EnableCustomColorScheme { get; set; }

        public Color LeftSaberColor { get; set; }

        public Color RightSaberColor { get; set; }

        internal static string[] Names = 
        { 
            nameof(DisableWhiteTrail), 
            nameof(OverrideTrailDuration), 
            nameof(TrailDuration),
            nameof(TrailType),
            nameof(EnableCustomEvents),
            nameof(ForcefullyFoolish),
            nameof(EnableCustomColorScheme),
            nameof(LeftSaberColor),
            nameof(RightSaberColor)
        };
    }
}
