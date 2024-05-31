using CustomSabersLite.Components.Game;
using CustomSabersLite.Components.Managers;
using CustomSabersLite.Configuration;
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

        private LiteSaber leftSaber;
        private LiteSaber rightSaber;

        // using these for now until i figure out how to get the actual physical position on the ui view
        private readonly Vector3 leftPreviewSaberPosition = new Vector3(0.56f, 1.0f, 4.2f);
        private readonly Vector3 rightPreviewSaberPosition = new Vector3(1.25f, 1.0f, 4.05f);
        private readonly Quaternion previewRotation = Quaternion.Euler(270f, 100f, 0f);

        public async Task GeneratePreview(CancellationToken token)
        {
            SetPreviewActive(false);
            leftSaber?.gameObject.Destroy();
            rightSaber?.gameObject.Destroy();

            if (string.IsNullOrWhiteSpace(config.CurrentlySelectedSaber)) return;

            Task loadSabers = saberSet.InstantiateSabers(config.CurrentlySelectedSaber);
            while (!loadSabers.IsCompleted)
            {
                token.ThrowIfCancellationRequested();
                await Task.Delay(100);
            }
            await loadSabers;

            leftSaber = saberSet.CustomSaberForSaberType(SaberType.SaberA);
            rightSaber = saberSet.CustomSaberForSaberType(SaberType.SaberB);
            if (!leftSaber || !rightSaber) return;

            SetColor();


            leftSaber.gameObject.transform.SetPositionAndRotation(leftPreviewSaberPosition, previewRotation);
            rightSaber.gameObject.transform.SetPositionAndRotation(rightPreviewSaberPosition, previewRotation);

            SetPreviewActive(true);
        }

        public void SetPreviewActive(bool active)
        {
            leftSaber?.gameObject.SetActive(active);
            rightSaber?.gameObject.SetActive(active);
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
            leftSaber?.SetColor(leftColor);
            rightSaber?.SetColor(rightColor);
        }
    }
}
