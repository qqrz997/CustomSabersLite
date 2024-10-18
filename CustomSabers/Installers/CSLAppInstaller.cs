using CustomSabersLite.Components.Managers;
using CustomSabersLite.Configuration;
using CustomSabersLite.UI.Managers;
using CustomSabersLite.Utilities;
using Zenject;

namespace CustomSabersLite.Installers;

internal class CSLAppInstaller(CSLConfig config) : Installer
{
    private readonly CSLConfig config = config;

    public override void InstallBindings()
    {
        Container.BindInstance(config);

        Container.BindInterfacesAndSelfTo<InternalResourcesProvider>().AsSingle();

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
