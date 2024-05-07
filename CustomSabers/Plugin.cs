using IPA;
using IPALogger = IPA.Logging.Logger;
using IPA.Config.Stores;
using Config = IPA.Config.Config;
using CustomSabersLite.Configuration;
using CustomSabersLite.Utilities;
using SiraUtil.Zenject;
using CustomSabersLite.Installers;


namespace CustomSabersLite
{
    [Plugin(RuntimeOptions.SingleStartInit)]
    internal class Plugin
    {
        public static string Version => "0.9.1";

        [Init]
        public async void Init(IPALogger logger, Config config, Zenjector zenjector)
        {
            Logger.Log = logger;
            
            CSLConfig pluginConfig = config.Generated<CSLConfig>();

            if (!await CSLUtils.LoadCustomSaberAssembly())
            {
                return;
            }

            zenjector.UseLogger(logger);

            zenjector.Install<CSLAppInstaller>(Location.App, logger, pluginConfig);
            zenjector.Install<CSLMenuInstaller>(Location.Menu);
            zenjector.Install<CSLGameInstaller>(Location.Player);
        }
    }
}
