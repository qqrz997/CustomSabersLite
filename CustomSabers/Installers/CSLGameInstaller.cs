using CustomSabersLite.Components;
using SiraUtil.Sabers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zenject;

namespace CustomSabersLite.Installers
{
    internal class CSLGameInstaller : Installer
    {
        public override void InstallBindings()
        {
            Plugin.Log.Info("Installing Game Bindings");

            Container.BindInstance(SaberModelRegistration.Create<CSLSaberModelController>(5)).AsSingle();
        }
    }
}
