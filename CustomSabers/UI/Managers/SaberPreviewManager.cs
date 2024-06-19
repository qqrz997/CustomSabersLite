using CustomSabersLite.Components.Game;
using CustomSabersLite.Components.Managers;
using CustomSabersLite.Configuration;
using CustomSabersLite.Data;
using CustomSabersLite.Utilities.Extensions;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace CustomSabersLite.UI.Managers
{
    internal class SaberPreviewManager
    {
        private readonly LiteSaberSet saberSet;
        private readonly CSLConfig config;
        private readonly ColorSchemesSettings colorSchemesSettings;

        public SaberPreviewManager(LiteSaberSet saberSet, CSLConfig config, ColorSchemesSettings colorSchemesSettings)
        {
            this.saberSet = saberSet;
            this.config = config;
            this.colorSchemesSettings = colorSchemesSettings;
        }

        private LiteSaber leftPreviewSaber = null;
        private LiteSaber rightPreviewSaber = null;

        // using these for now until i figure out how to get the actual physical position on the ui view
        private readonly Vector3 leftPreviewSaberPosition = new Vector3(0.56f, 1.0f, 4.2f);
        private readonly Vector3 rightPreviewSaberPosition = new Vector3(1.25f, 1.0f, 4.05f);
        private readonly Quaternion previewRotation = Quaternion.Euler(270f, 100f, 0f);

        public async Task GeneratePreview(CancellationToken token)
        {
            if (leftPreviewSaber)
            {
                leftPreviewSaber.gameObject.Destroy();
                leftPreviewSaber = null;
            }
            if (rightPreviewSaber)
            {
                rightPreviewSaber.gameObject.Destroy();
                rightPreviewSaber = null;
            }

            if (string.IsNullOrWhiteSpace(config.CurrentlySelectedSaber))
            {
                return;
            }

            await saberSet.GetSaberData(config.CurrentlySelectedSaber);
            token.ThrowIfCancellationRequested();

            await saberSet.SetSabers(config.CurrentlySelectedSaber);

            leftPreviewSaber = saberSet.NewSaberForSaberType(SaberType.SaberA);
            rightPreviewSaber = saberSet.NewSaberForSaberType(SaberType.SaberB);

            if (!leftPreviewSaber || !rightPreviewSaber)
            {
                Logger.Warn("Something went wrong when setting the current saber");
                return;
            }

            SetColor();

            leftPreviewSaber.gameObject.transform.SetPositionAndRotation(leftPreviewSaberPosition, previewRotation);
            rightPreviewSaber.gameObject.transform.SetPositionAndRotation(rightPreviewSaberPosition, previewRotation);
        }

        public void SetPreviewActive(bool active)
        {
            leftPreviewSaber?.gameObject.SetActive(active);
            rightPreviewSaber?.gameObject.SetActive(active);
        }

        public void SetColor()
        {
            if (config.EnableCustomColorScheme)
            {
                SetColor(config.LeftSaberColor, config.RightSaberColor);
            }
            else
            {
                ColorScheme selectedColorScheme = colorSchemesSettings.GetSelectedColorScheme();
                SetColor(selectedColorScheme.saberAColor, selectedColorScheme.saberBColor);
            }
        }

        private void SetColor(Color leftColor, Color rightColor)
        {
            leftPreviewSaber?.SetColor(leftColor);
            rightPreviewSaber?.SetColor(rightColor);
        }
    }
}
