using CustomSabersLite.Components.Game;
using CustomSabersLite.Components.Managers;
using CustomSabersLite.Configuration;
using CustomSabersLite.Utilities.Extensions;
using System.Threading.Tasks;
using UnityEngine;

namespace CustomSabersLite.UI.Managers
{
    internal class SaberPreviewManager
    {
        private readonly LiteSaberSet saberSet;
        private readonly CSLConfig config;

        public SaberPreviewManager(LiteSaberSet saberSet, CSLConfig config)
        {
            this.saberSet = saberSet;
            this.config = config;
        }

        private GameObject leftSaberObject;
        private GameObject rightSaberObject;

        // using these for now until i figure out how to get the actual physical position on the ui view
        private readonly Vector3 leftPreviewSaberPosition = new Vector3(0.56f, 1.0f, 4.2f);
        private readonly Vector3 rightPreviewSaberPosition = new Vector3(1.25f, 1.0f, 4.05f);
        private readonly Quaternion previewRotation = Quaternion.Euler(270f, 100f, 0f);

        public async Task GeneratePreview()
        {
            SetPreviewActive(false);

            if (config.CurrentlySelectedSaber is null) return;

            await saberSet.SetSabersFromPath(config.CurrentlySelectedSaber);
            LiteSaber leftSaber = saberSet.CustomSaberForSaberType(SaberType.SaberA);
            LiteSaber rightSaber = saberSet.CustomSaberForSaberType(SaberType.SaberB);
            if (!leftSaber || !rightSaber) return;

            if (leftSaberObject != null) GameObject.Destroy(leftSaberObject);
            leftSaberObject = GameObject.Instantiate(leftSaber.gameObject);
            if (rightSaberObject != null) GameObject.Destroy(rightSaberObject);
            rightSaberObject = GameObject.Instantiate(rightSaber.gameObject);
            SetupSaber(leftSaberObject, SaberType.SaberA);
            SetupSaber(rightSaberObject, SaberType.SaberB);
            MoveSaber(leftSaberObject, leftPreviewSaberPosition);
            MoveSaber(rightSaberObject, rightPreviewSaberPosition);

            SetPreviewActive(true);
        }

        public void SetPreviewActive(bool active)
        {
            leftSaberObject?.SetActive(active);
            rightSaberObject?.SetActive(active);
        }

        private void MoveSaber(GameObject saberObject, Vector3 overridePosition) => 
            saberObject.transform.SetPositionAndRotation(overridePosition, previewRotation);

        private void SetupSaber(GameObject saberObject, SaberType saberType)
        {
            LiteSaber saber = saberObject.TryGetComponentOrDefault<LiteSaber>();
            Color color = saberType == SaberType.SaberA ? new Color(0.784f, 0.078f, 0.078f, 1f) : new Color(0.157f, 0.557f, 0.824f, 1f);
            saber.SetColor(color);
        }
    }
}
