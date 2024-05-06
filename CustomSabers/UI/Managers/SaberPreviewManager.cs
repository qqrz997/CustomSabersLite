using CustomSabersLite.Components;
using CustomSabersLite.Components.Interfaces;
using CustomSabersLite.Configuration;
using UnityEngine;

namespace CustomSabersLite.UI.Managers
{
    internal class SaberPreviewManager
    {
        private readonly ISaberSet saberSet;
        private readonly CSLConfig config;

        public SaberPreviewManager(ISaberSet saberSet, CSLConfig config)
        {
            this.saberSet = saberSet;
            this.config = config;
        }

        private CSLSaber LeftSaber { get => saberSet.CustomSaberForSaberType(SaberType.SaberA); }
        private CSLSaber RightSaber { get => saberSet.CustomSaberForSaberType(SaberType.SaberB); }

        // using these for now until i figure out how to get the actual physical position on the ui view
        private readonly Vector3 leftPreviewSaberPosition = new Vector3(0.56f, 1.0f, 4.2f);
        private readonly Vector3 rightPreviewSaberPosition = new Vector3(1.25f, 1.0f, 4.05f);
        private readonly Quaternion previewRotation = Quaternion.Euler(270f, 100f, 0f);

        public void GeneratePreview(Transform leftSaberParent, Transform rightSaberParent)
        {
            if (config.CurrentlySelectedSaber == "Default") return;

            MoveSaber(LeftSaber, leftSaberParent, leftPreviewSaberPosition, true);
            MoveSaber(RightSaber, rightSaberParent, rightPreviewSaberPosition, true);

            SetupSaber(LeftSaber, SaberType.SaberA);
            SetupSaber(RightSaber, SaberType.SaberB);
        }

        public void SetPreviewActive(bool active)
        {
            if (config.CurrentlySelectedSaber == "Default") return;

            LeftSaber.gameObject.SetActive(active);
            RightSaber.gameObject.SetActive(active);
        }

        private void MoveSaber(CSLSaber saber, Transform parent, Vector3 overridePosition, bool useOverride = false)
        {
            if (useOverride)
            {
                saber.transform.SetPositionAndRotation(overridePosition, previewRotation);
            }
            saber.gameObject.SetActive(true);
        }

        private void SetupSaber(CSLSaber saber, SaberType saberType)
        {
            Color color = saberType == SaberType.SaberA ? new Color(0.784f, 0.078f, 0.078f, 1f) : new Color(0.157f, 0.557f, 0.824f, 1f);
            saber.SetColor(color);
        }
    }
}
