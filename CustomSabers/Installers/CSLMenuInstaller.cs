using CustomSabersLite.UI.Managers;
using CustomSabersLite.UI.Views;
using CustomSabersLite.UI.Views.Saber_List;
using CustomSabersLite.Utilities;
using UnityEngine;
using Zenject;

namespace CustomSabersLite.Installers;

internal class CSLMenuInstaller : Installer
{
    public override void InstallBindings()
    {
        Container.BindInterfacesAndSelfTo<MenuPointerProvider>().AsSingle();

        // View controllers
        Container.Bind<SaberListViewController>().FromNewComponentAsViewController().AsSingle();
        Container.Bind<SaberSettingsViewController>().FromNewComponentAsViewController().AsSingle();
        Container.Bind<CSLFlowCoordinator>().FromNewComponentOnNewGameObject().AsSingle();

        Container.BindInterfacesAndSelfTo<GameplaySetupTab>().AsSingle();
        //Container.Bind<TabTest>().FromNewComponentAsViewController().AsSingle();

        // UI managers
        Container.Bind<ViewControllerManager>().FromNewComponentOnNewGameObject().AsSingle().NonLazy();
        Container.BindInterfacesTo<MenuButtonManager>().AsSingle();
        Container.BindInterfacesAndSelfTo<SaberPreviewManager>().AsSingle();

        Container.BindInterfacesAndSelfTo<MenuSaberManager>().AsSingle();
        Container.Bind<MenuSaber>().AsTransient();
        Container.BindFactory<Transform, SaberType, MenuSaber, MenuSaber.Factory>();
        Container.Bind<BasicPreviewSaberManager>().AsSingle();
        Container.Bind<BasicPreviewTrailManager>().AsSingle();
    }
}
