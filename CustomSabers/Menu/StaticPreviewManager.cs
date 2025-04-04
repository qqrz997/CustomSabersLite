using System;
using CustomSabersLite.Models;
using CustomSabersLite.Utilities.Extensions;
using UnityEngine;
using Zenject;

namespace CustomSabersLite.Menu;

internal class StaticPreviewManager : IInitializable, IDisposable
{
    private readonly BasicPreviewSaberManager basicPreviewSaberManager;
    private readonly BasicPreviewTrailManager basicPreviewTrailManager;
    
    private readonly GameObject parent = new("CustomSabersLiteBasicPreview");
    private readonly Transform leftPreviewParent = new GameObject("Left").transform;
    private readonly Transform rightPreviewParent = new GameObject("Right").transform;

    public StaticPreviewManager(
        BasicPreviewSaberManager basicPreviewSaberManager,
        BasicPreviewTrailManager basicPreviewTrailManager)
    {
        this.basicPreviewSaberManager = basicPreviewSaberManager;
        this.basicPreviewTrailManager = basicPreviewTrailManager;
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

        basicPreviewSaberManager.Init(leftPreviewParent, rightPreviewParent);
        basicPreviewTrailManager.Init(leftPreviewParent, rightPreviewParent);
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
        basicPreviewSaberManager.ReplaceSabers(saberSet);
        basicPreviewTrailManager.SetTrails(saberSet);
    }

    public void UpdateTrails()
    {
        basicPreviewTrailManager.UpdateTrails();
    }
    
    public void UpdateSaberScale(float length, float width)
    {
        basicPreviewSaberManager.UpdateSaberScale(length, width);
        basicPreviewTrailManager.UpdateTrails();
    }

    public void SetColor(Color colorLeft, Color colorRight)
    {
        basicPreviewSaberManager.SetColor(colorLeft, colorRight);
        basicPreviewTrailManager.SetColor(colorLeft, colorRight);
    }
}