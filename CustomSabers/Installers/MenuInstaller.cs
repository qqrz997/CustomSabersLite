using CustomSabersLite.UI;
using CustomSabersLite.UI.Managers;
using CustomSabersLite.UI.Views;
using CustomSabersLite.Utilities;
using System;
using UnityEngine;
using Zenject;

namespace CustomSabersLite.Installers;

internal class MenuInstaller : Installer
{
    public override void InstallBindings()
    {
        Container.BindInterfacesAndSelfTo<MenuPointers>().AsSingle();

        // View controllers
        Container.Bind<SaberListViewController>().FromNewComponentAsViewController().AsSingle();
        Container.Bind<SaberSettingsViewController>().FromNewComponentAsViewController().AsSingle();
        Container.Bind<CSLFlowCoordinator>().FromNewComponentOnNewGameObject().AsSingle();

        Container.BindInterfacesAndSelfTo<GameplaySetupTab>().AsSingle();
        //Container.Bind<TabTest>().FromNewComponentAsViewController().AsSingle();

        // UI managers
        Container.BindInterfacesTo<GameplaySetupTabController>().AsSingle();
        Container.BindInterfacesTo<MenuButtonManager>().AsSingle();
        Container.BindInterfacesAndSelfTo<SaberPreviewManager>().AsSingle();

        Container.Bind<MenuSaberManager>().AsSingle();
        Container.Bind<MenuSaber>().AsTransient();
        Container.BindFactory<Transform, SaberType, MenuSaber, MenuSaber.Factory>();
        Container.Bind<BasicPreviewSaberManager>().AsSingle();
        Container.Bind<BasicPreviewTrailManager>().AsSingle();

        var time = IPA.Utilities.Utils.CanUseDateTimeNowSafely ? DateTime.Now : DateTime.UtcNow;
        if (time.Month == 4 && time.Day == 1)
        {
            Container.BindInterfacesTo<Jester>().AsSingle();
        }
    }
}
