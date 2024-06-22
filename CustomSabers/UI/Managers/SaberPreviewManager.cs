using CustomSabersLite.Components.Managers;
using CustomSabersLite.Configuration;
using CustomSabersLite.Data;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace CustomSabersLite.UI.Managers;

internal class SaberPreviewManager
{
    private readonly LiteSaberSet saberSet;
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

    public SaberPreviewManager(LiteSaberSet saberSet, CSLConfig config, ColorSchemesSettings colorSchemesSettings)
    {
        this.saberSet = saberSet;
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

        await saberSet.SetSabers(config.CurrentlySelectedSaber);
        token.ThrowIfCancellationRequested();


        previewSabers.SetSabers(saberSet);

        if (previewSabers == null)
        {
            Logger.Error("Couldn't get sabers for preview");
            return;
        }

        previewSabers.Init(leftPreviewSaberPosition, rightPreviewSaberPosition, leftPreviewRotation, rightPreviewRotation);

        UpdateTrailScale();
        UpdateColor();
        SetPreviewActive(true);
    }

    public void SetPreviewActive(bool active)
    {
        previewSabers.SetActive(active);
        previewTrails.SetActive(active);
    }

    public void UpdateTrailScale()
    {
        var trails = saberSet.Data?.Trails;
        if (trails == null || trails.Length == 0)
        {
            previewTrails.Clear();
            return;
        }

        var trail = trails[0];
        previewTrails.SwapMaterial(trail);

        var duration = !config.OverrideTrailDuration ? trail.Length
            : 0.4f * config.TrailDuration / 100f;

        var bottom = config.OverrideTrailWidth ? GetCustomWidthBottom(trail) :
            trail.Bottom.localPosition;

        previewTrails.UpdateVertices(bottom, trail.Top.localPosition, duration * 1.4f);
    }

    private Vector3 GetCustomWidthBottom(CustomTrailData trail)
    {
        var trailTop = trail.Top.localPosition;
        var trailBottom = trail.Bottom.localPosition;
        var distance = Vector3.Distance(trailTop, trailBottom);
        var width = distance > 0 ? config.TrailWidth / 100f / distance : 1f;
        return Vector3.LerpUnclamped(trailTop, trailBottom, width);
    }

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
