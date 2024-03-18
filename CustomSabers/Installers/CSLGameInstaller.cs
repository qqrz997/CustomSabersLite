using CustomSabersLite.Components;
using CustomSabersLite.Utilities;
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
            Logger.Info("Installing Game Bindings");

            Container.BindInstance(SaberModelRegistration.Create<CSLSaberModelController>(5)).AsSingle();

            Container.Bind<CustomTrailHandler>().AsSingle().NonLazy();


            Logger.Info("Creating saber manager");
            Container.Bind<CSLSaberManager>().FromNewComponentOnNewGameObject().AsSingle().NonLazy();
        }
    }
}
