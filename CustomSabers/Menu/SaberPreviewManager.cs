using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CustomSabersLite.Configuration;
using CustomSabersLite.Models;
using CustomSabersLite.Services;
using CustomSabersLite.Utilities.Extensions;
using UnityEngine;
using Zenject;

namespace CustomSabersLite.Menu;

internal class SaberPreviewManager
{
    private readonly SaberFactory saberFactory;
    private readonly PluginConfig config;
    private readonly ColorSchemesSettings colorSchemesSettings;
    private readonly ICoroutineStarter coroutineStarter;
    private readonly MenuSaberManager menuSaberManager;
    private readonly StaticPreviewManager staticPreviewManager;

    public SaberPreviewManager(
        SaberFactory saberFactory,
        PluginConfig config,
        ColorSchemesSettings colorSchemesSettings,
        ICoroutineStarter coroutineStarter,
        MenuSaberManager menuSaberManager,
        StaticPreviewManager staticPreviewManager)
    {
        this.saberFactory = saberFactory;
        this.config = config;
        this.colorSchemesSettings = colorSchemesSettings;
        this.coroutineStarter = coroutineStarter;
        this.menuSaberManager = menuSaberManager;
        this.staticPreviewManager = staticPreviewManager;
    }
    
    private readonly AnimationCurve animateSabersCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
    private readonly List<Coroutine> animations = [];

    private enum PreviewPosition { Animating, Generating, Static, Held }
    private const float AnimationDuration = 0.66f;
    
    private bool previewActive;
    private PreviewPosition previewPosition = PreviewPosition.Static;
    private SaberInstanceSet? saberSet;
    private CancellationTokenSource animationTokenSource = new();

    public async Task GeneratePreview(CancellationToken token)
    {
        CancelAnimations();
        SetNewPosition(PreviewPosition.Generating);
        
        Logger.Debug("Generating preview");
        
        saberSet?.Dispose();
        saberSet = await saberFactory.InstantiateCurrentSabers(token);
        token.ThrowIfCancellationRequested();
        
        menuSaberManager.ReplaceSabers(saberSet);
        staticPreviewManager.ReplaceSabers(saberSet);

        UpdateTrails();
        UpdateSaberModels();
        UpdateColor();
        
        UpdateActivePreviewInstant();
    }
    
    public void UpdateTrails()
    {
        if (!previewActive) return;
        staticPreviewManager.UpdateTrails();
        menuSaberManager.UpdateTrails();
    }

    public void UpdateSaberModels()
    {
        if (!previewActive) return;
        float length = config.OverrideSaberLength ? config.SaberLength : 1f;
        float width = config.OverrideSaberWidth ? config.SaberWidth : 1f;
        staticPreviewManager.UpdateSaberScale(length, width);
        menuSaberManager.UpdateSaberScale(length, width);
    }

    public void SetPreviewActive(bool active)
    {
        previewActive = active;
        UpdateActiveObjects();
    }
    
    public void UpdateActivePreviewAnimated()
    {
        bool isGenerating = previewPosition == PreviewPosition.Generating;
        if (!previewActive || isGenerating || saberSet is null) return;
        
        CancelAnimations();
        SetNewPosition(PreviewPosition.Animating);
        var token = animationTokenSource.Token;
        var (left, right) = config.EnableMenuSabers ? menuSaberManager.Parents : staticPreviewManager.Parents;
        var leftAnim = AnimateSaberToParent(saberSet.LeftSaber, left);
        var rightAnim = AnimateSaberToParent(saberSet.RightSaber, right);
        animations.AddRange(coroutineStarter.StartCoroutines(leftAnim, rightAnim));
        return;
        
        IEnumerator AnimateSaberToParent(ILiteSaber? saber, Transform target)
        {
            if (saber is null) yield break;
            var transform = saber.GameObject.transform;
            transform.parent = null;
            var (startPos, startRot) = (transform.position, transform.rotation);
            float t = 0f;
            while (t < 1f && !token.IsCancellationRequested)
            {
                t += Time.deltaTime / AnimationDuration;
                float tx = animateSabersCurve.Evaluate(t);
                transform.position = Vector3.Lerp(startPos, target.position, tx);
                transform.rotation = Quaternion.Slerp(startRot, target.rotation, tx);
                yield return null;
            }
            saber.SetParent(target);
            SetNewPosition(config.EnableMenuSabers ? PreviewPosition.Held : PreviewPosition.Static);
        }
    }

    private void UpdateColor()
    {
        if (!previewActive) return;
        var selectedColorScheme = colorSchemesSettings.GetSelectedColorScheme();
        var (colorLeft, colorRight) = (selectedColorScheme.saberAColor, selectedColorScheme.saberBColor);
        menuSaberManager.SetColor(colorLeft, colorRight);
        staticPreviewManager.SetColor(colorLeft, colorRight);
    }

    private void UpdateActivePreviewInstant()
    {
        if (!previewActive || saberSet is null) return;
        CancelAnimations();

        var (left, right) = config.EnableMenuSabers ? menuSaberManager.Parents : staticPreviewManager.Parents;
        saberSet.LeftSaber?.SetParent(left);
        saberSet.RightSaber?.SetParent(right);
        
        SetNewPosition(config.EnableMenuSabers ? PreviewPosition.Held : PreviewPosition.Static);
    }

    private void SetNewPosition(PreviewPosition position)
    {
        previewPosition = position;
        UpdateActiveObjects();
    }

    private void UpdateActiveObjects()
    {
        bool isHeld = previewPosition == PreviewPosition.Held;
        bool isAnimating = previewPosition == PreviewPosition.Animating;
        bool isGenerating = previewPosition == PreviewPosition.Generating;
        bool sabersActive = previewActive && !isGenerating && !isAnimating;
        staticPreviewManager.SetActive(sabersActive && !isHeld);
        menuSaberManager.SetActive(sabersActive && isHeld);
        if (previewActive || saberSet == null) return;
        saberSet.LeftSaber?.GameObject.SetActive(false);
        saberSet.RightSaber?.GameObject.SetActive(false);
    }
    
    private void CancelAnimations()
    {
        animationTokenSource.CancelThenDispose();
        animationTokenSource = new();
        animations.ForEach(coroutineStarter.StopCoroutine);
        animations.Clear();
    }
}
