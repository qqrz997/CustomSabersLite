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
            Container.BindInstance(SaberModelRegistration.Create<CSLSaberModelController>(5)).AsSingle();

            Container.Bind<CustomTrailHandler>().AsSingle().NonLazy();

            Container.Bind<CSLSaberManager>().FromNewComponentOnNewGameObject().AsSingle().NonLazy();
        }
    }
}
