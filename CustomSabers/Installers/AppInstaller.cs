using CustomSabersLite.Configuration;
using CustomSabersLite.Menu.Views;
using CustomSabersLite.Services;
using Zenject;

namespace CustomSabersLite.Installers;

internal class AppInstaller : Installer
{
    private readonly PluginConfigModel configModel;

    public AppInstaller(PluginConfigModel configModel)
    {
        this.configModel = configModel;
    }

    public override void InstallBindings()
    {
        // Configs
        Container.BindInstance(configModel);
        Container.Bind<PluginConfig>().FromInstance(PluginConfig.FromJson(configModel)).AsSingle();
        Container.BindInterfacesTo<PluginConfigManager>().AsSingle();
        
        // Services
        Container.BindInterfacesAndSelfTo<GameResourcesProvider>().AsSingle();
        Container.BindInterfacesAndSelfTo<SaberFoldersManager>().AsSingle();
        Container.BindInterfacesAndSelfTo<MetadataCacheLoader>().AsSingle();
        Container.BindInterfacesAndSelfTo<DirectoryManager>().AsSingle();
        Container.Bind<SaberMetadataConverter>().AsSingle();
        Container.Bind<SaberMetadataCacheMigrationManager>().AsSingle();
        Container.Bind<ITimeService>().To<UtcTimeService>().AsSingle();
        
        // Cache
        Container.BindInterfacesAndSelfTo<FavouritesManager>().AsSingle();
        Container.BindInterfacesAndSelfTo<SaberPrefabCache>().AsSingle();
        Container.Bind<SaberMetadataCache>().AsSingle();
        Container.Bind<SpriteCache>().AsSingle();
        
        // Asset loaders
        Container.Bind<CustomSabersLoader>().AsSingle();
        Container.Bind<WhackerLoader>().AsSingle();
        Container.Bind<SaberLoader>().AsSingle();
        
        Container.Bind<SaberFactory>().AsSingle();
        Container.Bind<TrailFactory>().AsSingle();
        Container.Bind<FileManager>().AsSingle();
    }
}
