using CustomSabersLite.UI;
using Zenject;

namespace CustomSabersLite.Installers
{
    internal class CSLMenuInstaller : Installer
    {
        public override void InstallBindings()
        {
            Plugin.Log.Info("Installing Menu Bindings");

            Container.BindInterfacesAndSelfTo<GameplaySetupTab>().AsSingle();
            Container.Bind<SaberListViewController>().AsSingle();
        }
    }
}
