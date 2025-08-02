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
        Container.BindInterfacesAndSelfTo<DirectoryManager>().AsSingle();
        
        // Cache
        Container.BindInterfacesAndSelfTo<FavouritesManager>().AsSingle();
        Container.Bind<SpriteCache>().AsSingle();
        
        Container.Bind<SaberFactory>().AsSingle();
    }
}
