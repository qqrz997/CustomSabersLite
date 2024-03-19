using CustomSabersLite.UI;
using CustomSabersLite.UI.Views;
using Zenject;

namespace CustomSabersLite.Installers
{
    internal class CSLMenuInstaller : Installer
    {
        public override void InstallBindings()
        {
            // View controllers
            Container.Bind<SaberListViewController>().FromNewComponentAsViewController().AsSingle();
            Container.Bind<SaberSettingsViewController>().FromNewComponentAsViewController().AsSingle();
            Container.Bind<TestViewController>().FromNewComponentAsViewController().AsSingle();
            Container.Bind<CSLFlowCoordinator>().FromNewComponentOnNewGameObject().AsSingle();

            Container.BindInterfacesAndSelfTo<GameplaySetupTab>().AsSingle();

            //UI managers
            Container.Bind<CSLViewManager>().FromNewComponentOnNewGameObject().AsSingle().NonLazy();
            Container.BindInterfacesTo<CSLMenuButton>().AsSingle();
        }
    }
}
