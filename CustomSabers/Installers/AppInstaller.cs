using CustomSabersLite.Configuration;
using CustomSabersLite.UI;
using CustomSabersLite.Utilities;
using CustomSabersLite.Utilities.Services;
using Zenject;

namespace CustomSabersLite.Installers;

internal class AppInstaller(CSLConfig config) : Installer
{
    private readonly CSLConfig config = config;

    public override void InstallBindings()
    {
        Container.BindInstance(config);

        Container.Bind<ITimeService>().To<UtcTimeService>().AsSingle();

        Container.BindInterfacesAndSelfTo<GameResourcesProvider>().AsSingle();

        Container.Bind<SaberMetadataCacheMigrationManager>().AsSingle();
        Container.BindInterfacesAndSelfTo<SaberMetadataCache>().AsSingle();
        Container.Bind<SaberListManager>().AsSingle();

        Container.Bind<SpriteCache>().AsSingle();

        Container.Bind<SaberLoader>().AsSingle();
        Container.Bind<WhackerLoader>().AsSingle();
        Container.Bind<CustomSabersLoader>().AsSingle();

        Container.BindInterfacesAndSelfTo<SaberInstanceManager>().AsSingle();
        Container.Bind<SaberFactory>().AsSingle();
        Container.Bind<TrailFactory>().AsSingle();
    }
}
