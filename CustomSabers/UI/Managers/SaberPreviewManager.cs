using CustomSabersLite.Components.Game;
using CustomSabersLite.Components.Managers;
using CustomSabersLite.Configuration;
using CustomSabersLite.Data;
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

        private LiteSaber LeftSaber => saberSet.CustomSaberForSaberType(SaberType.SaberA);
        private LiteSaber RightSaber => saberSet.CustomSaberForSaberType(SaberType.SaberB);

        // using these for now until i figure out how to get the actual physical position on the ui view
        private readonly Vector3 leftPreviewSaberPosition = new Vector3(0.56f, 1.0f, 4.2f);
        private readonly Vector3 rightPreviewSaberPosition = new Vector3(1.25f, 1.0f, 4.05f);
        private readonly Quaternion previewRotation = Quaternion.Euler(270f, 100f, 0f);

        public async Task GeneratePreview(CancellationToken token)
        {
            saberSet.DestroySabers();

            if (string.IsNullOrWhiteSpace(config.CurrentlySelectedSaber))
            {
                return;
            }

            await saberSet.GetSaberData(config.CurrentlySelectedSaber);
            token.ThrowIfCancellationRequested();

            await saberSet.SetSabers(config.CurrentlySelectedSaber);
            
            if (!LeftSaber || !RightSaber)
            {
                Logger.Warn("Something went wrong when setting the current saber");
                return;
            }

            SetColor();

            LeftSaber.gameObject.transform.SetPositionAndRotation(leftPreviewSaberPosition, previewRotation);
            RightSaber.gameObject.transform.SetPositionAndRotation(rightPreviewSaberPosition, previewRotation);
        }

        public void SetPreviewActive(bool active)
        {
            LeftSaber?.gameObject.SetActive(active);
            RightSaber?.gameObject.SetActive(active);
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
            LeftSaber?.SetColor(leftColor);
            RightSaber?.SetColor(rightColor);
        }
    }
}
