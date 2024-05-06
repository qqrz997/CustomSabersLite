using CustomSabersLite.UI.Views;
using System.Collections;
using UnityEngine;
using Zenject;

namespace CustomSabersLite.UI.Managers
{
    internal class ViewControllerManager : MonoBehaviour
    {
        private GameplaySetupViewController gameplaySetupViewController;
        private GameplaySetupTab customSabersTab;
        private SaberSettingsViewController saberSettingsViewController;
        private SaberPreviewManager previewManager;

        [Inject]
        public void Construct(GameplaySetupViewController gameplaySetupViewController, GameplaySetupTab customSabersTab, SaberSettingsViewController saberSettingsViewController, SaberPreviewManager previewManager)
        {
            this.gameplaySetupViewController = gameplaySetupViewController;
            this.customSabersTab = customSabersTab;
            this.saberSettingsViewController = saberSettingsViewController;
            this.previewManager = previewManager;
        }

        public void Awake()
        {
            gameplaySetupViewController.didActivateEvent += GameplaySetupActivated;
            saberSettingsViewController.didActivateEvent += SaberSettingsActivated;
            saberSettingsViewController.didDeactivateEvent += SaberSettingsDeactivated;
        }

        public void OnDestroy()
        {
            gameplaySetupViewController.didActivateEvent -= GameplaySetupActivated;
            saberSettingsViewController.didActivateEvent -= SaberSettingsActivated;
            saberSettingsViewController.didDeactivateEvent -= SaberSettingsDeactivated;
        }

        private void GameplaySetupActivated(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
        {
            StartCoroutine(WaitForSabersTabEnabled());
        }

        private IEnumerator WaitForSabersTabEnabled()
        {
            yield return new WaitUntil(() => customSabersTab.Root.activeInHierarchy);

            customSabersTab.Activated();
        }

        private void SaberSettingsActivated(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
        {
            saberSettingsViewController.Activated();
            previewManager.SetPreviewActive(true);
        }

        private void SaberSettingsDeactivated(bool removedFromHierarchy, bool screenSystemDisabling)
        {
            previewManager.SetPreviewActive(false);
        }
    }
}
