namespace CustomSabersLite.Installers;

internal class CSLMultiplayerInstaller : Zenject.Installer
{
    public override void InstallBindings() =>
        Container.BindInterfacesTo<PauseMenuHandlesFix>().AsSingle();
}
