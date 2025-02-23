using System;
using CustomSabersLite.Utilities.Extensions;
using Zenject;

namespace CustomSabersLite.Configuration;

internal class PluginConfigManager : IDisposable
{
    private readonly PluginConfigModel pluginConfigModel;
    private readonly PluginConfig pluginConfig;

    public PluginConfigManager(PluginConfigModel pluginConfigModel, PluginConfig pluginConfig)
    {
        this.pluginConfigModel = pluginConfigModel;
        this.pluginConfig = pluginConfig;
    }

    public void Dispose() => SaveChanges(); // Update the config when the application is closed

    public void SaveChanges()
    {
        pluginConfigModel.Enabled = pluginConfig.Enabled;
        pluginConfigModel.CurrentlySelectedSaber = pluginConfig.CurrentlySelectedSaber.GetSerializedName();
        pluginConfigModel.CurrentlySelectedTrail = pluginConfig.CurrentlySelectedTrail.GetSerializedName();
        pluginConfigModel.DisableWhiteTrail = pluginConfig.DisableWhiteTrail;
        pluginConfigModel.OverrideTrailDuration = pluginConfig.OverrideTrailDuration;
        pluginConfigModel.TrailDuration = pluginConfig.TrailDuration;
        pluginConfigModel.OverrideTrailWidth = pluginConfig.OverrideTrailWidth;
        pluginConfigModel.TrailWidth = pluginConfig.TrailWidth;
        pluginConfigModel.OverrideSaberLength = pluginConfig.OverrideSaberLength;
        pluginConfigModel.SaberLength = pluginConfig.SaberLength;
        pluginConfigModel.OverrideSaberWidth = pluginConfig.OverrideSaberWidth;
        pluginConfigModel.SaberWidth = pluginConfig.SaberWidth;
        pluginConfigModel.EnableCustomEvents = pluginConfig.EnableCustomEvents;
        
        // Notify the config changed so that BSIPA saves it to the file
        pluginConfigModel.Changed();
    }
}