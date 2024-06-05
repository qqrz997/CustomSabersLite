using System.Runtime.CompilerServices;
using IPA.Config.Stores;
using CustomSabersLite.Data;
using IPA.Config.Stores.Attributes;
using UnityEngine;

[assembly: InternalsVisibleTo(GeneratedStore.AssemblyVisibilityTarget)]
namespace CustomSabersLite.Configuration
{
    internal class CSLConfig
    {
        public virtual bool Enabled { get; set; } = true;

        public virtual string CurrentlySelectedSaber { get; set; } = null;

        public virtual TrailType TrailType { get; set; } = TrailType.Custom;

        public virtual bool EnableCustomEvents { get; set; } = true;

        public virtual bool OverrideTrailDuration { get; set; } = false;

        public virtual int TrailDuration { get; set; } = 100;

        public virtual bool OverrideTrailWidth { get; set; } = false;

        public virtual int TrailWidth { get; set; } = 100;

        public virtual bool DisableWhiteTrail { get; set; } = true;

        public virtual bool EnableCustomColorScheme { get; set; } = false;

        public virtual Color LeftSaberColor { get; set; } = new Color(0.784f, 0.078f, 0.078f, 1f);

        public virtual Color RightSaberColor { get; set; } = new Color(0.157f, 0.557f, 0.824f, 1f);

        public virtual string PluginVer { get; set; } = string.Empty;

        [Ignore] public virtual bool ForcefullyFoolish { get; set; } = false;

        [Ignore] public virtual bool Fooled { get; set; } = false;

        // public virtual void OnReload() { }

        // public virtual void Changed() { }

        // public virtual void CopyFrom(CustomSaberConfig other) { }
    }
}
