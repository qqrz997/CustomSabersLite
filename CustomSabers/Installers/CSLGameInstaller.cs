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

            Container.Bind<TrailManager>().AsTransient();
            Container.BindInterfacesAndSelfTo<EventManagerManager>().AsTransient();

            if (config.CurrentlySelectedSaber != null)
            {
                // This replaces the default sabers
                Container.BindInterfacesAndSelfTo<LevelSaberManager>().AsSingle();
                Container.BindInstance(SaberModelRegistration.Create<LiteSaberModelController>(5)).AsSingle();
            }
            else
            {
                Container.BindInterfacesTo<DefaultSaberSetter>().AsSingle();
            }
        }
    }
}
