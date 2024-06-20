using CustomSabersLite.UI.Managers;
using CustomSabersLite.UI.Views;
using Zenject;

namespace CustomSabersLite.Installers;

internal class CSLMenuInstaller : Installer
{
    public override void InstallBindings()
    {
        // View controllers
        Container.Bind<SaberListViewController>().FromNewComponentAsViewController().AsSingle();
        Container.Bind<SaberSettingsViewController>().FromNewComponentAsViewController().AsSingle();
        Container.Bind<CSLFlowCoordinator>().FromNewComponentOnNewGameObject().AsSingle();

        Container.BindInterfacesAndSelfTo<GameplaySetupTab>().AsSingle();

        //UI managers
        Container.Bind<ViewControllerManager>().FromNewComponentOnNewGameObject().AsSingle().NonLazy();
        Container.BindInterfacesTo<MenuButtonManager>().AsSingle();
        Container.Bind<SaberPreviewManager>().AsSingle();
    }
}
