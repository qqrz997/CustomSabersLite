using IPA;
using IPA.Config.Stores;
using IPALogger = IPA.Logging.Logger;
using Config = IPA.Config.Config;
using CustomSabersLite.Configuration;
using CustomSabersLite.Installers;
using SiraUtil.Zenject;
using IPA.Loader;
using Hive.Versioning;
using System.Threading.Tasks;
using CustomSabersLite.Utilities;

namespace CustomSabersLite;

[Plugin(RuntimeOptions.SingleStartInit), NoEnableDisable]
internal class Plugin
{
    public static Version Version { get; private set; } = Version.Zero;

    [Init]
    public Plugin(IPALogger logger, Config config, Zenjector zenjector, PluginMetadata pluginMetadata)
    {
        Version = pluginMetadata.HVersion;
        Logger.SetLogger(logger);
        Task.Run(() => InitAsync(logger, config, zenjector));
    }

    private async Task InitAsync(IPALogger logger, Config config, Zenjector zenjector)
    {
        if (!await CustomSaberUtils.LoadCustomSaberAssembly())
        {
            return;
        }

        zenjector.UseLogger(logger);

        zenjector.Install<AppInstaller>(Location.App, config.Generated<CSLConfig>());
        zenjector.Install<MenuInstaller>(Location.Menu);
        zenjector.Install<PlayerInstaller>(Location.Player);
        zenjector.Install<MultiPlayerInstaller>(Location.MultiPlayer);
    }
}
