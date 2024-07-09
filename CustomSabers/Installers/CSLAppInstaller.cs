using CustomSabersLite.Components.Managers;
using CustomSabersLite.Configuration;
using CustomSabersLite.Utilities;
using CustomSabersLite.Utilities.AssetBundles;
using Zenject;

namespace CustomSabersLite.Installers;

internal class CSLAppInstaller(CSLConfig config) : Installer
{
    private readonly CSLConfig config = config;

    public override void InstallBindings()
    {
        Container.Bind<PluginDirs>().AsSingle();

        Container.BindInstance(config);

        Container.BindInterfacesAndSelfTo<InternalResourcesProvider>().AsSingle();

        Container.BindInterfacesAndSelfTo<CacheManager>().AsSingle();
        Container.Bind<BundleLoader>().AsSingle();
        Container.Bind<SaberLoader>().AsSingle();
        Container.Bind<WhackerLoader>().AsSingle();
        Container.Bind<CustomSabersLoader>().AsSingle();

        Container.BindInterfacesAndSelfTo<SaberInstanceManager>().AsSingle();
        Container.Bind<SaberFactory>().AsSingle();
    }
}
