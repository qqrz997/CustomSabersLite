using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace CustomSabersLite.UI
{
    internal class CSLViewManager : MonoBehaviour
    {
        private GameplaySetupViewController gameplaySetupViewController;
        private GameplaySetupTab customSabersTab;

        [Inject]
        public void Construct(GameplaySetupViewController gameplaySetupViewController, GameplaySetupTab customSabersTab)
        {
            this.gameplaySetupViewController = gameplaySetupViewController;
            this.customSabersTab = customSabersTab;
        }

        public void Awake()
        {
            gameplaySetupViewController.didActivateEvent += ParentViewControllerDidActivate;
        }

        public void OnDestroy()
        {
            gameplaySetupViewController.didActivateEvent -= ParentViewControllerDidActivate;
        }

        private void ParentViewControllerDidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
        {
            StartCoroutine(WaitForSabersTabEnabled());
        }

        private IEnumerator WaitForSabersTabEnabled()
        {
            yield return new WaitUntil(() => { return customSabersTab.Root.activeInHierarchy; });

            customSabersTab.TabWasActivated();
        }
    }
}
