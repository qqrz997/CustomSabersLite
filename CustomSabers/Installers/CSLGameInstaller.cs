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

            Container.Bind<CustomTrailHandler>().AsSingle();
            Container.BindInterfacesAndSelfTo<EventManagerManager>().AsSingle();

            // Create the custom sabers
            Container.BindInterfacesAndSelfTo<CSLSaberSet>().AsCached().NonLazy();

            if (config.CurrentlySelectedSaber != "Default")
            {
                // This replaces the default sabers
                Container.BindInstance(SaberModelRegistration.Create<CSLSaberModelController>(5));
            }
            else
            {
                Container.Bind<DefaultSaberSetter>().FromNewComponentOnNewGameObject().AsSingle().NonLazy();
            }

            Container.BindInterfacesTo<ExtraSaberManager>().AsSingle();
        }
    }
}
