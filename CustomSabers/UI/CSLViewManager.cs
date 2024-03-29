using System.Collections;
using UnityEngine;
using Zenject;

namespace CustomSabersLite.UI
{
    internal class CSLViewManager : MonoBehaviour
    {
        private GameplaySetupViewController gameplaySetupViewController;
        private GameplaySetupTab customSabersTab;
        private SaberSettingsViewController saberSettingsViewController;

        [Inject]
        public void Construct(GameplaySetupViewController gameplaySetupViewController, GameplaySetupTab customSabersTab, SaberSettingsViewController saberSettingsViewController)
        {
            this.gameplaySetupViewController = gameplaySetupViewController;
            this.customSabersTab = customSabersTab;
            this.saberSettingsViewController = saberSettingsViewController;
        }

        public void Awake()
        {
            gameplaySetupViewController.didActivateEvent += GameplaySetupActivated;
            saberSettingsViewController.didActivateEvent += SaberSettingsActivated;
        }

        public void OnDestroy()
        {
            gameplaySetupViewController.didActivateEvent -= GameplaySetupActivated;
            saberSettingsViewController.didActivateEvent -= SaberSettingsActivated;
        }

        private void GameplaySetupActivated(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
        {
            StartCoroutine(WaitForSabersTabEnabled());
        }

        private IEnumerator WaitForSabersTabEnabled()
        {
            yield return new WaitUntil(() => { return customSabersTab.Root.activeInHierarchy; });

            customSabersTab.Activated();
        }

        private void SaberSettingsActivated(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
        {
            saberSettingsViewController.Activated();
        }
    }
}
