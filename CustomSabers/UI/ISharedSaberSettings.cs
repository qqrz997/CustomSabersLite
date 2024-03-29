using CustomSabersLite.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CustomSabersLite.UI
{
    internal interface ISharedSaberSettings
    {
        bool DisableWhiteTrail { get; set; }

        bool OverrideTrailDuration { get; set; }

        int TrailDuration { get; set; }

        TrailType TrailType { get; set; }

        bool EnableCustomEvents { get; set; }

        bool ForcefullyFoolish { get; set; }

        bool EnableCustomColorScheme { get; set; }

        Color LeftSaberColor { get; set; }

        Color RightSaberColor { get; set; }
    }
}
