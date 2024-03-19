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

            if (config.CurrentlySelectedSaber != "Default")
            {
                // Create the custom sabers
                Container.Bind<CSLSaberSet>().FromNewComponentOnNewGameObject().AsSingle().NonLazy();

                // This replaces the default sabers
                Container.BindInstance(SaberModelRegistration.Create<CSLSaberModelController>(5));
            }
            else
            {
                Container.Bind<DefaultSaberSetter>().FromNewComponentOnNewGameObject().AsSingle().NonLazy();
            }
        }
    }
}
