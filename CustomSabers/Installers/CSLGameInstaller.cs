using CustomSabersLite.Components.Game;
using CustomSabersLite.Configuration;
using SiraUtil.Sabers;
using Zenject;

namespace CustomSabersLite.Installers
{
    internal class CSLGameInstaller : Installer
    {
        public override void InstallBindings()
        {
            CSLConfig config = Container.Resolve<CSLConfig>();

            if (!config.Enabled)
            {
                Logger.Debug("Custom Sabers is disabled - will not run");
                return;
            }

            Container.Bind<LevelSaberManager>().AsSingle();
            Container.Bind<TrailManager>().AsTransient();
            Container.BindInterfacesAndSelfTo<EventManagerManager>().AsTransient();

            Logger.Info($"Selected saber: {config.CurrentlySelectedSaber}");

            if (config.CurrentlySelectedSaber != null)
            {
                // This replaces the default sabers
                Container.BindInstance(SaberModelRegistration.Create<LiteSaberModelController>(5)).AsSingle();
            }
            else
            {
                Container.BindInterfacesTo<DefaultSaberSetter>().AsSingle();
            }

            // Aprilfools
            Container.BindInterfacesTo<ExtraSaberManager>().AsSingle();
        }
    }
}
