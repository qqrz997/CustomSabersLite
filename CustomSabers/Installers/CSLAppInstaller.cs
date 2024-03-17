using CustomSabersLite.Utilities;
using IPA.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zenject;

namespace CustomSabersLite.Installers
{
    internal class CSLAppInstaller : Installer
    {
        private readonly Logger logger;

        public CSLAppInstaller(Logger logger)
        {
            this.logger = logger;
        }

        public override void InstallBindings()
        {
            Plugin.Log.Info("Installing App Bindings");

            Container.Bind<PluginDirs>().AsSingle();

            Container.BindInterfacesAndSelfTo<CSLAssetLoader>().AsSingle();
        }
    }
}
