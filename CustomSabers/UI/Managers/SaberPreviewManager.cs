﻿using CustomSabersLite.Components.Managers;
using CustomSabersLite.Configuration;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace CustomSabersLite.UI.Managers;

internal class SaberPreviewManager : IInitializable
{
    [Inject] private readonly SaberFactory saberFactory = null!;
    [Inject] private readonly CSLConfig config = null!;
    [Inject] private readonly ColorSchemesSettings colorSchemesSettings = null!;

    [Inject] private readonly MenuSaberManager menuSaberManager = null!;
    [Inject] private readonly BasicPreviewTrailManager previewTrailManager = null!;
    [Inject] private readonly BasicPreviewSaberManager previewSaberManager = null!;

    private readonly Transform previewParent = new GameObject("CustomSabersLite Basic Preview").transform;
    private readonly Transform leftPreviewParent = new GameObject("Left").transform;
    private readonly Transform rightPreviewParent = new GameObject("Right").transform;

    private bool previewActive;
    private bool previewGenerating;

    public void Initialize()
    {
        leftPreviewParent.SetParent(previewParent);
        rightPreviewParent.SetParent(previewParent);

        previewParent.SetPositionAndRotation(new Vector3(1.1f, 0.8f, 1.1f), Quaternion.Euler(270f, 135f, 0f));
        leftPreviewParent.localPosition = new Vector3(0f, 0.2f, 0f);
        rightPreviewParent.localPosition = new Vector3(0f, -0.2f, 0f);
        rightPreviewParent.localRotation = Quaternion.Euler(0f, 0f, 180f);

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
        var (colorLeft, colorRight) = (selectedColorScheme.saberAColor, selectedColorScheme.saberBColor);
        previewSaberManager.SetColor(colorLeft, colorRight);
        menuSaberManager.SetColor(colorLeft, colorRight);
        previewTrailManager.SetColor(colorLeft, colorRight);
    }
}
