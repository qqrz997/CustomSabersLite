using System.Runtime.CompilerServices;
using IPA.Config.Stores;
using CustomSaber.Data;

[assembly: InternalsVisibleTo(GeneratedStore.AssemblyVisibilityTarget)]
namespace CustomSaber.Configuration
{
    internal class CustomSaberConfig
    {
        public static CustomSaberConfig Instance { get; set; }

        public virtual string CurrentlySelectedSaber { get; internal set; } 

        public virtual TrailType TrailType { get; internal set; }

        // public virtual bool CustomEventsEnabled { get; internal set; }

        public virtual bool OverrideTrailDuration {  get; internal set; }

        public virtual int TrailDuration { get; internal set; }

        public virtual bool DisableWhiteTrail { get; internal set; }

        public virtual string PluginVer { get; internal set; }

        // public virtual void OnReload() { }

        public virtual void Changed()
        {
            Plugin.Log.Debug("Config changed");
        }

        // public virtual void CopyFrom(CustomSaberConfig other) { }
    }
}
