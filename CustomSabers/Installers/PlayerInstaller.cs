using CustomSabersLite.Components;
using CustomSabersLite.Configuration;
using CustomSabersLite.Utilities.Services;
using SiraUtil.Sabers;
using Zenject;

namespace CustomSabersLite.Installers;

internal class PlayerInstaller : Installer
{
    public override void InstallBindings()
    {
        var config = Container.Resolve<CSLConfig>();

        if (!config.Enabled)
        {
            Logger.Debug("Custom Sabers is disabled - will not run");
            return;
        }

        Container.BindInterfacesAndSelfTo<SaberEventService>().AsTransient();
        Container.Bind<LevelSaberManager>().AsSingle();

        // This replaces the default sabers
        Container.BindInstance(SaberModelRegistration.Create<LiteSaberModelController>(5));
    }
}
