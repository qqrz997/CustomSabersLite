using System.Runtime.CompilerServices;
using IPA.Config.Stores;
using CustomSaber.Data;

[assembly: InternalsVisibleTo(GeneratedStore.AssemblyVisibilityTarget)]
namespace CustomSaber.Configuration
{
    internal class CustomSaberConfig
    {
        public static CustomSaberConfig Instance { get; set; }

        public virtual string CurrentlySelectedSaber { get; internal set; } = "Default";

        public virtual TrailType TrailType { get; internal set; } = TrailType.Custom;

        public virtual bool CustomEventsEnabled { get; internal set; } = true;

        public virtual bool OverrideTrailDuration { get; internal set; } = false;

        public virtual int TrailDuration { get; internal set; } = 100;

        public virtual bool DisableWhiteTrail { get; internal set; } = true;

        public virtual string PluginVer { get; internal set; } = string.Empty;

        // public virtual void OnReload() { }

        public virtual void Changed()
        {
            Plugin.Log.Debug("Config changed");
        }

        // public virtual void CopyFrom(CustomSaberConfig other) { }
    }
}
