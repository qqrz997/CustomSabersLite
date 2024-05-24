using CustomSabersLite.Components;
using CustomSabersLite.Configuration;
using CustomSabersLite.Managers;
using CustomSabersLite.Utilities;
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
            Container.Bind<TrailUtils>().AsSingle();

            Container.BindInstance(config);

            Container.BindInterfacesAndSelfTo<CSLAssetLoader>().AsSingle();
            Container.BindInterfacesTo<BundleLoader>().AsSingle();
            Container.BindInterfacesTo<CustomSaberLoader>().AsSingle();
            Container.BindInterfacesTo<WhackerLoader>().AsSingle();

            Container.BindInterfacesAndSelfTo<SaberInstanceManager>().AsSingle();
            Container.BindInterfacesTo<CSLSaberSet>().AsSingle();
        }
    }
}
