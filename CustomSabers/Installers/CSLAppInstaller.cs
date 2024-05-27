using CustomSabersLite.Components.Managers;
using CustomSabersLite.Configuration;
using CustomSabersLite.Utilities.AssetBundles;
using Zenject;

namespace CustomSabersLite.Installers
{
    internal class CSLAppInstaller : Installer
    {
        private readonly IPA.Logging.Logger logger;
        private readonly CSLConfig config;

        public CSLAppInstaller(IPA.Logging.Logger logger, CSLConfig config)
        {
            this.logger = logger;
            this.config = config;
        }

        public override void InstallBindings()
        {
            Container.BindInstance(logger).AsSingle();

            Container.Bind<PluginDirs>().AsSingle();

            Container.BindInstance(config);

            Container.BindInterfacesAndSelfTo<CacheManager>().AsSingle();
            Container.Bind<BundleLoader>().AsSingle();
            Container.Bind<CustomSaberLoader>().AsSingle();
            Container.Bind<WhackerLoader>().AsSingle();

            Container.Bind<SaberInstanceManager>().AsSingle();
            Container.Bind<LiteSaberSet>().AsSingle();
        }
    }
}
