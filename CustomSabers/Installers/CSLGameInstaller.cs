using CustomSabersLite.Components;
using CustomSabersLite.Configuration;
using CustomSabersLite.Utilities;
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

            Container.Bind<CustomTrailHandler>().AsSingle();
            Container.BindInterfacesAndSelfTo<EventManagerManager>().AsTransient();

            if (config.CurrentlySelectedSaber != "Default")
            {
                // This replaces the default sabers
                Container.BindInstance(SaberModelRegistration.Create<CSLSaberModelController>(5)).AsSingle();
            }

            Container.Bind<DefaultSaberSetter>().FromNewComponentOnNewGameObject().AsSingle().NonLazy();

            // Aprilfools
            Container.BindInterfacesTo<ExtraSaberManager>().AsSingle();
        }
    }
}
