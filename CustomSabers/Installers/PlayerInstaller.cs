using System.Threading;
using CustomSabersLite.Components;
using CustomSabersLite.Configuration;
using CustomSabersLite.Services;
using JetBrains.Annotations;
using SiraUtil.Sabers;
using Zenject;

namespace CustomSabersLite.Installers;

[UsedImplicitly]
internal class PlayerInstaller : Installer
{
    private readonly PluginConfig config;
    private readonly SaberFactory saberFactory;
    
    private PlayerInstaller(PluginConfig config, SaberFactory saberFactory)
    {
        this.config = config;
        this.saberFactory = saberFactory;
    }
    
    public override void InstallBindings()
    {
        if (!config.Enabled)
        {
            Logger.Debug("Custom Sabers is disabled - will not run");
            return;
        }

        Container.BindInterfacesAndSelfTo<SaberEventService>().AsTransient();
        Container.BindInstance(saberFactory.InstantiateCurrentSabers(CancellationToken.None)).AsSingle();
        
        // This replaces the default sabers
        Container.BindInstance(SaberModelRegistration.Create<LiteSaberModelController>(5));
    }
}
