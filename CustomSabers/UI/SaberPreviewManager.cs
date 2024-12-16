using CustomSabersLite.Configuration;
using CustomSabersLite.Utilities.Services;
using System;
using System.Threading;
using System.Threading.Tasks;
using CustomSabersLite.Models;
using CustomSabersLite.Utilities.Extensions;
using UnityEngine;
using Zenject;

namespace CustomSabersLite.UI;

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

    private SaberInstanceSet? basicPreviewSaberSet;
    private SaberInstanceSet? heldPreviewSaberSet;
    
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
        
        basicPreviewSaberSet?.Dispose();
        heldPreviewSaberSet?.Dispose();

        basicPreviewSaberSet = await saberFactory.InstantiateCurrentSabers();
        heldPreviewSaberSet = await saberFactory.InstantiateCurrentSabers();
        token.ThrowIfCancellationRequested();

        basicPreviewSaberManager.ReplaceSabers(basicPreviewSaberSet.LeftSaber, basicPreviewSaberSet.RightSaber);
        basicPreviewTrailManager.SetTrails(basicPreviewSaberSet.LeftSaber, basicPreviewSaberSet.RightSaber);

        menuSaberManager.ReplaceSabers(heldPreviewSaberSet.LeftSaber, heldPreviewSaberSet.RightSaber);

        UpdateTrails();
        UpdateSaberModels();
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
        bool previewIsActive = previewActive && !previewGenerating;
        previewParent.gameObject.SetActive(previewIsActive && !config.EnableMenuSabers);
        menuSaberManager.SetActive(previewIsActive && config.EnableMenuSabers);
    }

    public void UpdateTrails()
    {
        basicPreviewTrailManager.UpdateTrails();
        menuSaberManager.UpdateTrails();
    }

    public void UpdateSaberModels()
    {
        float length = config.OverrideSaberLength ? config.SaberLength : 1f;
        float width = config.OverrideSaberWidth ? config.SaberWidth : 1f;
        basicPreviewSaberManager.UpdateSaberScale(length, width);
        menuSaberManager.UpdateSaberScale(length, width);
        basicPreviewTrailManager.UpdateTrails();
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
