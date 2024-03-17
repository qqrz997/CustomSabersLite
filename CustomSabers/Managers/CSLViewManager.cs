using CustomSabersLite.UI;
using System;
using Zenject;

namespace CustomSabersLite.Managers
{
    internal class CSLViewManager : IInitializable, IDisposable
    {
        private GameplaySetupTab gameplaySetupTab;

        public CSLViewManager(GameplaySetupTab gameplaySetupTab)
        {
            this.gameplaySetupTab = gameplaySetupTab;
        }

        public void Initialize()
        {

        }

        public void Dispose()
        {

        }
    }
}
