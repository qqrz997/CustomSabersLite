using CustomSabersLite.Components.Managers;
using CustomSabersLite.Configuration;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace CustomSabersLite.UI.Managers;

internal class SaberPreviewManager
{
    private readonly SaberFactory saberFactory;
    private readonly CSLConfig config;
    private readonly ColorSchemesSettings colorSchemesSettings;

    private readonly PreviewSabers previewSabers;
    private readonly PreviewTrails previewTrails;

    // using these for now until i figure out how to get the actual physical position on the ui view
    //private readonly Vector3 leftPreviewSaberPosition = new(0.56f, 1.0f, 4.2f);
    //private readonly Vector3 rightPreviewSaberPosition = new(1.25f, 1.0f, 4.05f); 
    private readonly Vector3 leftPreviewSaberPosition = new(0.72f, 1.0f, 4.1f);
    private readonly Vector3 rightPreviewSaberPosition = new(1.06f, 1.0f, 4.02f);
    private readonly Quaternion leftPreviewRotation = Quaternion.Euler(270f, 103.25f, 0f);
    private readonly Quaternion rightPreviewRotation = Quaternion.Euler(270f, 283.25f, 0f);

    public SaberPreviewManager(SaberFactory saberFactory, CSLConfig config, ColorSchemesSettings colorSchemesSettings)
    {
        this.saberFactory = saberFactory;
        this.config = config;
        this.colorSchemesSettings = colorSchemesSettings;

        previewSabers = new();
        previewTrails = new();
        previewTrails.SetPosition(leftPreviewSaberPosition, rightPreviewSaberPosition, leftPreviewRotation, rightPreviewRotation);
    }

    public async Task GeneratePreview(CancellationToken token)
    {
        SetPreviewActive(false);

        if (string.IsNullOrWhiteSpace(config.CurrentlySelectedSaber))
        {
            return;
        }

        var saberData = await saberFactory.GetCurrentSaberDataAsync();
        token.ThrowIfCancellationRequested();

        var leftSaber = saberFactory.TryCreate(SaberType.SaberA, saberData);
        var rightSaber = saberFactory.TryCreate(SaberType.SaberB, saberData);

        previewSabers.ReplaceSabers(leftSaber, rightSaber);
        previewSabers.Init(leftPreviewSaberPosition, rightPreviewSaberPosition, leftPreviewRotation, rightPreviewRotation);

        var leftTrail = leftSaber.GetTrailsFromInstance()?[0];
        var rightTrail = rightSaber.GetTrailsFromInstance()?[0];

        previewTrails.SetTrails(leftTrail, rightTrail);

        UpdateTrailScale();
        UpdateColor();
        SetPreviewActive(true);
    }

    public void SetPreviewActive(bool active)
    {
        previewSabers.SetActive(active);
        previewTrails.SetActive(active);
    }

    public void UpdateTrailScale() => previewTrails.UpdateTrails(config);

    public void UpdateColor()
    {
        var selectedColorScheme = colorSchemesSettings.GetSelectedColorScheme();
        (var colorLeft, var colorRight) = 
            config.EnableCustomColorScheme ? (config.LeftSaberColor, config.RightSaberColor) 
            : (selectedColorScheme.saberAColor, selectedColorScheme.saberBColor);
        previewSabers?.SetColor(colorLeft, colorRight);
        previewTrails?.UpdateColor(colorLeft, colorRight);
    }
}
