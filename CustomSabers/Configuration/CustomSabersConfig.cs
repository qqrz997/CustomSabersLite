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

        //public virtual bool CustomEventsEnabled { get; internal set; }

        public virtual bool OverrideTrailDuration {  get; internal set; }

        public virtual int TrailDuration { get; internal set; }

        public virtual bool DisableWhiteTrail { get; internal set; }

        /// <summary>
        /// This is called whenever BSIPA reads the config from disk (including when file changes are detected).
        /// </summary>
        public virtual void OnReload()
        {
            // Do stuff after config is read from disk.

        }

        /// <summary>
        /// Call this to force BSIPA to update the config file. This is also called by BSIPA if it detects the file was modified.
        /// </summary>
        public virtual void Changed()
        {
            // Do stuff when the config is changed.
            Plugin.Log.Debug("Config changed");
        }

        /// <summary>
        /// Call this to have BSIPA copy the values from <paramref name="other"/> into this config.
        /// </summary>
        public virtual void CopyFrom(CustomSaberConfig other)
        {
            // This instance's members populated from other
        }
    }
}
