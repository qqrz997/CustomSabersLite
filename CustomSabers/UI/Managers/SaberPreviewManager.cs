using CustomSabersLite.Components.Managers;
using CustomSabersLite.Configuration;
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

    [Inject] private readonly MenuSaberManager menuSaberManager;
    [Inject] private readonly BasicPreviewTrailManager previewTrailManager;
    [Inject] private readonly BasicPreviewSaberManager previewSaberManager;

    // using these for now until i figure out how to get the actual physical position on the ui view
    private readonly Vector3 leftPreviewSaberPosition = new(0.72f, 0.9f, 4.1f);
    private readonly Vector3 rightPreviewSaberPosition = new(1.06f, 0.9f, 4.02f);
    private readonly Quaternion leftPreviewRotation = Quaternion.Euler(270f, 103.25f, 0f);
    private readonly Quaternion rightPreviewRotation = Quaternion.Euler(270f, 283.25f, 0f);

    private readonly Transform previewParent = new GameObject("__CustomSabersLite Basic Preview").transform;
    private readonly Transform leftPreviewParent = new GameObject("Left").transform;
    private readonly Transform rightPreviewParent = new GameObject("Right").transform;

    private bool previewActive;
    private bool previewGenerating;

    public void Initialize()
    {
        leftPreviewParent.SetParent(previewParent);
        rightPreviewParent.SetParent(previewParent);
        leftPreviewParent.SetPositionAndRotation(leftPreviewSaberPosition, leftPreviewRotation);
        rightPreviewParent.SetPositionAndRotation(rightPreviewSaberPosition, rightPreviewRotation);

        previewSaberManager.Init(leftPreviewParent, rightPreviewParent);
        previewTrailManager.Init(leftPreviewParent, rightPreviewParent);
    }
      
    public async Task GeneratePreview(CancellationToken token)
    {
        previewGenerating = true;
        UpdateActivePreview();

        var saberData = await saberFactory.GetCurrentSaberDataAsync();
        token.ThrowIfCancellationRequested();

        var leftSaber = saberFactory.TryCreate(SaberType.SaberA, saberData);
        var rightSaber = saberFactory.TryCreate(SaberType.SaberB, saberData);
        previewSaberManager.ReplaceSabers(leftSaber, rightSaber);
        previewTrailManager.SetTrails(leftSaber, rightSaber);

        var leftMenuSaber = saberFactory.TryCreate(SaberType.SaberA, saberData);
        var rightMenuSaber = saberFactory.TryCreate(SaberType.SaberB, saberData);
        menuSaberManager.ReplaceSabers(leftMenuSaber, rightMenuSaber);

        UpdateTrails();
        UpdateColor();

        previewGenerating = false;
        UpdateActivePreview();
    }

    public void SetPreviewActive(bool active)
    {
        previewActive = active;
        UpdateActivePreview();
    }

    public void UpdateActivePreview()
    {
        previewParent.gameObject.SetActive(previewActive && !previewGenerating && !config.EnableMenuSabers);
        menuSaberManager.SetActive(previewActive && !previewGenerating && config.EnableMenuSabers);
    }

    public void UpdateTrails()
    {
        previewTrailManager.UpdateTrails();
        menuSaberManager.UpdateTrails();
    }

    public void UpdateColor()
    {
        var selectedColorScheme = colorSchemesSettings.GetSelectedColorScheme();
        var (colorLeft, colorRight) = config.EnableCustomColorScheme ? (config.LeftSaberColor, config.RightSaberColor) 
            : (selectedColorScheme.saberAColor, selectedColorScheme.saberBColor);
        previewSaberManager.SetColor(colorLeft, colorRight);
        menuSaberManager.SetColor(colorLeft, colorRight);
        previewTrailManager.SetColor(colorLeft, colorRight);
    }
}
