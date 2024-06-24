using IPA;
using IPA.Config.Stores;
using IPALogger = IPA.Logging.Logger;
using Config = IPA.Config.Config;
using CustomSabersLite.Configuration;
using CustomSabersLite.Utilities;
using CustomSabersLite.Installers;
using SiraUtil.Zenject;
using IPA.Loader;
using Hive.Versioning;

namespace CustomSabersLite;

[NoEnableDisable]
[Plugin(RuntimeOptions.SingleStartInit)]
internal class Plugin
{
    public static Version Version { get; private set; } = null;

    [Init]
    public async void Init(IPALogger logger, Config config, Zenjector zenjector, PluginMetadata pluginMetadata)
    {
        Version = pluginMetadata.HVersion;
        _ = new Logger(logger);
        var pluginConfig = config.Generated<CSLConfig>();

        if (!await CustomSaberUtils.LoadCustomSaberAssembly())
        {
            return;
        }

        zenjector.UseLogger(logger);

        zenjector.Install<CSLAppInstaller>(Location.App, pluginConfig);
        zenjector.Install<CSLMenuInstaller>(Location.Menu);
        zenjector.Install<CSLGameInstaller>(Location.Player);
    }
}
