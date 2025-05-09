﻿using System;
using BeatSaberMarkupLanguage.Tags;
using BeatSaberMarkupLanguage.TypeHandlers;
using CustomSabersLite.Menu;
using CustomSabersLite.Menu.Components;
using CustomSabersLite.Menu.Views;
using CustomSabersLite.Misc;
using CustomSabersLite.Services;
using Zenject;

namespace CustomSabersLite.Installers;

internal class MenuInstaller : Installer
{
    public override void InstallBindings()
    {
        Container.BindInterfacesAndSelfTo<MenuPointers>().AsSingle();
        
        // Custom tags
        Container.Bind<BSMLTag>().To<ToggleableSliderTag>().AsSingle();
        Container.Bind<BSMLTag>().To<BsInputFieldTag>().AsSingle();
        Container.Bind<BSMLTag>().To<SaberListTag>().AsSingle();
        Container.Bind<BSMLTag>().To<FavouriteToggleTag>().AsSingle();
        Container.Bind<BSMLTag>().To<ClickableIconTag>().AsSingle();
        
        Container.Bind<TypeHandler>().To<ToggleableSliderHandler>().AsSingle();
        Container.Bind<TypeHandler>().To<SaberListTableDataHandler>().AsSingle();
        Container.Bind<TypeHandler>().To<FavouriteToggleHandler>().AsSingle();
        Container.Bind<TypeHandler>().To<ClickableIconHandler>().AsSingle();
        
        // View controllers
        Container.Bind<SaberListViewController>().FromNewComponentAsViewController().AsSingle();
        Container.Bind<SaberSettingsViewController>().FromNewComponentAsViewController().AsSingle();
        Container.Bind<CslFlowCoordinator>().FromNewComponentOnNewGameObject().AsSingle();

        Container.BindInterfacesAndSelfTo<GameplaySetupTab>().AsSingle();

        // Menu managers
        Container.BindInterfacesTo<GameplaySetupTabController>().AsSingle();
        Container.BindInterfacesTo<MenuButtonManager>().AsSingle();
        Container.BindInterfacesAndSelfTo<SaberPreviewManager>().AsSingle();
        
        Container.Bind<SaberListManager>().AsSingle();

        Container.Bind<MenuSaberManager>().AsSingle();
        Container.Bind<MenuSaber>().WithId(SaberType.SaberA).AsCached();
        Container.Bind<MenuSaber>().WithId(SaberType.SaberB).AsCached();
        
        Container.BindInterfacesAndSelfTo<StaticPreviewManager>().AsSingle();
        Container.Bind<StaticPreviewSaberManager>().AsSingle();
        Container.Bind<StaticPreviewTrailManager>().AsSingle();
        Container.Bind<StaticPreviewTrail>().WithId(SaberType.SaberA).AsCached();
        Container.Bind<StaticPreviewTrail>().WithId(SaberType.SaberB).AsCached();

        var time = IPA.Utilities.Utils.CanUseDateTimeNowSafely ? DateTime.Now : DateTime.UtcNow;
        if (time is { Year: 2025, Month: 4, Day: 1 })
        {
            Container.BindInterfacesTo<Jester>().AsSingle();
        }
    }
}
