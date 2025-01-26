using CustomSabersLite.Misc;
using Zenject;

namespace CustomSabersLite.Installers;

internal class MultiPlayerInstaller : Installer
{
    public override void InstallBindings()
    {
        Container.BindInterfacesTo<PauseMenuHandlesFix>().AsSingle();
    }
}
