using CustomSabersLite.Components.Managers;
using CustomSabersLite.Configuration;
using CustomSabersLite.Data;
using CustomSabersLite.Utilities;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace CustomSabersLite.UI.Managers;

internal class SaberPreviewManager : IInitializable
{
    [Inject] private readonly SaberFactory saberFactory;
    [Inject] private readonly CSLConfig config;
    [Inject] private readonly ColorSchemesSettings colorSchemesSettings;
    [Inject] private readonly MenuSaberManager menuSabers;
    [Inject] private readonly MenuPointerProvider menuPointerProvider;

    private readonly PreviewSabers previewSabers = new();
    private readonly BasicPreviewTrails previewTrails = new();

    // using these for now until i figure out how to get the actual physical position on the ui view
    private readonly Vector3 leftPreviewSaberPosition = new(0.72f, 1.0f, 4.1f);
    private readonly Vector3 rightPreviewSaberPosition = new(1.06f, 1.0f, 4.02f);
    private readonly Quaternion leftPreviewRotation = Quaternion.Euler(270f, 103.25f, 0f);
    private readonly Quaternion rightPreviewRotation = Quaternion.Euler(270f, 283.25f, 0f);

    public void Initialize()
    {
        previewTrails.Init(leftPreviewSaberPosition, rightPreviewSaberPosition, leftPreviewRotation, rightPreviewRotation);
        previewSabers.Init(leftPreviewSaberPosition, rightPreviewSaberPosition, leftPreviewRotation, rightPreviewRotation);
        menuSabers.Init(menuPointerProvider.LeftPointer.transform, menuPointerProvider.RightPointer.transform);
    }

    public async Task GeneratePreview(CancellationToken token)
    {
        SetPreviewActive(false);

        if (string.IsNullOrWhiteSpace(config.CurrentlySelectedSaber)) return;

        var saberData = await saberFactory.GetCurrentSaberDataAsync();
        token.ThrowIfCancellationRequested();

        var leftSaber = saberFactory.TryCreate(SaberType.SaberA, saberData);
        var rightSaber = saberFactory.TryCreate(SaberType.SaberB, saberData);

        previewSabers.ReplaceSabers(leftSaber, rightSaber);

        CustomTrailData? leftTrail = leftSaber.InstanceTrails?.Length > 0 ? leftSaber.InstanceTrails[0] : null;
        CustomTrailData? rightTrail = rightSaber.InstanceTrails?.Length > 0 ? rightSaber.InstanceTrails[0] : null;

        previewTrails.SetTrails(leftTrail, rightTrail);

        menuSabers.ReplaceSabers(saberFactory.TryCreate(SaberType.SaberA, saberData), saberFactory.TryCreate(SaberType.SaberB, saberData));

        UpdateTrailScale();
        UpdateColor();
        SetPreviewActive(true);
    }

    public void SetPreviewActive(bool active)
    {
        previewSabers.SetActive(active);
        menuSabers.SetActive(active);
        previewTrails.SetActive(active);
    }

    public void UpdateTrailScale() => previewTrails.UpdateTrails(config);

    public void UpdateColor()
    {
        var selectedColorScheme = colorSchemesSettings.GetSelectedColorScheme();
        var (colorLeft, colorRight) = config.EnableCustomColorScheme ? (config.LeftSaberColor, config.RightSaberColor) 
            : (selectedColorScheme.saberAColor, selectedColorScheme.saberBColor);
        previewSabers.SetColor(colorLeft, colorRight);
        menuSabers.SetColor(colorLeft, colorRight);
        previewTrails.UpdateColor(colorLeft, colorRight);
    }
}
