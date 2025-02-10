using System;
using BeatSaberMarkupLanguage.Tags;
using BeatSaberMarkupLanguage.TypeHandlers;
using CustomSabersLite.Menu;
using CustomSabersLite.Menu.Components;
using CustomSabersLite.Menu.Views;
using CustomSabersLite.Misc;
using CustomSabersLite.Utilities.Services;
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
        // Container.Bind<TabTest>().FromNewComponentAsViewController().AsSingle();
        Container.Bind<SaberListViewController>().FromNewComponentAsViewController().AsSingle();
        Container.Bind<SaberSettingsViewController>().FromNewComponentAsViewController().AsSingle();
        Container.Bind<CslFlowCoordinator>().FromNewComponentOnNewGameObject().AsSingle();

        Container.BindInterfacesAndSelfTo<GameplaySetupTab>().AsSingle();

        // Menu managers
        Container.BindInterfacesTo<GameplaySetupTabController>().AsSingle();
        Container.BindInterfacesTo<MenuButtonManager>().AsSingle();
        Container.BindInterfacesAndSelfTo<SaberPreviewManager>().AsSingle();

        Container.Bind<MenuSaberManager>().AsSingle();
        Container.Bind<MenuSaber>().WithId(SaberType.SaberA).AsCached();
        Container.Bind<MenuSaber>().WithId(SaberType.SaberB).AsCached();
        
        Container.Bind<BasicPreviewSaberManager>().AsSingle();
        Container.Bind<BasicPreviewTrailManager>().AsSingle();
        Container.Bind<BasicPreviewTrail>().WithId(SaberType.SaberA).AsCached();
        Container.Bind<BasicPreviewTrail>().WithId(SaberType.SaberB).AsCached();

        var time = IPA.Utilities.Utils.CanUseDateTimeNowSafely ? DateTime.Now : DateTime.UtcNow;
        if (time is { Year: 2025, Month: 4, Day: 1 })
        {
            Container.BindInterfacesTo<Jester>().AsSingle();
        }
    }
}
