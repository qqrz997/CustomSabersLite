using CustomSabersLite.Configuration;
using CustomSabersLite.UI.Managers;
using CustomSabersLite.Utilities;
using CustomSabersLite.Utilities.Services;
using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace CustomSabersLite.UI;

#pragma warning disable IDE0031 // Use null propagation

internal class SaberPreviewManager : IInitializable, IDisposable
{
    [Inject] private readonly SaberFactory saberFactory = null!;
    [Inject] private readonly CSLConfig config = null!;
    [Inject] private readonly ColorSchemesSettings colorSchemesSettings = null!;

    [Inject] private readonly MenuSaberManager menuSaberManager = null!;
    [Inject] private readonly BasicPreviewTrailManager basicPreviewTrailManager = null!;
    [Inject] private readonly BasicPreviewSaberManager basicPreviewSaberManager = null!;

    private readonly Transform previewParent = new GameObject("CustomSabersLite Basic Preview").transform;
    private readonly Transform leftPreviewParent = new GameObject("Left").transform;
    private readonly Transform rightPreviewParent = new GameObject("Right").transform;

    private bool previewActive;
    private bool previewGenerating;

    public void Initialize()
    {
        leftPreviewParent.SetParent(previewParent);
        rightPreviewParent.SetParent(previewParent);

        previewParent.SetPositionAndRotation(new Vector3(0.7f, 0.8f, 1.1f), Quaternion.Euler(270f, 125f, 0f));
        leftPreviewParent.localPosition = new Vector3(0f, 0.17f, 0f);
        rightPreviewParent.localPosition = new Vector3(0f, -0.17f, 0f);
        rightPreviewParent.localRotation = Quaternion.Euler(0f, 0f, 180f);

        basicPreviewSaberManager.Init(leftPreviewParent, rightPreviewParent);
        basicPreviewTrailManager.Init(leftPreviewParent, rightPreviewParent);
    }

    public async Task GeneratePreview(CancellationToken token)
    {
        previewGenerating = true;
        UpdateActivePreview();

        var saberData = await saberFactory.GetCurrentSaberDataAsync();
        token.ThrowIfCancellationRequested();

        var leftSaber = saberFactory.Create(SaberType.SaberA, saberData);
        var rightSaber = saberFactory.Create(SaberType.SaberB, saberData);
        basicPreviewSaberManager.ReplaceSabers(leftSaber, rightSaber);
        basicPreviewTrailManager.SetTrails(leftSaber, rightSaber);

        var leftMenuSaber = saberFactory.Create(SaberType.SaberA, saberData);
        var rightMenuSaber = saberFactory.Create(SaberType.SaberB, saberData);
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
        basicPreviewTrailManager.UpdateTrails();
        menuSaberManager.UpdateTrails();
    }

    public void UpdateColor()
    {
        var selectedColorScheme = colorSchemesSettings.GetSelectedColorScheme();
        var (colorLeft, colorRight) = (selectedColorScheme.saberAColor, selectedColorScheme.saberBColor);
        basicPreviewSaberManager.SetColor(colorLeft, colorRight);
        menuSaberManager.SetColor(colorLeft, colorRight);
        basicPreviewTrailManager.SetColor(colorLeft, colorRight);
    }

    public void Dispose()
    {
        if (previewParent != null) previewParent.gameObject.Destroy();
    }
}
