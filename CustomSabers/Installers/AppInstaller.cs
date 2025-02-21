using CustomSabersLite.Configuration;
using CustomSabersLite.Utilities.Common;
using CustomSabersLite.Utilities.Services;
using Zenject;

namespace CustomSabersLite.Installers;

internal class AppInstaller(CslConfig config) : Installer
{
    private readonly CslConfig config = config;

    public override void InstallBindings()
    {
        Container.BindInterfacesAndSelfTo<FavouritesManager>().AsSingle();
        Container.BindInterfacesAndSelfTo<GameResourcesProvider>().AsSingle();
        Container.BindInterfacesAndSelfTo<MetadataCacheLoader>().AsSingle();
        Container.BindInterfacesAndSelfTo<DirectoryManager>().AsSingle();
        Container.Bind<SaberMetadataCacheMigrationManager>().AsSingle();
        Container.Bind<ITimeService>().To<UtcTimeService>().AsSingle();
        Container.Bind<SaberMetadataConverter>().AsSingle();
        Container.Bind<CustomSabersLoader>().AsSingle();
        Container.Bind<SaberMetadataCache>().AsSingle();
        Container.Bind<SaberPrefabCache>().AsSingle();
        Container.Bind<WhackerLoader>().AsSingle();
        Container.Bind<SaberFactory>().AsSingle();
        Container.Bind<TrailFactory>().AsSingle();
        Container.Bind<SaberLoader>().AsSingle();
        Container.Bind<SpriteCache>().AsSingle();
        Container.Bind<FileManager>().AsSingle();
        Container.BindInstance(config);
    }
}
