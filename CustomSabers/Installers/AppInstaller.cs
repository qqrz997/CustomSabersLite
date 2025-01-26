﻿using CustomSabersLite.Configuration;
using CustomSabersLite.Menu;
using CustomSabersLite.Utilities.Services;
using Zenject;

namespace CustomSabersLite.Installers;

internal class AppInstaller(CslConfig config) : Installer
{
    private readonly CslConfig config = config;

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

        Container.BindInterfacesAndSelfTo<SaberPrefabCache>().AsSingle();
        Container.Bind<SaberFactory>().AsSingle();
        Container.Bind<TrailFactory>().AsSingle();
    }
}
