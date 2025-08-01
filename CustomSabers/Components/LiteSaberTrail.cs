using CustomSabersLite.Models;
using CustomSabersLite.Utilities.Common;
using CustomSabersLite.Utilities.Extensions;
using UnityEngine;
using Zenject;

namespace CustomSabersLite.Components;

internal class LiteSaberTrail : SaberTrail
{
    private readonly SaberMovementData customTrailMovementData = new();

    private TimeHelper timeHelper = null!;
    private Transform trailTop = null!;
    private Transform trailBottom = null!;
    private ITrailData trailData = null!;

    [Inject]
    public void Construct(
        TimeHelper timeHelper,
        InitData initData)
    {
        this.timeHelper = timeHelper;
        trailTop = initData.TrailTop;
        trailBottom = initData.TrailBottom;
        trailData = initData.TrailData;
        gameObject.layer = 12;
        _movementData = customTrailMovementData;
    }

    public float OverrideWidth { private get; set; } = 1f;
    public bool UseWidthOverride { private get; set; }
    
    public ITrailData TrailData => trailData;
    
    public void SetColor(Color color)
    {
        _color = (trailData.UseCustomColor ? trailData.CustomColor : color) * trailData.ColorMultiplier;
        
        foreach (var trailMaterial in _trailRenderer._meshRenderer.materials)
        {
            trailMaterial.SetColor(MaterialProperties.Color, _color);
        }
    }
    
    private new void Start()
    {
        // Ignored
    }

    private void Update()
    {
        if (!gameObject.activeInHierarchy) return;

        var topPos = trailTop.position;
        var bottomPos = trailBottom.position;
        
        customTrailMovementData.AddNewData(
            topPos,
            UseWidthOverride ? GetOverrideWidthBottom(OverrideWidth) : bottomPos,
            timeHelper.Time);

        return;
        
        Vector3 GetOverrideWidthBottom(float trailWidth)
        {
            float distance = Vector3.Distance(topPos, bottomPos);
            return distance.Approximately(0) ? bottomPos 
                : Vector3.LerpUnclamped(topPos, bottomPos, trailWidth / distance);
        }
    }
    
    internal record InitData(Transform TrailTop, Transform TrailBottom, ITrailData TrailData);
}