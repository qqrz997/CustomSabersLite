using System;
using BeatSaberMarkupLanguage.Tags;
using BeatSaberMarkupLanguage.TypeHandlers;
using CustomSabersLite.Menu;
using CustomSabersLite.Menu.Components;
using CustomSabersLite.Menu.Views;
using CustomSabersLite.Misc;
using CustomSabersLite.Utilities.Services;
using UnityEngine;
using Zenject;

namespace CustomSabersLite.Installers;

internal class MenuInstaller : Installer
{
    public override void InstallBindings()
    {
        Container.BindInterfacesAndSelfTo<MenuPointers>().AsSingle();
        
        // Custom tags
        Container.Bind<BSMLTag>().To<ToggleableSliderTag>().AsSingle();
        Container.Bind<BSMLTag>().To<ToggleButtonTag>().AsSingle();
        Container.Bind<BSMLTag>().To<BsInputFieldTag>().AsSingle();
        Container.Bind<BSMLTag>().To<SaberListTag>().AsSingle();
        Container.Bind<TypeHandler>().To<ToggleableSliderHandler>().AsSingle();
        Container.Bind<TypeHandler>().To<ToggleButtonHandler>().AsSingle();
        Container.Bind<TypeHandler>().To<SaberListTableDataHandler>().AsSingle();
        
        // View controllers
        Container.Bind<SaberListViewController>().FromNewComponentAsViewController().AsSingle();
        Container.Bind<SaberSettingsViewController>().FromNewComponentAsViewController().AsSingle();
        Container.Bind<CSLFlowCoordinator>().FromNewComponentOnNewGameObject().AsSingle();

        Container.BindInterfacesAndSelfTo<GameplaySetupTab>().AsSingle();
        //Container.Bind<TabTest>().FromNewComponentAsViewController().AsSingle();

        // Menu managers
        Container.BindInterfacesTo<GameplaySetupTabController>().AsSingle();
        Container.BindInterfacesTo<MenuButtonManager>().AsSingle();
        Container.BindInterfacesAndSelfTo<SaberPreviewManager>().AsSingle();

        Container.Bind<MenuSaberManager>().AsSingle();
        Container.Bind<MenuSaber>().AsTransient();
        Container.BindFactory<Transform, SaberType, MenuSaber, MenuSaber.Factory>();
        Container.Bind<BasicPreviewSaberManager>().AsSingle();
        Container.Bind<BasicPreviewTrailManager>().AsSingle();

        var time = IPA.Utilities.Utils.CanUseDateTimeNowSafely ? DateTime.Now : DateTime.UtcNow;
        if (time is { Month: 4, Day: 1 })
        {
            Container.BindInterfacesTo<Jester>().AsSingle();
        }
    }
}
