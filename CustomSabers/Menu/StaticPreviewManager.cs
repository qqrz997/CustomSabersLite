using System;
using CustomSabersLite.Utilities.Extensions;
using SabersLib.Models;
using UnityEngine;
using Zenject;

namespace CustomSabersLite.Menu;

internal class StaticPreviewManager : IInitializable, IDisposable
{
    private readonly StaticPreviewSaberManager staticPreviewSaberManager;
    private readonly StaticPreviewTrailManager staticPreviewTrailManager;
    
    private readonly GameObject parent = new("StaticSaberPreview");
    private readonly Transform leftPreviewParent = new GameObject("Left").transform;
    private readonly Transform rightPreviewParent = new GameObject("Right").transform;

    public StaticPreviewManager(
        StaticPreviewSaberManager staticPreviewSaberManager,
        StaticPreviewTrailManager staticPreviewTrailManager)
    {
        this.staticPreviewSaberManager = staticPreviewSaberManager;
        this.staticPreviewTrailManager = staticPreviewTrailManager;
    }
    
    public (Transform leftParent, Transform rightParent) Parents => (leftPreviewParent, rightPreviewParent);
    
    public void Initialize()
    {
        leftPreviewParent.SetParent(parent.transform);
        rightPreviewParent.SetParent(parent.transform);

        parent.transform.SetPositionAndRotation(new(0.8f, 0.8f, 1.1f), Quaternion.Euler(270f, 125f, 0f));
        leftPreviewParent.localPosition = new(0f, 0.16f, 0f);
        rightPreviewParent.localPosition = new(0f, -0.16f, 0f);
        rightPreviewParent.localRotation = Quaternion.Euler(0f, 0f, 180f);

        staticPreviewSaberManager.Init(leftPreviewParent, rightPreviewParent);
        staticPreviewTrailManager.Init(leftPreviewParent, rightPreviewParent);
    }
    
    public void Dispose()
    {
        if (parent != null) parent.Destroy();
    }

    public void SetActive(bool active)
    {
        parent.SetActive(active);
    }

    public void ReplaceSabers(SaberInstanceSet saberSet)
    {
        staticPreviewSaberManager.ReplaceSabers(saberSet);
        staticPreviewTrailManager.SetTrails(saberSet);
    }

    public void UpdateTrails()
    {
        staticPreviewTrailManager.UpdateTrails();
    }
    
    public void UpdateSaberScale(float length, float width)
    {
        staticPreviewSaberManager.UpdateSaberScale(length, width);
        staticPreviewTrailManager.UpdateTrails();
    }

    public void SetColor(Color colorLeft, Color colorRight)
    {
        staticPreviewSaberManager.SetColor(colorLeft, colorRight);
        staticPreviewTrailManager.SetColor(colorLeft, colorRight);
    }
}